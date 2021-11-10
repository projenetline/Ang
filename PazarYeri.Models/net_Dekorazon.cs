using System;

namespace PazarYeri.Models
{
    public class net_Dekorazon
    {
        public string SiparisNo { get; set; } = string.Empty;
        public int LineNr { get; set; } = 0;
        public long OrderLineNr { get; set; } = 0;
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string LogoCode { get; set; } = string.Empty;
        public string Addr { get; set; } = string.Empty;
        public string EMail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ShipInfoName { get; set; } = string.Empty;
        public double Amount { get; set; } = 0;
        public double Price { get; set; } = 0;
        public DateTime Date_ { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now;
        public string Statu { get; set; } = string.Empty;
        public string LogoFicheNo { get; set; } = string.Empty;
    }
}
