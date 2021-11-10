using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PazarYeri.Models
{
   public class net_AltinciCadde
    {

        public int LineNr { get; set; } = 0;
        public string OrderNumber { get; set; } = string.Empty;
        public long OrderLineNr { get; set; } = 0;
        public string SasNo { get; set; } = "";
        public string AltinciCaddeCode { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public double Quantity { get; set; } = 0;
        public double Price { get; set; } = 0;
        public DateTime Date_ { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now;        
        public string Statu { get; set; } = string.Empty;
        public string LogoFicheNo { get; set; } = string.Empty;
    }
}

