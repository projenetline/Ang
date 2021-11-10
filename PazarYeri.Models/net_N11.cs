using System;

namespace PazarYeri.Models
{
    public class net_N11
    {
        public DateTime createDate { get; set; } = DateTime.Today;
        public string orderNumber { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public long Id { get; set; } = 0;
        public long id { get; set; } = 0;
        public int LineNr { get; set; } = 0;
        

        public long userId { get; set; } = 0;
        public string fullName { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;      
        public string city { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string neighborhood { get; set; } = string.Empty;
        public string tcId { get; set; } = string.Empty;
        public string postalCode { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string gsm { get; set; } = string.Empty;

        public long lineId { get; set; } = 0;
        public decimal quantity { get; set; } = 0;
        public decimal commission { get; set; } = 0;
        public decimal price { get; set; } = 0;
        public string productName { get; set; } = string.Empty;
        public string productSellerCode { get; set; } = string.Empty;


        public decimal n11Discount { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
        public decimal vadeFarki { get; set; } = 0;
        public decimal totalServicePrice { get; set; } = 0;
    }
}
