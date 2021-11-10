using System;
using System.Collections.Generic;

namespace PazarYeri.Models
{
    public class net_Trendyol
    {
        public int Id { get; set; }
        public int LineNr { get; set; }
        public string orderNumber { get; set; }
        public double totalPrice { get; set; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public int customerId { get; set; }
        public long cargoTrackingNumber { get; set; }
        public DateTime orderDate { get; set; }
        public int LineId { get; set; }
        public int quantity { get; set; }
        public string merchantSku { get; set; }
        public string productName { get; set; }
        public int productCode { get; set; }
        public double price { get; set; }
        public string sku { get; set; }
        public string barcode { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string Status { get; set; }
        public string ResultMsg { get; set; }   
        public InvoiceAddress _Address { get; set; } = new InvoiceAddress();
    }
  
}
