using System.Xml.Serialization;

namespace PazarYeri.Models
{


    [XmlRoot(ElementName = "ORDERS")]
    public class net_KoctasOrderList
    {
        [XmlElement(ElementName = "SAT_ORDERS")]
        public SAT_ORDERS SAT_ORDERS { get; set; }
    }

    [XmlRoot(ElementName = "SAT_ORDERS")]
    public class SAT_ORDERS
    {
        [XmlElement(ElementName = "ORDER_DATE")]
        public string ORDER_DATE { get; set; }
        [XmlElement(ElementName = "ORDER_NO")]
        public string ORDER_NO { get; set; }
        [XmlElement(ElementName = "PART_NO")]
        public string PART_NO { get; set; }
        [XmlElement(ElementName = "BARCODE")]
        public string BARCODE { get; set; }
        [XmlElement(ElementName = "PART_DESC")]
        public string PART_DESC { get; set; }
        [XmlElement(ElementName = "QTY")]
        public string QTY { get; set; }
        [XmlElement(ElementName = "UNIT")]
        public string UNIT { get; set; }
        [XmlElement(ElementName = "STATUS")]
        public string STATUS { get; set; }
        [XmlElement(ElementName = "PLANSTATUS")]
        public string PLANSTATUS { get; set; }
        [XmlElement(ElementName = "SHIPTOID")]
        public string SHIPTOID { get; set; }
        [XmlElement(ElementName = "SHIPTONAME")]
        public string SHIPTONAME { get; set; }
        [XmlElement(ElementName = "ORDER_ISSUE_DATE")]
        public string ORDER_ISSUE_DATE { get; set; }
    }



}
