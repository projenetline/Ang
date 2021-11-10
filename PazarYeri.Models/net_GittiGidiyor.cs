using System.Collections.Generic;

namespace PazarYeri.Models
{
    public class net_GittiGidiyor
    {
        public int Id { get; set; }
        public string saleCode { get; set; }
        public string status { get; set; }
        public int productId { get; set; }
        public bool productIdSpecified { get; set; }
        public string productBarcode { get; set; }
        public string productTitle { get; set; }
        public string price { get; set; }
        public string cargoPayment { get; set; }
        public string cargoCode { get; set; }
        public int amount { get; set; }
        public bool amountSpecified { get; set; }
        public string endDate { get; set; }
        public BuyerInfo buyerInfo { get; set; }
        public object changeStatus { get; set; }
        public string thumbImageLink { get; set; }
        public string lastActionDate { get; set; }
        public object waitingProcesses { get; set; }
        public InvoiceInfo invoiceInfo { get; set; }
        public int variantId { get; set; }
        public bool variantIdSpecified { get; set; }
        public object moneyDate { get; set; }
        public object cargoApprovementDate { get; set; }
        public object itemId { get; set; }
    }

    public class BuyerInfo
    {
        public string username { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string phone { get; set; }
        public string mobilePhone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string zipCode { get; set; }
        public bool processCountSpecified { get; set; }
        public bool ratePercentageSpecified { get; set; }
    }

    public class InvoiceInfo
    {
        public string fullname { get; set; }
        public string address { get; set; }
        public string district { get; set; }
        public int cityCode { get; set; }
        public bool cityCodeSpecified { get; set; }
        public string zipCode { get; set; }
        public string phoneNumber { get; set; }
        public string taxOffice { get; set; }
        public string taxNumber { get; set; }
        public string companyTitle { get; set; }
        public string tcCertificate { get; set; }
    }
}