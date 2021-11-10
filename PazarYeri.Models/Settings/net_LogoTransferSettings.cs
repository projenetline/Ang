namespace PazarYeri.Models.Settings
{
    public class net_LogoTransferSettings
    {
        public int Id { get; set; } = 0;
        public string EntegrationName { get; set; } = "";

        public string ClientTransferPrefixCode { get; set; } = "";
        public string ClientTransferSpeCode1 { get; set; } = "";
        public string ClientTransferSpeCode2 { get; set; } = "";
        public string ClientTransferSpeCode3 { get; set; } = "";
        public string ClientTransferSpeCode4 { get; set; } = "";
        public string ClientTransferSpeCode5 { get; set; } = "";

        public string ClientTransferAccountCode { get; set; } = "";
        public string ClientTransferPaymentCode { get; set; } = "";
        public string ClientTransferTradingGroup { get; set; } = "";
        public string ClientTransferProjectCode { get; set; } = "";
        public string ClientTransferAuthCode { get; set; } = "";

        public string OrderTransferDiscountCouponCode { get; set; } = "";
        public string OrderTransferServiceChargeCode { get; set; } = "";
        public string OrderTransferLateChargeCode { get; set; } = "";
        public string OrderTransferSalesManCode { get; set; } = "";
        public string OrderTransferDoCode { get; set; } = "";
        public string OrderTransferProjectCode { get; set; } = "";
        public string OrderTransferTradingGroup { get; set; } = "";
        public string OrderTransferSpeCode { get; set; } = "";
        public string OrderTransferPaymentCode { get; set; } = "";
        public string OrderTransferArpShippmentCode { get; set; } = "";
        public string OrderTransferArpCode { get; set; } = "";
        public string OrderTransferAuthCode { get; set; } = "";

        public int OrderTransferWareHouseNr { get; set; } = 0;
        public int OrderTransferDivisionNr { get; set; } = 0;
        public int OrderTransferUnitPriceRoundingNumberOfDigits { get; set; } = 2;

        public int OrderTransferStatus { get; set; } = 4;

        public bool OrderTransferRetransferTransferedOrder { get; set; }
        public bool OrderTransferAddRowsIfOrder { get; set; }
        public bool OrderTransferGroupByOrderNumber { get; set; }
        public bool OrderTransferTransferToShippingAddress { get; set; }

        public string TransferFicheProjectCode { get; set; } = "";
    }
}