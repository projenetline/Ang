using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PazarYeri.BusinessLayer.tr.com.koctas.vsrm;
using PazarYeri.Models.Common;
using UnityObjects;

namespace PazarYeri.BusinessLayer.Koctas
{
    public class KoctasLayer
    {
        private readonly DatabaseLayer _databaseLayer;
        private readonly ProjectUtil _util;
        private readonly net_EntegrationSettings _entegrationSettings;
        private readonly net_LogoTransferSettings _transferSettings;
        private readonly vediFileTransferWS _vediFileTransfer;

        private readonly string _entegrationName = "Koçtaş";

        public KoctasLayer()
        {
            _databaseLayer = new DatabaseLayer();
            _util = new ProjectUtil();
            _entegrationSettings = _databaseLayer.GetEntegrationSettings(_entegrationName);
            _transferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);
            _vediFileTransfer = new vediFileTransferWS();
            //{ Url = "https://vsrm.koctas.com.tr:8443/vediFileTransferWS/services/vediFileTransferWS/" };
        }

        public bool OrderTransfer(List<net_Koctas> koctasLines, out string result)
        {
            var order = MapKoctas(koctasLines);

            var unityManager = new UnityManager(_entegrationName);

            unityManager.PostOrder(order, out result, out var ficheRef);

            return ficheRef > 0;
        }

        private CommonOrder MapKoctas(List<net_Koctas> koctasLines)
        {
            var commonOrder = new CommonOrder
            {
                Header = new CommonOrderHeader()
                {
                    Number = "~",
                    Date = koctasLines.First().Date_,
                    DocTrackNr = "",
                    ArpCode = _transferSettings.OrderTransferArpCode,
                    CustOrdNo = koctasLines.First().SiparisNo,
                    AffectRisk = 0,
                    CurrselTotal = 1,
                    CargoNo = "",
                    SourceWh = _transferSettings.OrderTransferWareHouseNr,
                    SourceCostGrp = _transferSettings.OrderTransferWareHouseNr,
                    DocNumber = _transferSettings.OrderTransferDoCode,
                    PaymentCode = _transferSettings.OrderTransferPaymentCode,
                    Division = _transferSettings.OrderTransferDivisionNr,
                    SalesmanCode = _transferSettings.OrderTransferSalesManCode,
                    TradingGrp = _transferSettings.OrderTransferTradingGroup,
                    AuxilCode = _transferSettings.OrderTransferSpeCode,
                    ArpCodeShpm = _transferSettings.OrderTransferArpShippmentCode,
                    OrderStatus = _transferSettings.OrderTransferStatus,
                    EArchiveTrInsteadOfDesp = 0,
                    EArchiveTrSendMod = 0,
                    EInvoice = 1,
                    EInvoiceProfileId = 0
                }
            };

            var addresses = koctasLines.First().Addr.Split(' ');
            var address1 = string.Empty;
            var address2 = string.Empty;
            var afull = true;

            foreach (var a in addresses)
            {
                if (koctasLines.First().Addr.Length + a.Length < 200 && afull)
                {
                    address1 += " " + a;
                }
                else
                {
                    address2 += " " + a;
                    afull = false;
                }
            }

            var commonShippingAddress = new CommonShippingAddress
            {
                Code = koctasLines.First().Phone,
                Description = koctasLines.First().ShipInfoName,
                ArpCode = _transferSettings.OrderTransferArpCode,
                Address1 = address1,
                Address2 = address2,
                City = "",
                Town = ""
            };

            commonOrder.Header.ShippingAddress = commonShippingAddress;

            commonOrder.Header.ArAps = new CommonArAps()
            {
                AccountType = 3,
                Code = _transferSettings.OrderTransferArpCode,
                Title = "",
                City = "",
                Town = "",
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
                Telephone1 = "",
                Telephone2 = "",
                EMail = "",
                Contact = "",
                Address1 = "",
                Address2 = "",
                PersCompany = 1,
                TCKNO = "",
                Name = "",
                SurName = "",
                InsteadOfDispatch = 0
            };


            var lines = new List<CommonOrderLine>();

            foreach (var koctasLine in koctasLines)
            {
                lines.Add(
                    new CommonOrderLine()
                    {
                        Type = 0,
                        MasterCode = koctasLine.LogoCode,
                        AuxilCode = _transferSettings.OrderTransferSpeCode,
                        Quantity = koctasLine.Amount,
                        Price = koctasLine.Price,
                        VatRate = _databaseLayer.GetTaxRate(koctasLine.LogoCode),
                        UnitCode = "ADET",
                        UnitConv1 = 1,
                        UnitConv2 = 1
                    });
            }

            commonOrder.Lines = lines;

            return commonOrder;
        }

        //public bool transferOrders(List<net_Koctas> orderLines, out string result)
        //{
        //    var transfered = false;
        //    result = "";

        //    var netorder = orderLines.First();

        //    try
        //    {
        //        var OrderRef =
        //            _databaseLayer.GetOrderRef(_transferSettings.OrderTransferSpeCode, netorder.SiparisNo, "");
        //        var order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);

        //        if (OrderRef == 0)
        //        {
        //            order.New();
        //            order.DataFields.FieldByName("NUMBER").Value = "~";

        //            //  order.DataFields.FieldByName("DOC_TRACK_NR").Value = netorder.cargoTrackingNumber.ToString();
        //            order.DataFields.FieldByName("DATE").Value = netorder.Date_.ToString("dd.MM.yyyy");
        //            order.DataFields.FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpCode;
        //            order.DataFields.FieldByName("CUST_ORD_NO").Value = netorder.SiparisNo;
        //            order.DataFields.FieldByName("AFFECT_RISK").Value = 0;

        //            if (!string.IsNullOrEmpty(_transferSettings.OrderTransferSpeCode))
        //            {
        //                order.DataFields.FieldByName("AUXIL_CODE").Value = _transferSettings.OrderTransferSpeCode;
        //            }

        //            if (!string.IsNullOrEmpty(_transferSettings.OrderTransferArpShippmentCode))
        //            {
        //                order.DataFields.FieldByName("ARP_CODE_SHPM").Value =
        //                    _transferSettings.OrderTransferArpShippmentCode;
        //            }

        //            order.DataFields.FieldByName("ORDER_STATUS").Value = 4;
        //            order.DataFields.FieldByName("CURRSEL_TOTAL").Value = 1;

        //            var shipinforef = CreateSevkiyatAdresi(
        //                _transferSettings.OrderTransferArpCode,
        //                netorder.Phone,
        //                netorder.ShipInfoName,
        //                netorder.Addr);
        //            if (shipinforef > 0)
        //            {
        //                order.DataFields.FieldByName("SHIPLOC_CODE").Value = netorder.Phone;
        //            }

        //            var transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;

        //            foreach (var line in orderLines)
        //            {
        //                transactions_lines.AppendLine();
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value =
        //                    line.LogoCode;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value = "";
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = line.Amount;

        //                //transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((line.Price * 1.18), 2);
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value =
        //                    Math.Round((line.Price), 2);
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
        //            }
        //        }
        //        else
        //        {
        //            order.Read(OrderRef);

        //            var transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;

        //            foreach (var line in orderLines)
        //            {
        //                transactions_lines.AppendLine();
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value =
        //                    line.LogoCode;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value = "";
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = line.Amount;

        //                //transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((line.Price * 1.18), 2);
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value =
        //                    Math.Round((line.Price), 2);
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
        //                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
        //            }

        //            order.DataFields.FieldByName("AFFECT_RISK").Value = 0;
        //        }

        //        //order.ExportToXML("SALES_ORDERS", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Xml\{orfiches[0].SiparisNo}.xml");
        //        if (order.Post() == true)
        //        {
        //            int logicalref = order.DataFields.FieldByName("INTERNAL_REFERENCE").Value;
        //            transfered = true;
        //        }
        //        else
        //        {
        //            if (order.ErrorCode != 0)
        //            {
        //                result = "DBError(" + order.ErrorCode.ToString() + ")-" + order.ErrorDesc + order.DBErrorDesc;
        //            }
        //            else if (order.ValidateErrors.Count > 0)
        //            {
        //                result = "XML ErrorList:";
        //                for (var i = 0; i < order.ValidateErrors.Count; i++)
        //                {
        //                    result += "(" + order.ValidateErrors[i].ID.ToString() + ") - " +
        //                              order.ValidateErrors[i].Error;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //    }

        //    return transfered;
        //}

        //private int CreateSevkiyatAdresi(string clcode, string code, string desc, string adr)
        //{
        //    var result = "";
        //    var dataObject = Global.application.NewDataObject(DataObjectType.doArpShipLic);
        //    var data = dataObject.DataFields;
        //    var adresler = adr.Split(' ');
        //    var adr1 = string.Empty;
        //    var adr2 = string.Empty;
        //    var shipInfoRef = 0;
        //    var afull = true;

        //    foreach (var a in adresler)
        //    {
        //        if ((adr.Length + a.Length < 200) && afull)
        //        {
        //            adr1 += " " + a;
        //        }
        //        else
        //        {
        //            adr2 += " " + a;
        //            afull = false;
        //        }
        //    }

        //    shipInfoRef = _databaseLayer.getShipInforef(code, clcode);
        //    if (shipInfoRef > 0)
        //    {
        //        dataObject.Read(shipInfoRef);
        //    }
        //    else
        //    {
        //        dataObject.New();

        //        // carinin aktif pasif durumu eksik
        //    }

        //    dataObject.DataFields.FieldByName("CODE").Value = code;
        //    dataObject.DataFields.FieldByName("ARP_CODE").Value = clcode;
        //    dataObject.DataFields.FieldByName("DESCRIPTION").Value = desc;
        //    dataObject.DataFields.FieldByName("ADDRESS1").Value = adr1;
        //    dataObject.DataFields.FieldByName("ADDRESS2").Value = adr2;
        //    dataObject.DataFields.FieldByName("CITY").Value = "";
        //    dataObject.DataFields.FieldByName("TOWN").Value = "";

        //    if (dataObject.Post())
        //    {
        //        shipInfoRef = dataObject.DataFields.FieldByName("INTERNAL_REFERENCE").Value;

        //        //MessageBox.Show("Sevkiyat Hesabı Oluşturuldu.", "caption", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    else
        //    {
        //        if (dataObject.ErrorCode != 0)
        //        {
        //            result = $"{dataObject.ErrorCode} {dataObject.ErrorDesc} {dataObject.DBErrorDesc}";
        //        }
        //        else if (dataObject.ValidateErrors.Count > 0)
        //        {
        //            result = "XML Hatası:";
        //            for (var i = 0; i < dataObject.ValidateErrors.Count; i++)
        //            {
        //                result += "(" +
        //                          dataObject.ValidateErrors[i].ID +
        //                          ") - " +
        //                          dataObject.ValidateErrors[i].Error;
        //            }
        //        }
        //    }

        //    return shipInfoRef;
        //}

        public void getOrders()
        {
            try
            {
                var database = new DatabaseLayer();

                var vendorId = _entegrationSettings.FirmCode.Split(';')[0];
                var shipToId = _entegrationSettings.FirmCode.Split(';')[1];
                _vediFileTransfer.Credentials = new NetworkCredential("Itg_Ws", "Itg_2014");

                var _vendorDetail = new VendorDetail()
                {
                    username = _entegrationSettings.UserName,
                    password = _entegrationSettings.PassWord,
                    shipToId = shipToId,
                    vendorId = vendorId,
                };

                var exportDetail = new OrderExportDetail()
                {
                    isMultiFile = "y",
                    orderNumbers = "",
                    preferredExtension = "xml",
                    type = "14",
                    zippedExport = "y",
                };

                var orderData = new OrderExportType[] { };

                var json = JsonConvert.SerializeObject(_vediFileTransfer);
                var json1 = JsonConvert.SerializeObject(_vendorDetail);
                var json2 = JsonConvert.SerializeObject(exportDetail);

                var wsResult = "";
                try
                {
                    orderData = _vediFileTransfer.getNewPurchaseOrder(_vendorDetail, exportDetail);
                }
                catch (Exception ex)
                {
                    wsResult = ex.Message;
                }

                var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                           @"\TempFile";

                _util.LogService($"Koçtaş wsResult: {wsResult}");

                if ("There is no data." != wsResult)
                {
                    var diFile = new DirectoryInfo(path);

                    if (!diFile.Exists)
                        Directory.CreateDirectory(path);

                    var fileName = orderData[0].fileName;

                    _util.LogService(fileName);

                    File.WriteAllBytes(path + @"\" + fileName, orderData[0].content);
                }

                var fileContents = "";

                _util.LogService("path : " + path);
                var d = new DirectoryInfo(path); //Assuming Test is your Folder
                var Files = d.GetFiles("*.zip"); //Getting Text files
                foreach (var file in Files)
                {
                    var OrderListserializer = new XmlSerializer(typeof(cls_Koctas));
                    var apcZipFile = System.IO.Compression.ZipFile.Open(
                        path + @"\" + file.Name,
                        System.IO.Compression.ZipArchiveMode.Read);
                    foreach (var entry in apcZipFile.Entries)
                    {
                        if (entry.Name.ToUpper().EndsWith(".XML"))
                        {
                            using (var sr = new StreamReader(entry.Open()))
                            {
                                //read the contents into a string
                                fileContents = sr.ReadToEnd();

                                using (TextReader reader = new StringReader(fileContents))
                                {
                                    var processPurchase = (cls_Koctas) OrderListserializer.Deserialize(reader);

                                    foreach (var PurchaseOrderLine in processPurchase.DataArea.PurchaseOrder
                                        .PurchaseOrderLine)
                                    {
                                        var koctas_serializer = new XmlSerializer(typeof(net_Koctas));
                                        var koctas = new net_Koctas()
                                        {
                                            Addr = PurchaseOrderLine.Item.BudgetCode,
                                            Amount = Convert.ToDouble(PurchaseOrderLine.Quantity.Replace('.', ',')),
                                            Date_ =
                                                Convert.ToDateTime(processPurchase.ApplicationArea.CreationDateTime),
                                            ItemCode = PurchaseOrderLine.Item.CustomerItemID.ID,
                                            Phone = PurchaseOrderLine.Item.Field2,
                                            Price = Convert.ToDouble(
                                                PurchaseOrderLine.UnitPrice.Amount.Text.Replace('.', ',')),
                                            SiparisNo = processPurchase.DataArea.PurchaseOrder.PurchaseOrderHeader
                                                .DocumentID.ID,
                                            ItemName = PurchaseOrderLine.Item.Description,
                                            Barcode = PurchaseOrderLine.Item.Barcode,
                                            OrderLineNr = Convert.ToInt32(PurchaseOrderLine.LineNumber),
                                            ShipInfoName = PurchaseOrderLine.Item.Field1.Replace(",", " "),
                                            LogoCode = PurchaseOrderLine.Item.SupplierItemID.ID,
                                        };
                                        var pairing = new net_ProductPairing();
                                        pairing.EntegrationCode = koctas.ItemCode;
                                        pairing.EntegrationName = "Koçtaş";
                                        pairing = _databaseLayer.GetProductPairing(pairing);

                                        if (pairing != null && string.IsNullOrEmpty(koctas.LogoCode))
                                        {
                                            koctas.LogoCode = pairing.LogoCode;
                                            koctas.Barcode = pairing.Barcode;
                                        }
                                        else
                                        {
                                            var itemcode = _databaseLayer.GetItemCode(koctas.LogoCode);

                                            if (!string.IsNullOrEmpty(itemcode))
                                            {
                                                koctas.LogoCode = itemcode;
                                            }
                                        }

                                        var entegratedOrder = new net_EntegratedOrders();
                                        using (var stringwriter = new StringWriter())
                                        {
                                            koctas_serializer.Serialize(stringwriter, koctas);
                                            var returnStr = stringwriter.ToString();
                                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                                        }

                                        entegratedOrder.EntegrationName = "Koçtaş";
                                        entegratedOrder.OrderDate =
                                            Convert.ToDateTime(processPurchase.ApplicationArea.CreationDateTime);
                                        entegratedOrder.OrderNo = processPurchase.DataArea.PurchaseOrder
                                            .PurchaseOrderHeader.DocumentID.ID;
                                        entegratedOrder.LineNr = PurchaseOrderLine.LineNumber;
                                        entegratedOrder.Transfered = 0;
                                        database.saveKoctasOrders(entegratedOrder);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _util.LogService("Hata : " + ex.Message + " Konum : " + ex.StackTrace);
            }
        }
    }
}