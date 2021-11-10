using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using UnityObjects;

namespace PazarYeri.BusinessLayer.ExcelTransfer
{
    public class EvideaLayer
    {
        private readonly DatabaseLayer _databaseLayer;

        private readonly net_LogoTransferSettings _transferSettings;

        private const string _entegrationName = "Evidea";

        public EvideaLayer()
        {
            _databaseLayer = new DatabaseLayer();
            ;
            _transferSettings = _databaseLayer.GetLogoTransferSettings(_entegrationName);
        }

        public bool transferOrder(net_Evidea evideaOrder, out string result)
        {
            result = "";
            var transfered = false;

            try
            {
                var OrderRef = _databaseLayer.GetOrderRef(_transferSettings.OrderTransferSpeCode, evideaOrder.OrderNumber, "");
                var OrderlineCount = 0;
                if (OrderRef == 0)
                {
                    OrderlineCount = _databaseLayer.GetOrderLineCount(OrderRef);
                }

                var order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);
                if (OrderRef == 0 || OrderlineCount == 21 || OrderlineCount == 41 || OrderlineCount == 61)
                {
                    order.New();
                    order.DataFields.FieldByName("NUMBER").Value = "~";
                    order.DataFields.FieldByName("DOC_TRACK_NR").Value = "";
                    order.DataFields.FieldByName("DATE").Value = evideaOrder.Date_.ToString("dd.MM.yyyy");
                    order.DataFields.FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpCode;

                    //  order.DataFields.FieldByName("CUST_ORD_NO").Value = evideaOrder.OrderCode;
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

                    var itemCode = _databaseLayer.GetItemCodeByProducerCode(evideaOrder.ProductCode);
                    if (string.IsNullOrEmpty(itemCode))
                    {
                        itemCode = evideaOrder.ProductCode;
                    }

                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = itemCode;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = evideaOrder.Quantity;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((evideaOrder.Price / 1.18), 2);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                }
                else
                {
                    order.Read(OrderRef);
                    var transactions_lines = order.DataFields.FieldByName("TRANSACTIONS").Lines;
                    transactions_lines.AppendLine();
                    transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = evideaOrder.ProductCode;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = evideaOrder.Quantity;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("PRICE").Value = Math.Round((evideaOrder.Price / 1.18), 2);
                    transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                }

                order.DataFields.FieldByName("AFFECT_RISK").Value = 0;
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
    }
}