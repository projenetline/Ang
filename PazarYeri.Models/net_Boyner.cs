using System;

namespace PazarYeri.Models
{
    public class net_Boyner
    {
        public int LineNr { get; set; } = 0;
        public string OrderCode { get; set; } = string.Empty;        
        public int OrderLineCode { get; set; } = 0;
        public string BoynerProductCode { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public double Quantity { get; set; } = 0;    
        public double Price { get; set; } = 0;
        public string CargoNo { get; set; } = string.Empty;
        public DateTime Date_ { get; set; } = DateTime.Now;
        public string Statu { get; set; } = string.Empty;
        public string LogoFicheNo { get; set; } = string.Empty;
    }
}
