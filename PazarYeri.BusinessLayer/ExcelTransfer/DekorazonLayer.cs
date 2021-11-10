using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using UnityObjects;

namespace PazarYeri.BusinessLayer.ExcelTransfer
{
    public class DekorazonLayer
    {
        private readonly DatabaseLayer _databaseLayer;

        private readonly net_LogoTransferSettings _transferSettings;

        private const string _entegrationName = "Dekorazon";

        public DekorazonLayer()
        {
            _databaseLayer = new DatabaseLayer();
            _transferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);
        }

        public bool transferOrders(net_Dekorazon netorder, out string result)
        {
            var transfered = false;
            result = "";

            try
            {
                var OrderRef = _databaseLayer.GetOrderRef(_transferSettings.OrderTransferSpeCode, netorder.SiparisNo, "");
                var order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);
                if (OrderRef == 0)
                {
                    order.New();
                    order.DataFields.FieldByName("NUMBER").Value = "~";

                    //  order.DataFields.FieldByName("DOC_TRACK_NR").Value = netorder.cargoTrackingNumber.ToString();
                    order.DataFields.FieldByName("DATE").Value = netorder.Date_.ToString("dd.MM.yyyy");
                    order.DataFields.FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpCode;
                    order.DataFields.FieldByName("CUST_ORD_NO").Value = netorder.SiparisNo;
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
                    var shipinforef = CreateSevkiyatAdresi(
                        _transferSettings.OrderTransferArpCode,
                        netorder.Phone,
                        netorder.ShipInfoName,
                        netorder.Addr);
                    if (shipinforef > 0)
                    {
                        order.DataFields.FieldByName("SHIPLOC_CODE").Value = netorder.Phone;
                    }

                    var transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;

                    transactions_lines.AppendLine();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = netorder.LogoCode;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value =
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = netorder.Amount;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((netorder.Price / 1.18), 2);
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
                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = netorder.LogoCode;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("AUXIL_CODE").Value =
                        transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = netorder.Amount;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((netorder.Price / 1.18), 2);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                    order.DataFields.FieldByName("AFFECT_RISK").Value = 0;
                }
                order.SetClientInfo();
                //order.ExportToXML("SALES_ORDERS", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Xml\{orfiches[0].SiparisNo}.xml");
                if (order.Post() == true)
                {
                    int logicalref = order.DataFields.FieldByName("INTERNAL_REFERENCE").Value;

                    result = order.DataFields.FieldByName("NUMBER").Value;
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

        private int CreateSevkiyatAdresi(string clcode, string code, string desc, string adr)
        {
            var result = "";
            var dataObject = Global.application.NewDataObject(DataObjectType.doArpShipLic);
            var data = dataObject.DataFields;
            var adresler = adr.Split(' ');
            var adr1 = string.Empty;
            var adr2 = string.Empty;
            var shipInfoRef = 0;
            var afull = true;
            foreach (var a in adresler)
            {
                if ((adr.Length + a.Length < 200) && afull)
                {
                    adr1 += " " + a;
                }
                else
                {
                    adr2 += " " + a;
                    afull = false;
                }
            }

            shipInfoRef = _databaseLayer.getShipInforef(code, clcode);
            if (shipInfoRef > 0)
            {
                dataObject.Read(shipInfoRef);
            }
            else
            {
                dataObject.New();

                // carinin aktif pasif durumu eksik
            }

            dataObject.DataFields.FieldByName("CODE").Value = code;
            dataObject.DataFields.FieldByName("ARP_CODE").Value = clcode;
            dataObject.DataFields.FieldByName("DESCRIPTION").Value = desc;
            dataObject.DataFields.FieldByName("ADDRESS1").Value = adr1;
            dataObject.DataFields.FieldByName("ADDRESS2").Value = adr2;
            dataObject.DataFields.FieldByName("CITY").Value = "";
            dataObject.DataFields.FieldByName("TOWN").Value = "";

            if (dataObject.Post())
            {
                shipInfoRef = dataObject.DataFields.FieldByName("INTERNAL_REFERENCE").Value;

                //MessageBox.Show("Sevkiyat Hesabı Oluşturuldu.", "caption", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (dataObject.ErrorCode != 0)
                {
                    result = $"{dataObject.ErrorCode} {dataObject.ErrorDesc} {dataObject.DBErrorDesc}";
                }
                else if (dataObject.ValidateErrors.Count > 0)
                {
                    result = "XML Hatası:";
                    for (var i = 0; i < dataObject.ValidateErrors.Count; i++)
                    {
                        result += "(" +
                                  dataObject.ValidateErrors[i].ID +
                                  ") - " +
                                  dataObject.ValidateErrors[i].Error;
                    }
                }
            }

            return shipInfoRef;
        }
    }
}