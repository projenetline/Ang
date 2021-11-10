using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityObjects;
using PazarYeri.Models;
using System.Xml.Serialization;
using PazarYeri.Models.Settings;
using System.Collections.Generic;
using PazarYeri.BusinessLayer.Utility;
using Newtonsoft.Json;
using PazarYeri.BusinessLayer.Helpers;
using System.Net.Http;
using PazarYeri.BusinessLayer.ServiceReference2;

namespace PazarYeri.BusinessLayer.HepsiBurada
{
    public class HepsiBuradaLayer
    {
        private readonly DatabaseLayer _databaseLayer;
        private readonly com.hepsiburada.b2b.orders3rdParty orders3 = new com.hepsiburada.b2b.orders3rdParty();
        private readonly net_EntegrationSettings _entegrationSettings;
        private readonly net_LogoTransferSettings _transferSettings;
        private readonly ProjectUtil _util;
        private HepsiBuradaRestModel _hepsiBuradaOrderList;

        private const string _entegrationName = "HepsiBurada";

        public HepsiBuradaLayer()
        {
            _databaseLayer = new DatabaseLayer();
            _entegrationSettings = _databaseLayer.GetEntegrationSettings(_entegrationName);
            _transferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);
            _util = new ProjectUtil();
        }

        public void GetHepsiBuradaOrders()
        {
            try
            {
                new ProjectUtil().LogService($"HepsiBurada Siparişleri Okunacak. " +
                                             $"FirmCode: {_entegrationSettings.FirmCode}, " +
                                             $"UserName: {_entegrationSettings.UserName}, " +
                                             $"Password: {_entegrationSettings.PassWord}");

                //var items = orders3.GetOpenOrders(
                //    _entegrationSettings.FirmCode,
                //    _entegrationSettings.UserName,
                //    _entegrationSettings.PassWord);

                var client = new orders3rdPartyNewSoapClient();

                var items = client.GetOpenOrders(
                    _entegrationSettings.FirmCode,
                    _entegrationSettings.UserName,
                    _entegrationSettings.PassWord);

                if (items != null)
                {
                    foreach (var subList in items)
                    {
                        foreach (var item in subList.SasItemList)
                        {
                            var hepsiBurada = new net_HepsiBurada()
                            {
                                Address = subList.Address,
                                CreatedDate = Convert.ToDateTime(subList.Date),
                                Currency = "TRY",
                                DeliveryDate = item.EstimatedShipmentDate,
                                HBSKU = item.Sku,
                                PackageNote = item.CustomerNote,
                                ProductName = item.ProductName,
                                Quantity = Convert.ToDouble(item.Adet),
                                ReceiverName = subList.ReceiverName,
                                SasID = subList.SasNo,
                                SasItemNr = item.KalemNo,
                                SasNo = subList.SasNo,
                                TotalPrice = Convert.ToDouble(item.TotalPriceWithoutTax) +
                                             (Convert.ToDouble(item.TotalPriceWithoutTax) *
                                                 Convert.ToDouble(item.TaxRate) / 100.0),
                                UnitPrice = Convert.ToDouble(item.TotalPriceWithoutTax) / Convert.ToDouble(item.Adet),
                                adres1 = subList.AddressInfo.adres1,
                                adres2 = subList.AddressInfo.adres2,
                                adres3 = subList.AddressInfo.adres3,
                                city = subList.AddressInfo.city,
                                country = subList.AddressInfo.country,
                                district = subList.AddressInfo.district,
                                isim = subList.AddressInfo.isim,
                                isimgonderen = subList.AddressInfo.isimgonderen,
                                telgonderen = subList.AddressInfo.telgonderen,
                                town = subList.AddressInfo.town,
                                TedSKU = item.Sku
                                //TedSKU = item.TedSku
                            };

                            var pairing = new net_ProductPairing
                                {EntegrationCode = item.Sku, EntegrationName = _entegrationName};
                            pairing = _databaseLayer.GetProductPairing(pairing);

                            if (pairing != null && string.IsNullOrEmpty(item.TedSku) && item.TedSku != "-")
                            {
                                hepsiBurada.TedSKU = pairing.LogoCode;
                            }

                            var orderStatus = _databaseLayer.GetOrder(_entegrationName, subList.SasNo,
                                Convert.ToInt32(item.KalemNo));

                            var entegratedOrder = new net_EntegratedOrders();
                            using (var stringwriter = new System.IO.StringWriter())
                            {
                                var serializer = new XmlSerializer(typeof(net_HepsiBurada));
                                serializer.Serialize(stringwriter, hepsiBurada);
                                var returnStr = stringwriter.ToString();
                                entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                            }

                            entegratedOrder.EntegrationName = _entegrationName;
                            entegratedOrder.OrderDate = hepsiBurada.CreatedDate;
                            entegratedOrder.OrderNo = hepsiBurada.SasNo;
                            entegratedOrder.LineNr = hepsiBurada.SasItemNr.ToString();
                            entegratedOrder.Transfered = 0;
                            var database = new DatabaseLayer();
                            database.saveOrders(entegratedOrder);
                        }
                    }
                }
                else
                {
                    new ProjectUtil().LogService($"HepsiBurada - Açık Sipariş Okunamadı");
                }
            }
            catch (Exception ex)
            {
                new ProjectUtil().LogService("[Error-HepsiBurada] (GetHepsiBuradaOrders)" + Environment.NewLine +
                                             ex.Message);
            }
        }

        public bool TransferOrders(List<net_HepsiBurada> list, out string result)
        {
            var transfered = false;
            result = "";

            try
            {
                var netorder = list.First();

                //int OrderRef = 0;
                // _databaseLayer.GetOrderRef(_entegrationSettings.OrderTransferSpeCode, netorder.orderNumber, netorder.coString());

                var clCode = _databaseLayer.GetClCodeByTCKN(netorder.email);

                if (string.IsNullOrEmpty(clCode))
                {
                    clCode = "120." + _databaseLayer.GetLastClCode("120.");
                    CreateClCard(netorder, clCode);
                }

                var order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);

                order.New();
                order.DataFields.FieldByName("NUMBER").Value = "~";

                //  order.DataFields.FieldByName("DOC_TRACK_NR").Value = netorder.cargoTrackingNumber.ToString();
                order.DataFields.FieldByName("DATE").Value = netorder.CreatedDate.ToString("dd.MM.yyyy"); //"dd.MM.yyyy"
                order.DataFields.FieldByName("ARP_CODE").Value = clCode;
                order.DataFields.FieldByName("CUST_ORD_NO").Value = netorder.SasNo;
                order.DataFields.FieldByName("SHIPPING_AGENT").Value = "ARAS";

                order.DataFields.FieldByName("EINVOICE").Value = 2;
                order.DataFields.FieldByName("EINVOICE_PROFILEID").Value = 2;
                order.DataFields.FieldByName("EARCHIVEDETR_SENDMOD").Value = 2;
                order.DataFields.FieldByName("EARCHIVEDETR_INSTEADOFDESP").Value = 1;

                if (!string.IsNullOrEmpty(_transferSettings.OrderTransferSpeCode))
                {
                    order.DataFields.FieldByName("AUXIL_CODE").Value = _transferSettings.OrderTransferSpeCode;
                }

                if (!string.IsNullOrEmpty(_transferSettings.OrderTransferArpShippmentCode))
                {
                    order.DataFields.FieldByName("ARP_CODE_SHPM").Value =
                        _transferSettings.OrderTransferArpShippmentCode;
                }

                order.DataFields.FieldByName("ORDER_STATUS").Value = 4;
                order.DataFields.FieldByName("CURRSEL_TOTAL").Value = 1;

                var transactions = order.DataFields.FieldByName("TRANSACTIONS").Lines;

                foreach (var line in list)
                {
                    transactions.AppendLine();
                    transactions[transactions.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions[transactions.Count - 1].FieldByName("MASTER_CODE").Value = line.TedSKU;
                    transactions[transactions.Count - 1].FieldByName("AUXIL_CODE").Value =
                    transactions[transactions.Count - 1].FieldByName("QUANTITY").Value = line.Quantity;
                    transactions[transactions.Count - 1].FieldByName("PRICE").Value = Math.Round((line.UnitPrice), 2);
                    transactions[transactions.Count - 1].FieldByName("VAT_INCLUDED").Value = 1;
                    transactions[transactions.Count - 1].FieldByName("VAT_RATE").Value = line.vatRate;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                }

                order.DataFields.FieldByName("AFFECT_RISK").Value = 0;
                order.SetClientInfo();
                //order.ExportToXML("SALES_ORDERS", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Xml\{orfiches[0].SiparisNo}.xml");
                if (order.Post() == true)
                {
                    int logicalref = order.DataFields.FieldByName("INTERNAL_REFERENCE").Value;

                    //  InsertShipAddr(logicalref, orfiches[0].ShipAddr);
                    transfered = true;
                }
                else
                {
                    if (order.ErrorCode != 0)
                    {
                        result = "DBError(" + order.ErrorCode.ToString() + ")-" + order.ErrorDesc + order.DBErrorDesc;
                        _util.LogService($"Aktarım Hatası : " + result);
                    }
                    else if (order.ValidateErrors.Count > 0)
                    {
                        result = "XML ErrorList:";
                        for (var i = 0; i < order.ValidateErrors.Count; i++)
                        {
                            result += "(" + order.ValidateErrors[i].ID.ToString() + ") - " +
                                      order.ValidateErrors[i].Error;
                        }

                        _util.LogService($"Aktarım Hatası : " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                _util.LogService(result);
            }

            return transfered;
        }

        public void GetHepsiBuradaOrdersFromRestService(DateTime begdate, DateTime enddate)
        {
            var pageNr = 0;
            var nextPage = true;
            while (nextPage)
            {
                var result = HTTP_GET(pageNr, begdate, enddate);
                try
                {
                    _hepsiBuradaOrderList = JsonConvert.DeserializeObject<HepsiBuradaRestModel>(result.Result);

                    if (_hepsiBuradaOrderList != null)
                    {
                        if (_hepsiBuradaOrderList.items != null)
                        {
                            foreach (var hepsiBuradaOrder in _hepsiBuradaOrderList.items)
                            {
                                var hepsiBurada = new net_HepsiBurada()
                                {
                                    Address = hepsiBuradaOrder.shippingAddress.address,
                                    CreatedDate = Convert.ToDateTime(hepsiBuradaOrder.orderDate),
                                    Currency = "TRY",
                                    DeliveryDate = Convert.ToDateTime(hepsiBuradaOrder.dueDate),
                                    HBSKU = hepsiBuradaOrder.sku,
                                    //PackageNote = hepsiBuradaOrder.deliveryNote,
                                    //ProductName = hepsiBuradaOrder.id,
                                    Quantity = Convert.ToDouble(hepsiBuradaOrder.quantity),
                                    ReceiverName = hepsiBuradaOrder.customerName,
                                    SasID = hepsiBuradaOrder.merchantId, //?
                                    //SasItemNr = hepsiBuradaOrder.orderNumber, 
                                    SasNo = hepsiBuradaOrder.orderNumber,
                                    TotalPrice =hepsiBuradaOrder.totalPrice.amount,
                                    UnitPrice = hepsiBuradaOrder.unitPrice.amount,
                                    //adres1 ="" ,
                                    //adres2 = "",
                                    //adres3 = "",
                                    city = hepsiBuradaOrder.shippingAddress.city,
                                    country = hepsiBuradaOrder.shippingAddress.countryCode,
                                    district = hepsiBuradaOrder.shippingAddress.district,
                                    isim = hepsiBuradaOrder.shippingAddress.name,
                                    //isimgonderen = item.addressdetails.isimgonderen,
                                    //telgonderen = item.addressdetails.telgonderen,
                                    town = hepsiBuradaOrder.shippingAddress.town,
                                    TedSKU = hepsiBuradaOrder.sku,
                                    SasItemNr = hepsiBuradaOrder.merchantSKU,
                                    tckno = hepsiBuradaOrder.invoice.turkishIdentityNumber,
                                    email = hepsiBuradaOrder.shippingAddress.email,
                                    vat = hepsiBuradaOrder.vat,
                                    vatRate = hepsiBuradaOrder.vatRate
                                };
                                _util.LogService("Parsing : " + JsonConvert.SerializeObject(hepsiBurada));
                                var pairing = new net_ProductPairing
                                    {EntegrationCode = hepsiBurada.HBSKU, EntegrationName = _entegrationName};
                                pairing = _databaseLayer.GetProductPairing(pairing);

                                if (pairing != null && string.IsNullOrEmpty(hepsiBurada.TedSKU))
                                {
                                    hepsiBurada.TedSKU = pairing.LogoCode;
                                }

                                //var orderStatus = _databaseLayer.GetOrder(_entegrationName, hepsiBurada.SasNo,
                                //    hepsiBurada.SasItemNr);

                                var entegratedOrder = new net_EntegratedOrders();
                                using (var stringwriter = new System.IO.StringWriter())
                                {
                                    var serializer = new XmlSerializer(typeof(net_HepsiBurada));
                                    serializer.Serialize(stringwriter, hepsiBurada);
                                    var returnStr = stringwriter.ToString();
                                    entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                                }

                                entegratedOrder.EntegrationName = _entegrationName;
                                entegratedOrder.OrderDate = hepsiBurada.CreatedDate;

                                entegratedOrder.OrderNo = hepsiBurada.SasNo;

                                entegratedOrder.LineNr = hepsiBurada.SasItemNr.ToString();
                                entegratedOrder.Transfered = 0;
                                var database = new DatabaseLayer();
                                database.saveOrders(entegratedOrder);
                            }
                        }
                    }

                    pageNr++;
                    if (pageNr >= _hepsiBuradaOrderList.pageCount)
                    {
                        nextPage = false;
                    }
                }
                catch (Exception exception)
                {
                    _util.LogService($"[ERROR] HepsiBuradaLayer (getOrders) {exception.Message}");
                }
            }
        }

        private async Task<string> HTTP_GET(int pageNr, DateTime begDate, DateTime endDate)
        {
            var TARGETURL = $"https://oms-external.hepsiburada.com/orders/merchantid/{_entegrationSettings.FirmCode}";
            var startDate = begDate.ToString("yyyy-MM-dd HH:mm");
            var enddate = endDate.AddDays(1).ToString("yyyy-MM-dd HH:mm");


            TARGETURL = TARGETURL +
                        $"?beginDate=" +
                        startDate +
                        "&endDate=" +
                        enddate +
                        "";

            var handler = new HttpClientHandler()
            {
                UseProxy = false,
            };
            var client = new HttpClient(handler);

            var byteArray =
                Encoding.ASCII.GetBytes(_entegrationSettings.UserName + ":" + _entegrationSettings.PassWord); //canlı


            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var response = client.GetAsync(TARGETURL).Result;

            var content = response.Content;

            var result = await content.ReadAsStringAsync();

            _util.LogService($"" + result);


            return result;
        }

        private string CreateClCard(net_HepsiBurada netOrder, string clcode)
        {
            var address = netOrder.Address;
            _util.LogService($"netorder = " + JsonConvert.SerializeObject(netOrder));
            var result = "";
            var arps = Global.application.NewDataObject(DataObjectType.doAccountsRP);
            arps.New();
            arps.DataFields.FieldByName("ACCOUNT_TYPE").Value = 3;
            arps.DataFields.FieldByName("CODE").Value = clcode;
            arps.DataFields.FieldByName("TITLE").Value = netOrder.isim;
            arps.DataFields.FieldByName("ADDRESS1").Value = netOrder.Address;
            arps.DataFields.FieldByName("ADDRESS2").Value = netOrder.adres2;
            arps.DataFields.FieldByName("CITY").Value = netOrder.city;
            arps.DataFields.FieldByName("TOWN").Value = netOrder.district;
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
            arps.DataFields.FieldByName("E_MAIL").Value = netOrder.email;

            arps.DataFields.FieldByName("PERSCOMPANY").Value = 1;
            arps.DataFields.FieldByName("TCKNO").Value = "11111111111";
            
            

            var namelist = splitName(netOrder.isim);
            arps.DataFields.FieldByName("NAME").Value = namelist[0];
            arps.DataFields.FieldByName("SURNAME").Value = namelist[1];

            arps.DataFields.FieldByName("PROFILE_ID").Value = 2;
            arps.DataFields.FieldByName("EARCHIVE_SEND_MODE").Value = 2;
            arps.DataFields.FieldByName("CREATE_WH_FICHE").Value = 1;
            arps.DataFields.FieldByName("INSTEAD_OF_DISPATCH").Value = 1;

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
                    for (var i = 0; i < arps.ValidateErrors.Count; i++)
                    {
                        result += "(" + arps.ValidateErrors[i].ID.ToString() + ") - " + arps.ValidateErrors[i].Error;
                    }
                }
            }

            return result;
        }

        public List<string> splitName(string name)
        {
            string firtname = "";
            string lastname = "";
            int count = name.Count(s => s == ' ');
            var splittedName = name.Split(' ');
            for (int i = 0; i < splittedName.Length; i++)
            {
                if (i == splittedName.Length - 1)
                {
                    lastname = splittedName[i];
                }
                else
                {
                    firtname = firtname + " " + splittedName[i];
                }
            }

            var list = new List<string>();
            list.Add(firtname);
            list.Add(lastname);
            return list;
        }
    }
}