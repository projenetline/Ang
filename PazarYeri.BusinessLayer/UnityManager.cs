using System;
using System.Text;
using PazarYeri.Models.Common;
using PazarYeri.BusinessLayer.Helpers;
using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models.Settings;
using UnityObjects;

namespace PazarYeri.BusinessLayer
{
    public class UnityManager
    {
        private readonly net_LogoTransferSettings _transferSettings;
        private readonly DatabaseLayer _databaseLayer;
        private readonly ProjectUtil _util;

        private readonly string _entegrationName;

        public UnityManager(string entegrationName)
        {
            _databaseLayer = new DatabaseLayer();
            _entegrationName = entegrationName;
            _util = new ProjectUtil();

            _transferSettings = _databaseLayer.GetLogoTransferSettings(entegrationName);
        }

        private void PostClient(CommonArAps arps, out string result, out int cardRef)
        {
            var data = Global.application.NewDataObject(DataObjectType.doAccountsRP);

            var clientref = _databaseLayer.GetClientrefByCode(arps.Code);

            if (clientref > 0)
                data.Read(clientref);
            else
                data.New();

            data.DataFields.FieldByName("ACCOUNT_TYPE").Value = arps.AccountType;
            data.DataFields.FieldByName("CODE").Value = arps.Code;
            data.DataFields.FieldByName("GL_CODE").Value = arps.GlCode;
            data.DataFields.FieldByName("TITLE").Value = arps.Title;
            data.DataFields.FieldByName("TITLE2").Value = arps.Title;

            data.DataFields.FieldByName("ADDRESS1").Value = arps.Address1;
            data.DataFields.FieldByName("ADDRESS2").Value = arps.Address2;
            data.DataFields.FieldByName("CITY").Value = arps.City;
            data.DataFields.FieldByName("TOWN").Value = arps.Town;
            data.DataFields.FieldByName("COUNTRY").Value = arps.Country;
            data.DataFields.FieldByName("COUNTRY_CODE").Value = arps.CountryCode;

            data.DataFields.FieldByName("AUXIL_CODE").Value = arps.AuxilCode;
            data.DataFields.FieldByName("AUXIL_CODE2").Value = arps.AuxilCode2;
            data.DataFields.FieldByName("AUXIL_CODE3").Value = arps.AuxilCode3;
            data.DataFields.FieldByName("AUXIL_CODE4").Value = arps.AuxilCode4;
            data.DataFields.FieldByName("AUXIL_CODE5").Value = arps.AuxilCode5;
            data.DataFields.FieldByName("TRADING_GRP").Value = arps.TradingGrp;
            data.DataFields.FieldByName("PAYMENT_CODE").Value = arps.PaymentCode;
            data.DataFields.FieldByName("PROJECT_CODE").Value = arps.ProjectCode;
            data.DataFields.FieldByName("AUTH_CODE").Value = arps.AuthCode;

            data.DataFields.FieldByName("TELEPHONE1").Value = arps.Telephone1;
            data.DataFields.FieldByName("TELEPHONE1").Value = arps.Telephone2;
            data.DataFields.FieldByName("E_MAIL").Value = arps.EMail;
            data.DataFields.FieldByName("CONTACT").Value = arps.Contact;
            data.DataFields.FieldByName("DSP_SEND_EMAIL").Value = arps.EMail;
            data.DataFields.FieldByName("ORD_SEND_EMAIL").Value = arps.EMail;
            data.DataFields.FieldByName("INV_SEND_EMAIL").Value = arps.EMail;
            data.DataFields.FieldByName("REMINDER_EMAIL").Value = arps.EMail;
            data.DataFields.FieldByName("PROFILE_ID").Value = arps.ProfilId;
            data.DataFields.FieldByName("EARCHIVE_SEND_MODE").Value = arps.EArchiveSendMode;
            data.DataFields.FieldByName("ITR_SEND_MAIL_ADR").Value = arps.EMail;
            data.DataFields.FieldByName("FBS_SEND_EMAILADDR").Value = arps.EMail;
            data.DataFields.FieldByName("FBA_SEND_EMAILADDR").Value = arps.EMail;
            data.DataFields.FieldByName("OFF_SEND_MAIL_ADDR").Value = arps.EMail;
            data.DataFields.FieldByName("PERSCOMPANY").Value = arps.PersCompany;
            data.DataFields.FieldByName("TCKNO").Value = arps.TCKNO;
            data.DataFields.FieldByName("NAME").Value = arps.Name;
            data.DataFields.FieldByName("SURNAME").Value = arps.SurName;
            data.DataFields.FieldByName("INSTEAD_OF_DISPATCH").Value = arps.InsteadOfDispatch;

            data.DataFields.FieldByName("COLLATRLRISK_TYPE").Value = 1;
            data.DataFields.FieldByName("INVOICE_PRNT_CNT").Value = 1;
            data.DataFields.FieldByName("CREATE_WH_FICHE").Value = 1;
            data.DataFields.FieldByName("CORRESP_LANG").Value = 1;
            data.DataFields.FieldByName("CL_ORD_FREQ").Value = 1;
            data.DataFields.FieldByName("RISK_TYPE1").Value = 1;
            data.DataFields.FieldByName("RISK_TYPE2").Value = 1;
            data.DataFields.FieldByName("RISK_TYPE3").Value = 1;
            data.DataFields.FieldByName("PURCHBRWS").Value = 1;
            data.DataFields.FieldByName("SALESBRWS").Value = 1;
            data.DataFields.FieldByName("IMPBRWS").Value = 1;
            data.DataFields.FieldByName("EXPBRWS").Value = 1;
            data.DataFields.FieldByName("FINBRWS").Value = 1;

            ExportXml(data, "AR_AP", "Clients", $"{arps.EMail}");

            PostData(data, out result, out cardRef);

            if (cardRef > 0)
            {
                _databaseLayer.InsertOrUpdateClientPairing(_entegrationName, arps.Code, arps.EMail);
            }
        }

        private void CreateShippingAddress(CommonShippingAddress shippingAddress)
        {
            if (string.IsNullOrEmpty(shippingAddress.Code))
            {
                return;
            }

            var data = Global.application.NewDataObject(DataObjectType.doArpShipLic);

            var shipInfoRef = _databaseLayer.getShipInforef(shippingAddress.Code, shippingAddress.ArpCode);

            if (shipInfoRef > 0)
                data.Read(shipInfoRef);
            else
                data.New();

            data.DataFields.FieldByName("CODE").Value = shippingAddress.Code;
            data.DataFields.FieldByName("ARP_CODE").Value = shippingAddress.ArpCode;
            data.DataFields.FieldByName("DESCRIPTION").Value = shippingAddress.Description;
            data.DataFields.FieldByName("ADDRESS1").Value = shippingAddress.Address1;
            data.DataFields.FieldByName("ADDRESS2").Value = shippingAddress.Address2;
            data.DataFields.FieldByName("CITY").Value = shippingAddress.City;
            data.DataFields.FieldByName("TOWN").Value = "";

            ExportXml(
                data,
                "ARP_SHIPMENT_LOCATIONS",
                "ShippingAddresses",
                $"{shippingAddress.ArpCode}-{shippingAddress.Code}");

            PostData(data, out var result, out var cardRef);

            if (cardRef <= 0)
            {
                _util.LogService(
                    $"[ERROR] Sevkiyat Adresi Aktarım Hatası" +
                    $"{Environment.NewLine}" +
                    $"{_entegrationName}{Environment.NewLine}" +
                    $"UnityManager (CreateShippingAddress)" +
                    $"{Environment.NewLine}" +
                    $"{result}");
            }
        }

        public void PostOrder(CommonOrder order, out string result, out int ficheRef)
        {
            result = "";
            ficheRef = 0;

            try
            {
                if (string.IsNullOrEmpty(_transferSettings.OrderTransferArpCode))
                {
                    PostClient(order.Header.ArAps, out result, out var cardRef);

                    if (cardRef == 0)
                    {
                        return;
                    }
                }

                CreateShippingAddress(order.Header.ShippingAddress);

                var orderRef =
                    _databaseLayer.GetOrderRef(_transferSettings.OrderTransferSpeCode, order.Header.CustOrdNo, order.Header.CargoNo);

                var data = Global.application.NewDataObject(DataObjectType.doSalesOrderSlip);

                if (orderRef > 0 && _transferSettings.OrderTransferRetransferTransferedOrder)
                    data.New();

                else if (orderRef > 0 &&
                         !_transferSettings.OrderTransferRetransferTransferedOrder &&
                         _transferSettings.OrderTransferAddRowsIfOrder)
                {
                    data.Read(orderRef);
                }
                else if (orderRef > 0 && !_transferSettings.OrderTransferAddRowsIfOrder)
                {
                    _util.LogService(
                        $"Sipariş Satırları Aktarılmayacak Olarak İşaretli." +
                        $"{Environment.NewLine}" +
                        $"[{_entegrationName}]-(PostOrder)");

                    ficheRef = orderRef;
                    result = $"Sipariş Satırları Aktarılmayacak Olarak İşaretli." +
                             $"{Environment.NewLine}" +
                             $"[{_entegrationName}]-(PostOrder)";

                    return;
                }
                else
                    data.New();

                data.DataFields.FieldByName("DATE").Value = order.Header.Date.ToString("dd.MM.yyyy");
                data.DataFields.FieldByName("NUMBER").Value = order.Header.Number;
                data.DataFields.FieldByName("CUST_ORD_NO").Value = order.Header.CustOrdNo;
                data.DataFields.FieldByName("DOC_TRACK_NR").Value = order.Header.DocTrackNr;
                data.DataFields.FieldByName("ARP_CODE").Value = order.Header.ArpCode;
                data.DataFields.FieldByName("ARP_CODE_SHPM").Value = order.Header.ArpCodeShpm;
                data.DataFields.FieldByName("AFFECT_RISK").Value = order.Header.AffectRisk;
                data.DataFields.FieldByName("AUXIL_CODE").Value = order.Header.AuxilCode;
                data.DataFields.FieldByName("ORDER_STATUS").Value = order.Header.OrderStatus;
                data.DataFields.FieldByName("CURRSEL_TOTAL").Value = order.Header.CurrselTotal;
                data.DataFields.FieldByName("DIVISION").Value = order.Header.Division;
                data.DataFields.FieldByName("SOURCE_WH").Value = order.Header.SourceWh;
                data.DataFields.FieldByName("SOURCE_COST_GRP").Value = order.Header.SourceCostGrp;
                data.DataFields.FieldByName("DOC_NUMBER").Value = order.Header.DocNumber;
                data.DataFields.FieldByName("DOC_TRACK_NR").Value = order.Header.DocTrackNr;
                data.DataFields.FieldByName("SALESMAN_CODE").Value = order.Header.SalesmanCode;
                data.DataFields.FieldByName("TRADING_GRP").Value = order.Header.TradingGrp;
                data.DataFields.FieldByName("PROJECT_CODE").Value = order.Header.ProjectCode;
                data.DataFields.FieldByName("PAYMENT_CODE").Value = order.Header.PaymentCode;
                data.DataFields.FieldByName("SHIPLOC_CODE").Value = order.Header.ShippingAddress.Code;
                data.DataFields.FieldByName("EINVOICE").Value = order.Header.EInvoice;
                data.DataFields.FieldByName("EINVOICE_PROFILEID").Value = order.Header.EInvoiceProfileId;
                data.DataFields.FieldByName("EARCHIVEDETR_SENDMOD").Value = order.Header.EArchiveTrSendMod;
                data.DataFields.FieldByName("EARCHIVEDETR_INSTEADOFDESP").Value = order.Header.EArchiveTrInsteadOfDesp;

                var transactions = data.DataFields.FieldByName("TRANSACTIONS").Lines;

                foreach (var line in order.Lines)
                {
                    transactions.AppendLine();
                    transactions[transactions.Count - 1].FieldByName("TYPE").Value = line.Type;
                    transactions[transactions.Count - 1].FieldByName("MASTER_CODE").Value = line.MasterCode;
                    transactions[transactions.Count - 1].FieldByName("AUXIL_CODE").Value = line.AuxilCode;
                    transactions[transactions.Count - 1].FieldByName("CALC_TYPE").Value = line.CalcType;
                    transactions[transactions.Count - 1].FieldByName("QUANTITY").Value = line.Quantity;
                    transactions[transactions.Count - 1].FieldByName("TOTAL").Value = line.Total;

                    transactions[transactions.Count - 1].FieldByName("PRICE").Value = Math.Round(
                        (line.Price),
                        _transferSettings.OrderTransferUnitPriceRoundingNumberOfDigits);

                    transactions[transactions.Count - 1].FieldByName("VAT_RATE").Value = line.VatRate;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CODE").Value = line.UnitCode;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV1").Value = line.UnitConv1;
                    transactions[transactions.Count - 1].FieldByName("UNIT_CONV2").Value = line.UnitConv1;
                    transactions[transactions.Count - 1].FieldByName("SOURCE_WH").Value = line.SourceWh;
                    transactions[transactions.Count - 1].FieldByName("SOURCE_COST_GRP").Value = line.SourceCostGrp;
                    transactions[transactions.Count - 1].FieldByName("DIVISION").Value = line.Division;
                    transactions[transactions.Count - 1].FieldByName("SALESMAN_CODE").Value = line.SalesManCode;
                    transactions[transactions.Count - 1].FieldByName("PROJECT_CODE").Value = line.ProjectCode;
                }

                data.ReCalculate();
                data.FillAccCodes();
                data.SetClientInfo();

                ExportXml(data, "SALES_ORDERS", "Orders", $"OrderNo #{order.Header.CustOrdNo}");

                PostData(data, out result, out ficheRef);

                if (ficheRef > 0 && _transferSettings.OrderTransferTransferToShippingAddress)
                {
                    double total = _databaseLayer.GetOrderAmount(ficheRef);

                    PostTransferFiche(
                        order,
                        total,
                        out var transferResult,
                        out var transferFicheRef);

                    if (transferFicheRef <= 0)
                    {
                        result = $"Virman Hatası: {transferResult}";
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                _util.LogService(
                    $"Sipariş Aktarım Hatası." +
                    $"{Environment.NewLine}" +
                    $"[{_entegrationName}]- UnityManager (PostOrder)" +
                    $"{Environment.NewLine}{ex.Message}");
            }
        }

        private void PostTransferFiche(CommonOrder order, double total, out string result, out int ficheRef)
        {
            var data = Global.application.NewDataObject(DataObjectType.doARAPVoucher);

            data.New();
            data.DataFields.FieldByName("NUMBER").Value = "~";
            data.DataFields.FieldByName("DATE").Value = order.Header.Date;
            data.DataFields.FieldByName("TYPE").Value = 5;
            data.DataFields.FieldByName("NOTES1").Value = order.Header.CustOrdNo;
            data.DataFields.FieldByName("TOTAL_DEBIT").Value = total;
            data.DataFields.FieldByName("TOTAL_CREDIT").Value = total;
            data.DataFields.FieldByName("AFFECT_RISK").Value = 0;

            var transactions = data.DataFields.FieldByName("TRANSACTIONS").Lines;

            transactions.AppendLine();
            transactions[transactions.Count - 1].FieldByName("ARP_CODE").Value = order.Header.ArAps.Code;
            transactions[transactions.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions[transactions.Count - 1].FieldByName("DOC_NUMBER").Value = order.Header.CustOrdNo;
            transactions[transactions.Count - 1].FieldByName("DESCRIPTION").Value = order.Header.CustOrdNo;
            transactions[transactions.Count - 1].FieldByName("CREDIT").Value = total;
            transactions[transactions.Count - 1].FieldByName("TC_AMOUNT").Value = total;
            transactions[transactions.Count - 1].FieldByName("AFFECT_RISK").Value = 1;
            transactions[transactions.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;

            transactions.AppendLine();
            transactions[transactions.Count - 1].FieldByName("ARP_CODE").Value = _transferSettings.OrderTransferArpShippmentCode;
            transactions[transactions.Count - 1].FieldByName("TRANNO").Value = "~";
            transactions[transactions.Count - 1].FieldByName("DOC_NUMBER").Value = order.Header.CustOrdNo;
            transactions[transactions.Count - 1].FieldByName("DESCRIPTION").Value = order.Header.CustOrdNo;
            transactions[transactions.Count - 1].FieldByName("DEBIT").Value = total;
            transactions[transactions.Count - 1].FieldByName("TC_AMOUNT").Value = total;
            transactions[transactions.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;
            
            ExportXml(data, "ARP_VOUCHERS", "TransferFiches", $"OrderNo #{order.Header.CustOrdNo}");

            PostData(data, out result, out ficheRef);
        }

        private void ExportXml(Data data, string rootKey, string folder, string fileName)
        {
            var path = DirectoryHelper.GetAppDirectoryPath();

            path = $@"{path}\XML\{_entegrationName}\{folder}\{DateTime.Now.Year:0000}{DateTime.Now.Month:00}";

            DirectoryHelper.CreateFolderIfNotExists(path);

            data.ExportToXML(rootKey, $@"{path}\{fileName}.xml");
        }

        private void PostData(Data data, out string result, out int ficheRef)
        {
            if (data.Post())
            {
                var logicalRef = data.DataFields.FieldByName("INTERNAL_REFERENCE").Value;

                // ReSharper disable once NotAccessedVariable
                var resultNumber = "";

                var dataObjectType = data.DataObject;

                switch (data.DataObject)
                {
                    case DataObjectType.doAccountsRP:
                    case DataObjectType.doArpShipLic:
                        resultNumber = data.DataFields.FieldByName("CODE").Value;
                        break;
                    case DataObjectType.doSalesOrderSlip:
                    case DataObjectType.doARAPVoucher:
                        resultNumber = data.DataFields.FieldByName("NUMBER").Value;
                        break;
                }

                result = "Aktarım Başarılı";
                ficheRef = logicalRef;

                data = null;

                data = Global.application.NewDataObject(dataObjectType);

                if (data.Read(logicalRef))
                {
                    data.Post();
                }

                return;
            }

            var sb = new StringBuilder();

            if (data.ErrorCode != 0)
            {
                sb.Append(
                    $"[{_entegrationName}] (Sipariş Aktarım Hatası)" +
                    $"[#{data.ErrorCode}] " +
                    $"{data.ErrorDesc} {data.DBErrorDesc}");
            }

            var validateErrors = data.ValidateErrors;

            for (var i = 0; i < validateErrors.Count; i++)
            {
                sb.Append($"[#{validateErrors[i].ID}] - {validateErrors[i].Error} ");
            }

            result = sb.ToString();
            ficheRef = 0;
        }
    }
}