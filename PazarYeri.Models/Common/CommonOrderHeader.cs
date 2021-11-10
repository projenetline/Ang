using System;

namespace PazarYeri.Models.Common
{
    public class CommonOrderHeader
    {
        public string Number { get; set; } = "";

        public DateTime Date { get; set; } = DateTime.Now;

        public string DocNumber { get; set; } = "";

        public string DocTrackNr { get; set; } = "";

        public string ArpCode { get; set; } = "";

        public string CustOrdNo { get; set; } = "";

        public int AffectRisk { get; set; } = 0;

        public string AuxilCode { get; set; } = "";

        public string AuthCode { get; set; } = "";

        public string ArpCodeShpm { get; set; } = "";

        public int OrderStatus { get; set; } = 4;

        public int SourceWh { get; set; } = 0;

        public int SourceCostGrp { get; set; } = 0;

        public int Division { get; set; } = 0;

        public int CurrselTotal { get; set; } = 1;

        public string CargoNo { get; set; } = "";

        public string PaymentCode { get; set; } = "";

        public string SalesmanCode { get; set; } = "";

        public string TradingGrp { get; set; } = "";

        public string ProjectCode { get; set; } = "";

        public CommonShippingAddress ShippingAddress { get; set; }

        public CommonArAps ArAps { get; set; }
        public int EInvoice { get; set; }
        public int EInvoiceProfileId { get; set; }
        public int EArchiveTrSendMod { get; set; }
        public int EArchiveTrInsteadOfDesp { get; set; }
    }
}