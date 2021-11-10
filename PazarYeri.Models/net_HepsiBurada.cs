using System;

namespace PazarYeri.Models
{
    public class net_HepsiBurada
    {
        public int Id { get; set; }
        public string SasItemNr { get; set; } = string.Empty;
        public double Quantity { get; set; } = 0;
        public string SasID { get; set; } = string.Empty;
        public string SasNo { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string HBSKU { get; set; } = string.Empty;
        public string TedSKU { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string PackageNote { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Today;
        public DateTime DeliveryDate { get; set; } = DateTime.Today;
        public double UnitPrice { get; set; } = 0;
        public double TotalPrice { get; set; } = 0;
        public double vat { get; set; } = 0;
        public int vatRate { get; set; } = 0;

        
        public string isim { get; set; } = string.Empty;
        public string adres1 { get; set; } = string.Empty;
        public string adres2 { get; set; } = string.Empty;
        public string adres3 { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string town { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string telgonderen { get; set; } = string.Empty;
        public string isimgonderen { get; set; } = string.Empty;
        public string tckno { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string ResultMsg { get; set; }
        public int LineNr { get; set; } = 0;

    }

  

}
