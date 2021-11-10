using Newtonsoft.Json;
using PazarYeri.BusinessLayer.Helpers;
using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityObjects;

namespace PazarYeri.BusinessLayer.Trendyol
{
    public class TrendyolLayer
    {
        private cls_Trendyol _trendyolList;
        private readonly DatabaseLayer _databaseLayer;
        private readonly ProjectUtil _util;

        private readonly net_EntegrationSettings _entegrationSettings;
        private readonly net_LogoTransferSettings _transferSettings;

        private const string _entegrationName = "Trendyol";

        public TrendyolLayer()
        {
            _trendyolList = new cls_Trendyol();
            _databaseLayer = new DatabaseLayer();
            _util = new ProjectUtil();
            _entegrationSettings = _databaseLayer.GetEntegrationSettings(_entegrationName);

            _entegrationSettings.UserName = _entegrationSettings.UserName; /*+ " - NetlineBilgiIslem";*/

            _transferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);
        }

        private async Task<string> HTTP_GET(int page, DateTime begdate, DateTime enddate, string status)
        {
            var TARGETURL = "https://api.trendyol.com/sapigw/suppliers/" + _entegrationSettings.FirmCode + "/orders";

            //var TARGETURL = "https://stageapi.trendyol.com/stagesapigw/suppliers/2738/orders";

            var startDate = (long) DateTimeToTimestamp(begdate);
            var endDate = (long) DateTimeToTimestamp(enddate.AddDays(1));

            if (string.IsNullOrEmpty(status))
            {
                TARGETURL = TARGETURL +
                            $"?startDate=" +
                            startDate +
                            "&endDate=" +
                            endDate +
                            "&page=" +
                            page +
                            "&size=200&orderByField=PackageLastModifiedDate&orderByDirection=DESC";
            }
            else
            {
                TARGETURL = TARGETURL +
                            $"?status={status}&startDate=" +
                            startDate +
                            "&endDate=" +
                            endDate +
                            "&page=" +
                            page +
                            "&size=200&orderByField=PackageLastModifiedDate&orderByDirection=DESC";
            }

            var handler = new HttpClientHandler()
            {
                UseProxy = false,
            };
            var client = new HttpClient(handler);

            // var byteArray = Encoding.ASCII.GetBytes("YANwxpWmogkVDt61cp4O:o5FPnaN0KrkdRj7dyAkk"); //test
            var byteArray = Encoding.ASCII.GetBytes(_entegrationSettings.UserName + ":" + _entegrationSettings.PassWord); //canlı

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Add("User-Agent",_entegrationSettings.UserName + " - " + "NetlineBilgiIslem");
            //client.DefaultRequestHeaders.UserAgent = new System.Net.Http.Headers.HttpHeaderValueCollection<ProductInfoHeaderValue>("sada",);

            var response = client.GetAsync(TARGETURL).Result;
            //_entegrationSettings.UserName + " - " + "NetlineBilgiIslem"
            var content = response.Content;

            var result = await content.ReadAsStringAsync();

            return result;
        }

        public void getOrders(DateTime begdate, DateTime enddate, string status)
        {
            var orderList = new List<net_Trendyol>();
            var pageNr = 0;
            var nextPage = true;
            while (nextPage)
            {
                var result = HTTP_GET(pageNr, begdate, enddate, status);
                try
                {
                   
                    _trendyolList = JsonConvert.DeserializeObject<cls_Trendyol>(result.Result);

                    new LogHelper(
                        fileName: $"{begdate:yyyyMMdd} - {enddate:yyyyMMdd}",
                        message: JsonConvert.SerializeObject(result.Result, Formatting.Indented),
                        subFolder: "TRENDYOL");

                    if (_trendyolList != null)
                    {
                        if (_trendyolList.content != null)
                        {
                            foreach (var trendyOrder in _trendyolList.content.OrderBy(x => x.orderDate))
                            {
                                if (trendyOrder.orderNumber == "266565376")
                                {
                                    ;
                                }

                                new LogHelper(
                                    fileName: $"TRENDYOL\\[{trendyOrder.orderNumber}] - {begdate:yyyyMMdd} - {enddate:yyyyMMdd}",
                                    message: JsonConvert.SerializeObject(trendyOrder, Formatting.Indented));

                                foreach (var item in trendyOrder.lines)
                                {
                                    var order = new net_Trendyol()
                                    {
                                        LineNr = 0,
                                        barcode = item.barcode,
                                        cargoTrackingNumber = trendyOrder.cargoTrackingNumber,
                                        city = trendyOrder.invoiceAddress.city,
                                        customerEmail = trendyOrder.customerEmail,
                                        customerId = trendyOrder.customerId,
                                        customerName = trendyOrder.customerFirstName + " " + trendyOrder.customerLastName,
                                        district = trendyOrder.invoiceAddress.district,
                                        merchantSku = item.merchantSku,
                                        LineId = item.id,
                                        orderDate = TimeStampToDateTime(trendyOrder.orderDate),
                                        orderNumber = trendyOrder.orderNumber,
                                        price = item.price,
                                        productCode = item.productCode,
                                        productName = item.productName,
                                        quantity = item.quantity,
                                        sku = item.sku,
                                        totalPrice = trendyOrder.totalPrice,
                                        Id = trendyOrder.id,
                                        _Address = trendyOrder.invoiceAddress,
                                    };

                                    long createdDate = 0;
                                    foreach (var history in trendyOrder.packageHistories)
                                    {
                                        if (createdDate < history.createdDate)
                                        {
                                            order.Status = history.status;
                                        }

                                        createdDate = history.createdDate;
                                    }

                                    var entegratedOrder = new net_EntegratedOrders();
                                    using (var stringwriter = new System.IO.StringWriter())
                                    {
                                        var serializer = new XmlSerializer(typeof(net_Trendyol));
                                        serializer.Serialize(stringwriter, order);
                                        var returnStr = stringwriter.ToString();
                                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                                    }

                                    entegratedOrder.EntegrationName = _entegrationName;
                                    entegratedOrder.OrderDate = order.orderDate;
                                    entegratedOrder.OrderNo = order.orderNumber;
                                    entegratedOrder.LineNr = order.LineId.ToString();
                                    var database = new DatabaseLayer();
                                    database.saveOrders(entegratedOrder);
                                }
                            }
                        }
                    }

                    pageNr++;
                    if (pageNr >= _trendyolList.totalPages)
                    {
                        nextPage = false;
                    }
                }
                catch (Exception exception)
                {
                    _util.LogService($"[ERROR] TrendyolLayer (getOrders) {exception.Message}");
                }
            }
        }

        public DateTime TimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(unixTimeStamp);
        }

        public double DateTimeToTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                    new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;
        }

        public async Task<string> updatePackage(int Id, int lineId, double amount)
        {
            // var TARGETURL = "https://api.trendyol.com/sapigw/suppliers/104743/orders";

            var TARGETURL = "https://stageapi.trendyol.com/stagesapigw/suppliers/2738/shipment-packages/" + Id;

            var handler = new HttpClientHandler()
            {
                UseProxy = false,
            };
            var json = "{\"lines\":[{\"lineId\":" + lineId + " , \"quantity\":" + amount + "}] , \"params\":{}, \"status\": \"Picking\"}";
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient(handler);
            var byteArray = Encoding.ASCII.GetBytes("YANwxpWmogkVDt61cp4O:o5FPnaN0KrkdRj7dyAkk");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var response = client.PutAsync(TARGETURL, httpContent).Result;

            var content = response.Content;

            var result = await content.ReadAsStringAsync();

            return result;
        }

        public bool transferOrder(net_Trendyol netorder, out string result)
        {
            result = "";
            var transfered = false;

            try
            {
                var OrderRef = _databaseLayer.GetOrderRef(
                    _transferSettings.OrderTransferSpeCode,
                    netorder.orderNumber,
                    netorder.cargoTrackingNumber.ToString());

                var clCode = _databaseLayer.GetClCodeByUyeId(netorder._Address.id.ToString());

                if (string.IsNullOrEmpty(clCode))
                {
                    clCode = "120." + _databaseLayer.GetLastClCode("120.");
                    CreateClCard(netorder, clCode);
                }

                var order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);
                if (OrderRef == 0)
                {
                    order.New();
                    order.DataFields.FieldByName("NUMBER").Value = "~";
                    order.DataFields.FieldByName("DOC_TRACK_NR").Value = netorder.cargoTrackingNumber.ToString();
                    order.DataFields.FieldByName("DATE").Value = netorder.orderDate.ToString("dd.MM.yyyy");
                    order.DataFields.FieldByName("ARP_CODE").Value = clCode;
                    order.DataFields.FieldByName("CUST_ORD_NO").Value = netorder.orderNumber;
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
                        order.DataFields.FieldByName("ARP_CODE_SHPM").Value = _transferSettings.OrderTransferArpShippmentCode;
                    }

                    order.DataFields.FieldByName("ORDER_STATUS").Value = 4;
                    order.DataFields.FieldByName("CURRSEL_TOTAL").Value = 1;

                    var transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;

                    transactions_lines.AppendLine();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = _databaseLayer.GetItemCode(netorder.barcode);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value =
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = netorder.quantity;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((netorder.price / 1.18), 2);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                    order.DataFields.FieldByName("AFFECT_RISK").Value = 0;
                }
                else
                {
                    order.Read(OrderRef);
                    var transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;
                    transactions_lines.AppendLine();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = _databaseLayer.GetItemCode(netorder.barcode);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value =
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = netorder.quantity;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((netorder.price / 1.18), 2);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                }

                order.SetClientInfo();

                //order.ExportToXML("SALES_ORDERS", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Xml\{orfiches[0].SiparisNo}.xml");
                if (order.Post() == true)
                {
                    int logicalref = order.DataFields.FieldByName("INTERNAL_REFERENCE").Value;

                    //  InsertShipAddr(logicalref, orfiches[0].ShipAddr);

                    double NetTotal = order.DataFields.FieldByName("TOTAL_NET").Value;

                    CreateArpVoucher(netorder, clCode, NetTotal);
                    transfered = true;
                }
                else
                {
                    if (order.ErrorCode != 0)
                    {
                        result = "DBError(" + order.ErrorCode.ToString() + ")-" + order.ErrorDesc + order.DBErrorDesc;
                    }
                    else if (order.ValidateErrors.Count > 0)
                    {
                        result = "XML ErrorList:";
                        for (var i = 0; i < order.ValidateErrors.Count; i++)
                        {
                            result += "(" + order.ValidateErrors[i].ID.ToString() + ") - " + order.ValidateErrors[i].Error;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return transfered;
        }

        private string CreateClCard(net_Trendyol netOrder, string clcode)
        {
            var address = netOrder._Address;

            var result = "";
            var arps = Global.application.NewDataObject(DataObjectType.doAccountsRP);
            arps.New();
            arps.DataFields.FieldByName("ACCOUNT_TYPE").Value = 3;
            arps.DataFields.FieldByName("CODE").Value = clcode;
            arps.DataFields.FieldByName("TITLE").Value = address.firstName + " " + address.lastName;
            arps.DataFields.FieldByName("ADDRESS1").Value = address.address1;
            arps.DataFields.FieldByName("ADDRESS2").Value = address.address2;
            arps.DataFields.FieldByName("CITY").Value = address.city;
            arps.DataFields.FieldByName("TOWN").Value = address.district;
            arps.DataFields.FieldByName("AUXIL_CODE5").Value = address.id;
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
            arps.DataFields.FieldByName("E_MAIL").Value = netOrder.customerEmail;

            arps.DataFields.FieldByName("PERSCOMPANY").Value = 1;
            arps.DataFields.FieldByName("TCKNO").Value = "11111111111";

            arps.DataFields.FieldByName("NAME").Value = address.firstName;
            arps.DataFields.FieldByName("SURNAME").Value = address.lastName;

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

        private void CreateArpVoucher(net_Trendyol order, string clCode, double netTotal)
        {
            var arpvoucher = Global.application.NewDataObject(UnityObjects.DataObjectType.doARAPVoucher);
            arpvoucher.New();
            arpvoucher.DataFields.FieldByName("NUMBER").Value = "~";
            arpvoucher.DataFields.FieldByName("DATE").Value = order.orderDate;
            arpvoucher.DataFields.FieldByName("TYPE").Value = 5;
            arpvoucher.DataFields.FieldByName("NOTES1").Value = order.orderNumber;
            arpvoucher.DataFields.FieldByName("TOTAL_DEBIT").Value = netTotal;
            arpvoucher.DataFields.FieldByName("TOTAL_CREDIT").Value = netTotal;

            var transactions_lines = arpvoucher.DataFields.FieldByName("TRANSACTIONS").Lines;
            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ARP_CODE").Value = clCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("DOC_NUMBER").Value = order.orderNumber;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = order.orderNumber;
            transactions_lines[transactions_lines.Count - 1].FieldByName("CREDIT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TC_AMOUNT").Value = netTotal;
            transactions_lines[transactions_lines.Count - 1].FieldByName("AFFECT_RISK").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;

            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpShippmentCode;
            transactions_lines[transactions_lines.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("DOC_NUMBER").Value = order.orderNumber;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = order.orderNumber;
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
                    var result = "XML ErrorList:";
                    for (var i = 0; i < arpvoucher.ValidateErrors.Count; i++)
                    {
                        result += "(" + arpvoucher.ValidateErrors[i].ID.ToString() + ") - " + arpvoucher.ValidateErrors[i].Error;
                    }

                    // MessageBox.Show(result);
                }
            }
        }
    }
}