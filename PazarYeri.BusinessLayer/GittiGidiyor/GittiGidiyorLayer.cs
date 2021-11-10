using System;
using GittiGidiyor;
using Newtonsoft.Json;
using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GittiGidiyor.Product;
using GittiGidiyor.Sale;
using PazarYeri.Models.Common;
using UnityObjects;

namespace PazarYeri.BusinessLayer.GittiGidiyor
{
    public class GittiGidiyorLayer
    {
        private readonly net_LogoTransferSettings _transferSettings;
        private readonly SaleService _saleService;
        private readonly ProductService _productService;
        private readonly DatabaseLayer _databaseLayer;

        //com.gittigidiyor.dev.DeveloperServiceService DeveloperService = new com.gittigidiyor.dev.DeveloperServiceService();
        //com.gittigidiyor.dev_sales.IndividualSaleServiceService service = new com.gittigidiyor.dev_sales.IndividualSaleServiceService();

        private const string _entegrationName = "GittiGidiyor";

        public GittiGidiyorLayer()
        {
            _databaseLayer = new DatabaseLayer();
            _transferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);

            var config = new AuthConfig
            {
                ApiKey = "gDawdPnxa6EvxeYZKqys3GzwxY6BCeQS",
                SecretKey = "aqwTVCyWVxtQyWTS",
                RoleName = "angtasarim",
                RolePass = "9VRVDz7NFf88wXMJjr6e2SgqtByBvhZ9"
            };

            ConfigurationManager.setAuthParameters(config);

            _saleService = ServiceProvider.getSaleService();
            _productService = ServiceProvider.getProductService();
        }

        public bool OrderTransfer(net_GittiGidiyor netorder, out string result)
        {
            var order = MapGittiGidiyor(netorder);

            var unityManager = new UnityManager(_entegrationName);

            unityManager.PostOrder(order, out result, out var ficheRef);

            return ficheRef > 0;
        }

        private CommonOrder MapGittiGidiyor(net_GittiGidiyor netorder)
        {
            var clientCode = _transferSettings.OrderTransferArpCode == ""
                ? _databaseLayer.GetClientCodeFromPairingTable(netorder.buyerInfo.email, _entegrationName)
                : _transferSettings.OrderTransferArpCode;

            if (string.IsNullOrEmpty(clientCode))
                clientCode = _transferSettings.ClientTransferPrefixCode + _databaseLayer.GetLastClCode(_transferSettings.ClientTransferPrefixCode);

            var date = netorder.lastActionDate;

            var commonOrder = new CommonOrder()
            {
                Header = new CommonOrderHeader()
                {
                    Number = "~",
                    Date = Convert.ToDateTime(
                        $"{date.Substring(0, 2)}." +
                        $"{date.Substring(3, 2)}." +
                        $"{date.Substring(6, 4)}"),
                    ArpCode = clientCode,
                    CustOrdNo = netorder.saleCode,
                    SourceWh = _transferSettings.OrderTransferWareHouseNr,
                    SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                    DocNumber = _transferSettings.OrderTransferDoCode,
                    PaymentCode = _transferSettings.OrderTransferPaymentCode,
                    Division = _transferSettings.OrderTransferDivisionNr,
                    SalesmanCode = _transferSettings.OrderTransferSalesManCode,
                    TradingGrp = _transferSettings.OrderTransferTradingGroup,
                    AuxilCode = _transferSettings.OrderTransferSpeCode,
                    AuthCode = _transferSettings.OrderTransferAuthCode,
                    ArpCodeShpm = _transferSettings.OrderTransferArpShippmentCode,
                    OrderStatus = _transferSettings.OrderTransferStatus,
                    CurrselTotal = 1,
                    AffectRisk = 0,
                    ProjectCode = _transferSettings.OrderTransferProjectCode,
                    CargoNo = "",
                    DocTrackNr = netorder.cargoCode,
                    EInvoice = 2,
                    EArchiveTrInsteadOfDesp = 1,
                    EArchiveTrSendMod = 2,
                    EInvoiceProfileId = 2,                    
                }
            };

            commonOrder.Header.ArAps = new CommonArAps()
            {
                AccountType = 3,
                Code = clientCode,
                Title = $"{netorder.buyerInfo.name} {netorder.buyerInfo.surname}",
                City = netorder.buyerInfo.city,
                Town = netorder.buyerInfo.district,
                AuxilCode = _transferSettings.ClientTransferSpeCode1,
                AuxilCode2 = _transferSettings.ClientTransferSpeCode2,
                AuxilCode3 = _transferSettings.ClientTransferSpeCode3,
                AuxilCode4 = _transferSettings.ClientTransferSpeCode4,
                AuxilCode5 = _transferSettings.ClientTransferSpeCode5,
                AuthCode = _transferSettings.ClientTransferAuthCode,
                Country = "TÜRKİYE",
                CountryCode = "TR",
                ProfilId = 2,
                EArchiveSendMode = 2,
                TradingGrp = _transferSettings.ClientTransferTradingGroup,
                PaymentCode = _transferSettings.ClientTransferPaymentCode,
                ProjectCode = _transferSettings.ClientTransferProjectCode,
                GlCode = _transferSettings.ClientTransferAccountCode,
                Telephone1 = netorder.buyerInfo.mobilePhone,
                Telephone2 = netorder.buyerInfo.phone,
                EMail = netorder.buyerInfo.email,
                Contact = $"{netorder.buyerInfo.name} {netorder.buyerInfo.surname}",
                Address1 = netorder.buyerInfo.address.Length > 200 ? netorder.buyerInfo.address.Substring(0, 199) : netorder.buyerInfo.address,
                Address2 = netorder.buyerInfo.address.Length > 200 ? netorder.buyerInfo.address.Substring(200) : netorder.buyerInfo.address,
                PersCompany = 1,
                TCKNO = "11111111111",
                Name = netorder.buyerInfo.name,
                SurName = netorder.buyerInfo.surname,
                InsteadOfDispatch = 1
            };

            commonOrder.Header.ShippingAddress = new CommonShippingAddress()
            {
                Code = ""
            };

            var product = _productService.getProduct(
                netorder.productId.ToString(),
                "",
                "tr");

            var lines = new List<CommonOrderLine>
            {
                new CommonOrderLine()
                {
                    Type = 0,
                    MasterCode = _databaseLayer.GetItemCode(product.productDetail.product.globalTradeItemNo),
                    Quantity = (double) netorder.amount,
                    Price = Convert.ToDouble(netorder.price.Replace(".", ",")) / 1.18 / netorder.amount,
                    VatRate = 18,
                    UnitCode = _databaseLayer.GetItemMainUnit(_databaseLayer.GetItemCode(product.productDetail.product.globalTradeItemNo)),
                    UnitConv1 = 1,
                    UnitConv2 = 1,
                    Total = (Convert.ToDouble(netorder.price.Replace(".", ",")) / 1.18),
                    CalcType = 0,
                    SourceWh = _transferSettings.OrderTransferWareHouseNr,
                    SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                    Division = _transferSettings.OrderTransferDivisionNr,
                    SalesManCode = _transferSettings.OrderTransferSalesManCode,
                    ProjectCode = _transferSettings.OrderTransferProjectCode,
                    AuxilCode = _transferSettings.OrderTransferSpeCode,
                    AuthCode = _transferSettings.OrderTransferAuthCode,
                }
            };

            commonOrder.Lines = lines;

            return commonOrder;
        }

        public List<net_GittiGidiyor> getOrders()
        {
            var order = _saleService.getSales(
                true,
                "S",
                "",
                "A",
                "D",
                1,
                100,
                "tr");

            if (order == null)
                return new List<net_GittiGidiyor>();

            if (order.sales == null)
                return new List<net_GittiGidiyor>();

            if (!order.sales.Any())
                return new List<net_GittiGidiyor>();

            var gittiGidiyors = new List<net_GittiGidiyor>();

            var gittiGidiyorSerializer = new XmlSerializer(typeof(net_GittiGidiyor));

            foreach (var sale in order.sales.OrderByDescending(x => x.saleCode))
            {
                var orderStr = JsonConvert.SerializeObject(sale);

                var gittiGidiyor = JsonConvert.DeserializeObject<net_GittiGidiyor>(orderStr);

                gittiGidiyors.Add(gittiGidiyor);

                var entegratedOrder = new net_EntegratedOrders();

                using (var stringwriter = new StringWriter())
                {
                    gittiGidiyorSerializer.Serialize(stringwriter, gittiGidiyor);
                    var returnStr = stringwriter.ToString();
                    entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                }

                entegratedOrder.EntegrationName = _entegrationName;
                entegratedOrder.OrderDate = Convert.ToDateTime(sale.lastActionDate);
                entegratedOrder.OrderNo = sale.saleCode;
                entegratedOrder.LineNr = "1";
                entegratedOrder.Transfered = 0;

                _databaseLayer.saveOrders(entegratedOrder);
            }

            return gittiGidiyors;
        }

        public string GetItemBarcodeFromGittiGidiyor(int productId)
        {
            return _productService.getProduct(
                productId.ToString(),
                "",
                "tr").productDetail.product.globalTradeItemNo;
        }

        public bool transferOrder(List<net_GittiGidiyor> list, out string result)
        {
            result = "";

            var netorder = list.First();

            try
            {
                var clCode = _databaseLayer.GetClCodeByUserName(netorder.buyerInfo.username);

                if (string.IsNullOrEmpty(clCode))
                {
                    clCode = "120." + _databaseLayer.GetLastClCode("120.");

                    CreateClCard(netorder.buyerInfo, clCode);
                }

                var order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);

                order.New();
                order.DataFields.FieldByName("NUMBER").Value = "~";
                order.DataFields.FieldByName("DOC_TRACK_NR").Value = netorder.cargoCode;

                var date = netorder.lastActionDate;

                order.DataFields.FieldByName("DATE").Value = Convert.ToDateTime(
                    $"{date.Substring(0, 2)}." +
                    $"{date.Substring(3, 2)}." +
                    $"{date.Substring(6, 4)}");
                
                order.DataFields.FieldByName("ARP_CODE").Value = clCode;
                order.DataFields.FieldByName("CUST_ORD_NO").Value = netorder.saleCode;

                if (!string.IsNullOrEmpty(_transferSettings.OrderTransferSpeCode))
                {
                    order.DataFields.FieldByName("AUXIL_CODE").Value = _transferSettings.OrderTransferSpeCode;
                }

                order.DataFields.FieldByName("ORDER_STATUS").Value = 4;
                order.DataFields.FieldByName("CURRSEL_TOTAL").Value = 1;
                order.DataFields.FieldByName("AFFECT_RISK").Value = 0;

                var transactions = order.DataFields.FieldByName("TRANSACTIONS").Lines;

                foreach (var item in list)
                {
                    var product = _productService.getProduct(
                        item.productId.ToString(),
                        "",
                        "tr");

                    transactions.AppendLine();
                    transactions[transactions.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions[transactions.Count - 1].FieldByName("MASTER_CODE").Value =
                        _databaseLayer.GetItemCode(product.productDetail.product.globalTradeItemNo);
                    transactions[transactions.Count - 1].FieldByName("AUXIL_CODE").Value = "";
                    transactions[transactions.Count - 1].FieldByName("QUANTITY").Value = item.amount;
                    //transactions[transactions.Count - 1].FieldByName("PRICE").Value = Math.Round((Convert.ToDouble(netorder.price) / 1.18), 2);
                    transactions[transactions.Count - 1].FieldByName("PRICE").Value = Convert.ToDouble(item.price.Replace(".", ",")) / 1.18 / (item.amount == 0 ? 1 : item.amount);
                    transactions[transactions.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                }
                order.SetClientInfo();
                //order.ExportToXML("SALES_ORDERS", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Xml\{orfiches[0].SiparisNo}.xml");
                if (order.Post() == true)
                {
                    int logicalref = order.DataFields.FieldByName("INTERNAL_REFERENCE").Value;

                    //  InsertShipAddr(logicalref, orfiches[0].ShipAddr);

                    double NetTotal = order.DataFields.FieldByName("TOTAL_NET").Value;

                    CreateArpVoucher(netorder, clCode, NetTotal);

                    return true;
                }

                if (order.ErrorCode != 0)
                {
                    result = "DBError(" + order.ErrorCode.ToString() + ")-" + order.ErrorDesc + order.DBErrorDesc;
                }
                else if (order.ValidateErrors.Count > 0)
                {
                    result = "XML ErrorList:";
                    for (int i = 0; i < order.ValidateErrors.Count; i++)
                    {
                        result += "(" + order.ValidateErrors[i].ID.ToString() + ") - " + order.ValidateErrors[i].Error;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return false;
        }

        private void CreateArpVoucher(net_GittiGidiyor order, string clCode, double netTotal)
        {
            var arpvoucher = Global.application.NewDataObject(DataObjectType.doARAPVoucher);

            arpvoucher.New();
            arpvoucher.DataFields.FieldByName("NUMBER").Value = "~";

            var date = order.lastActionDate;

            arpvoucher.DataFields.FieldByName("DATE").Value = Convert.ToDateTime(
                $"{date.Substring(0, 2)}." +
                $"{date.Substring(3, 2)}." +
                $"{date.Substring(6, 4)}");
            ;

            arpvoucher.DataFields.FieldByName("TYPE").Value = 5;
            arpvoucher.DataFields.FieldByName("NOTES1").Value = order.saleCode;
            arpvoucher.DataFields.FieldByName("TOTAL_DEBIT").Value = netTotal;
            arpvoucher.DataFields.FieldByName("TOTAL_CREDIT").Value = netTotal;

            var transactions_lines = arpvoucher.DataFields.FieldByName("TRANSACTIONS").Lines;

            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ARP_CODE").Value = clCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("DOC_NUMBER").Value = order.saleCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = order.saleCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("CREDIT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TC_AMOUNT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("AFFECT_RISK").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;

            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpShippmentCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("DOC_NUMBER").Value = order.saleCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = order.saleCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DEBIT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TC_AMOUNT").Value = netTotal;

            transactions_lines[transactions_lines.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;

            arpvoucher.DataFields.FieldByName("AFFECT_RISK").Value = 0;

            if (arpvoucher.Post() == true)
            {
            }
            else
            {
                if (arpvoucher.ErrorCode != 0)
                {
                    //  MessageBox.Show("DBError(" + arpvoucher.ErrorCode.ToString() + ")-" + arpvoucher.ErrorDesc + arpvoucher.DBErrorDesc);
                }
                else if (arpvoucher.ValidateErrors.Count > 0)
                {
                    string result = "XML ErrorList:";
                    for (int i = 0; i < arpvoucher.ValidateErrors.Count; i++)
                    {
                        result += "(" + arpvoucher.ValidateErrors[i].ID.ToString() + ") - " + arpvoucher.ValidateErrors[i].Error;
                    }

                    // MessageBox.Show(result);
                }
            }
        }

        private string CreateClCard(BuyerInfo buyer, string clcode)
        {
            string result = "";

            var arps = Global.application.NewDataObject(DataObjectType.doAccountsRP);
            arps.New();
            arps.DataFields.FieldByName("ACCOUNT_TYPE").Value = 3;
            arps.DataFields.FieldByName("CODE").Value = clcode;
            arps.DataFields.FieldByName("TITLE").Value = $"{buyer.name} {buyer.surname}";
            arps.DataFields.FieldByName("TITLE2").Value = buyer.username;
            arps.DataFields.FieldByName("ADDRESS1").Value = buyer.address;
            arps.DataFields.FieldByName("ADDRESS2").Value = "";
            arps.DataFields.FieldByName("CITY").Value = buyer.city;
            arps.DataFields.FieldByName("TOWN").Value = buyer.district;
            arps.DataFields.FieldByName("AUXIL_CODE5").Value = "";
            arps.DataFields.FieldByName("COUNTRY_CODE").Value = "TR";
            arps.DataFields.FieldByName("COUNTRY").Value = "TÜRKİYE";

            arps.DataFields.FieldByName("CORRESP_LANG").Value = 1;

            arps.DataFields.FieldByName("CL_ORD_FREQ").Value = 1;
            arps.DataFields.FieldByName("INVOICE_PRNT_CNT").Value = 1;
            arps.DataFields.FieldByName("PURCHBRWS").Value = 1;
            arps.DataFields.FieldByName("SALESBRWS").Value = 1;
            arps.DataFields.FieldByName("IMPBRWS").Value = 1;
            arps.DataFields.FieldByName("EXPBRWS").Value = 1;
            arps.DataFields.FieldByName("FINBRWS").Value = 1;
            arps.DataFields.FieldByName("COLLATRLRISK_TYPE").Value = 1;
            arps.DataFields.FieldByName("RISK_TYPE1").Value = 1;
            arps.DataFields.FieldByName("RISK_TYPE2").Value = 1;
            arps.DataFields.FieldByName("RISK_TYPE3").Value = 1;

            arps.DataFields.FieldByName("PERSCOMPANY").Value = 1;
            arps.DataFields.FieldByName("TCKNO").Value = "11111111111";

            arps.DataFields.FieldByName("NAME").Value = buyer.name;
            arps.DataFields.FieldByName("SURNAME").Value = buyer.surname;

            arps.DataFields.FieldByName("PROFILE_ID").Value = 2;
            arps.DataFields.FieldByName("EARCHIVE_SEND_MODE").Value = 2;
            arps.DataFields.FieldByName("CREATE_WH_FICHE").Value = 1;

            if (arps.Post() == true)
            {
            }
            else
            {
                if (arps.ErrorCode != 0)
                {
                    result = "DBError(" + arps.ErrorCode.ToString() + ")-" + arps.ErrorDesc + arps.DBErrorDesc;
                }
                else if (arps.ValidateErrors.Count > 0)
                {
                    result = "XML ErrorList:";
                    for (int i = 0; i < arps.ValidateErrors.Count; i++)
                    {
                        result += "(" + arps.ValidateErrors[i].ID.ToString() + ") - " + arps.ValidateErrors[i].Error;
                    }
                }
            }

            return result;
        }

        class registerDeveloper
        {
            public string nick { get; set; } = string.Empty;
            public string password { get; set; } = string.Empty;
            public string lang { get; set; } = string.Empty;
        }
    }
}