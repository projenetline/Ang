using System;

namespace PazarYeri.Models
{
    public class net_EntegratedOrders
    {
        public long Id { get; set; } = 0;
        public string LineNr { get; set; } = "";
        public string EntegrationName { get; set; } = "";
        public string OrderNo { get; set; } = "";
        public DateTime OrderDate { get; set; } = DateTime.Today;
        public byte[] OrderXml { get; set; } = new byte[] { };
        public int Transfered { get; set; } = 0;
        public string ResultMsg { get; set; } = "";
        public string LogoFicheNo { get; set; } = "";

    }
}
