namespace PazarYeri.Models.Common
{
    public class CommonOrderLine
    {
        public int Type { get; set; } = 0;

        public string MasterCode { get; set; } = "";

        public string AuxilCode { get; set; } = "";

        public string AuthCode { get; set; } = "";

        public double Quantity { get; set; } = 0;

        public double Total { get; set; } = 0;

        public double Price { get; set; } = 0;

        public double VatRate { get; set; } = 0;

        public string UnitCode { get; set; } = "";

        public string SalesManCode { get; set; } = "";

        public string ProjectCode { get; set; } = "";

        public int SourceWh { get; set; } = 0;

        public int SourceCostGrp { get; set; } = 0;

        public int Division { get; set; } = 0;

        public int UnitConv1 { get; set; } = 1;

        public int UnitConv2 { get; set; } = 1;

        public int CalcType { get; set; } = 1;
    }
}