using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityObjects;

namespace PazarYeri.BusinessLayer.Amazon
{
    public class AmazonLayer
    {
        private readonly net_LogoTransferSettings _transferSettings;

        private const string _entegrationName = "Amazon";

        public AmazonLayer()
        {
            _transferSettings = new DatabaseLayer().GetLogoTransferSettings(_entegrationName);
        }

        public bool TransferOrder(List<net_Amazon> netorder, out string result)
        {
            var transfered = false;
            result = "";

            var header = netorder.First();

            try
            {
                var order = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);

                order.New();
                order.DataFields.FieldByName("NUMBER").Value = "~";

                //  order.DataFields.FieldByName("DOC_TRACK_NR").Value = netorder.cargoTrackingNumber.ToString();
                order.DataFields.FieldByName("DATE").Value = header.OrderDate.ToString("dd.MM.yyyy");
                order.DataFields.FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpCode;
                order.DataFields.FieldByName("CUST_ORD_NO").Value = header.PONumber;

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

                var transactions = order.DataFields.FieldByName("TRANSACTIONS").Lines;

                foreach (var line in netorder)
                {
                    if (string.IsNullOrEmpty(line.LogoCode))
                        continue;

                    transactions.AppendLine();
                    transactions[transactions.Count - 1].FieldByName("TYPE").Value = 0;
                    transactions[transactions.Count - 1].FieldByName("MASTER_CODE").Value = line.LogoCode;
                    transactions[transactions.Count - 1].FieldByName("AUXIL_CODE").Value = "";
                    transactions[transactions.Count - 1].FieldByName("QUANTITY").Value = line.Quantity;
                    transactions[transactions.Count - 1].FieldByName("PRICE").Value = line.Price;
                    transactions[transactions.Count - 1].FieldByName("VAT_RATE").Value = 18;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
                }

                order.DataFields.FieldByName("AFFECT_RISK").Value = 0;
                order.SetClientInfo();
                order.ExportToXML(
                    "SALES_ORDERS",
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Xml\Amazon_{header.PONumber}.xml");

                if (order.Post() == true)
                {
                    int logicalref = order.DataFields.FieldByName("INTERNAL_REFERENCE").Value;
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