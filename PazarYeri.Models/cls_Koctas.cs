using System.Collections.Generic;
using System.Xml.Serialization;

namespace PazarYeri.Models
{
  
    [XmlRoot(ElementName = "ProcessPurchaseOrder", Namespace = "http://www.openapplications.org/oagis/9")]
    public class cls_Koctas
    {
        [XmlElement(ElementName = "ApplicationArea", Namespace = "http://www.openapplications.org/oagis/9")]
        public ApplicationArea ApplicationArea { get; set; }
        [XmlElement(ElementName = "DataArea", Namespace = "http://www.openapplications.org/oagis/9")]
        public DataArea DataArea { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "releaseID")]
        public string ReleaseID { get; set; }
    }
    [XmlRoot(ElementName = "ApplicationArea", Namespace = "http://www.openapplications.org/oagis/9")]
    public class ApplicationArea
    {
        [XmlElement(ElementName = "CreationDateTime", Namespace = "http://www.openapplications.org/oagis/9")]
        public string CreationDateTime { get; set; }
    }

    [XmlRoot(ElementName = "DocumentID", Namespace = "http://www.openapplications.org/oagis/9")]
    public class DocumentID
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "PartyIDs", Namespace = "http://www.openapplications.org/oagis/9")]
    public class PartyIDs
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "SupplierParty", Namespace = "http://www.openapplications.org/oagis/9")]
    public class SupplierParty
    {
        [XmlElement(ElementName = "PartyIDs", Namespace = "http://www.openapplications.org/oagis/9")]
        public PartyIDs PartyIDs { get; set; }
    }

    [XmlRoot(ElementName = "AddressLine", Namespace = "http://www.openapplications.org/oagis/9")]
    public class AddressLine
    {
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
    }

    [XmlRoot(ElementName = "Address", Namespace = "http://www.openapplications.org/oagis/9")]
    public class Address
    {
        [XmlElement(ElementName = "AddressLine", Namespace = "http://www.openapplications.org/oagis/9")]
        public List<AddressLine> AddressLine { get; set; }
        [XmlElement(ElementName = "CityName", Namespace = "http://www.openapplications.org/oagis/9")]
        public string CityName { get; set; }
        [XmlElement(ElementName = "CountrySubDivisionCode", Namespace = "http://www.openapplications.org/oagis/9")]
        public string CountrySubDivisionCode { get; set; }
        [XmlElement(ElementName = "CountryCode", Namespace = "http://www.openapplications.org/oagis/9")]
        public string CountryCode { get; set; }
        [XmlElement(ElementName = "PostalCode", Namespace = "http://www.openapplications.org/oagis/9")]
        public string PostalCode { get; set; }
    }

    [XmlRoot(ElementName = "Location", Namespace = "http://www.openapplications.org/oagis/9")]
    public class Location
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Directions", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Directions { get; set; }
        [XmlElement(ElementName = "Address", Namespace = "http://www.openapplications.org/oagis/9")]
        public Address Address { get; set; }
    }

    [XmlRoot(ElementName = "ShipToParty", Namespace = "http://www.openapplications.org/oagis/9")]
    public class ShipToParty
    {
        [XmlElement(ElementName = "Location", Namespace = "http://www.openapplications.org/oagis/9")]
        public Location Location { get; set; }
    }

    [XmlRoot(ElementName = "PurchaseOrderHeader", Namespace = "http://www.openapplications.org/oagis/9")]
    public class PurchaseOrderHeader
    {
        [XmlElement(ElementName = "DocumentID", Namespace = "http://www.openapplications.org/oagis/9")]
        public DocumentID DocumentID { get; set; }
        [XmlElement(ElementName = "Status", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Status { get; set; }
        [XmlElement(ElementName = "SupplierParty", Namespace = "http://www.openapplications.org/oagis/9")]
        public SupplierParty SupplierParty { get; set; }
        [XmlElement(ElementName = "ShipToParty", Namespace = "http://www.openapplications.org/oagis/9")]
        public ShipToParty ShipToParty { get; set; }
        [XmlElement(ElementName = "RequestedShipDateTime", Namespace = "http://www.openapplications.org/oagis/9")]
        public string RequestedShipDateTime { get; set; }
        [XmlElement(ElementName = "ReleaseNumber", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ReleaseNumber { get; set; }
        [XmlElement(ElementName = "Buyer", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Buyer { get; set; }
    }

    [XmlRoot(ElementName = "ItemID", Namespace = "http://www.openapplications.org/oagis/9")]
    public class ItemID
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "CustomerItemID", Namespace = "http://www.openapplications.org/oagis/9")]
    public class CustomerItemID
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "ManufacturerItemID", Namespace = "http://www.openapplications.org/oagis/9")]
    public class ManufacturerItemID
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "SupplierItemID", Namespace = "http://www.openapplications.org/oagis/9")]
    public class SupplierItemID
    {
        [XmlElement(ElementName = "ID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "Item", Namespace = "http://www.openapplications.org/oagis/9")]
    public class Item
    {
        [XmlElement(ElementName = "ItemID", Namespace = "http://www.openapplications.org/oagis/9")]
        public ItemID ItemID { get; set; }
        [XmlElement(ElementName = "CustomerItemID", Namespace = "http://www.openapplications.org/oagis/9")]
        public CustomerItemID CustomerItemID { get; set; }
        [XmlElement(ElementName = "ManufacturerItemID", Namespace = "http://www.openapplications.org/oagis/9")]
        public ManufacturerItemID ManufacturerItemID { get; set; }
        [XmlElement(ElementName = "SupplierItemID", Namespace = "http://www.openapplications.org/oagis/9")]
        public SupplierItemID SupplierItemID { get; set; }
        [XmlElement(ElementName = "UPCID", Namespace = "http://www.openapplications.org/oagis/9")]
        public string UPCID { get; set; }
        [XmlElement(ElementName = "Description", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Field1", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Field1 { get; set; }
        [XmlElement(ElementName = "Field2", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Field2 { get; set; }
        [XmlElement(ElementName = "BudgetCode", Namespace = "http://www.openapplications.org/oagis/9")]
        public string BudgetCode { get; set; }
        [XmlElement(ElementName = "Note", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Note { get; set; }
        [XmlElement(ElementName = "Barcode", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Barcode { get; set; }
    }

    [XmlRoot(ElementName = "Amount", Namespace = "http://www.openapplications.org/oagis/9")]
    public class Amount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "UnitPrice", Namespace = "http://www.openapplications.org/oagis/9")]
    public class UnitPrice
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.openapplications.org/oagis/9")]
        public Amount Amount { get; set; }
        [XmlElement(ElementName = "PerQuantity", Namespace = "http://www.openapplications.org/oagis/9")]
        public string PerQuantity { get; set; }
        [XmlElement(ElementName = "Code", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "PurchaseOrderLine", Namespace = "http://www.openapplications.org/oagis/9")]
    public class PurchaseOrderLine
    {
        [XmlElement(ElementName = "LineNumber", Namespace = "http://www.openapplications.org/oagis/9")]
        public string LineNumber { get; set; }
        [XmlElement(ElementName = "Status", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Status { get; set; }
        [XmlElement(ElementName = "Item", Namespace = "http://www.openapplications.org/oagis/9")]
        public Item Item { get; set; }
        [XmlElement(ElementName = "Quantity", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Quantity { get; set; }
        [XmlElement(ElementName = "UnitPrice", Namespace = "http://www.openapplications.org/oagis/9")]
        public UnitPrice UnitPrice { get; set; }
    }

    [XmlRoot(ElementName = "TotalAmount", Namespace = "http://www.openapplications.org/oagis/9")]
    public class TotalAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PurchaseOrderSummary", Namespace = "http://www.openapplications.org/oagis/9")]
    public class PurchaseOrderSummary
    {
        [XmlElement(ElementName = "TotalItemCount", Namespace = "http://www.openapplications.org/oagis/9")]
        public string TotalItemCount { get; set; }
        [XmlElement(ElementName = "TotalAmount", Namespace = "http://www.openapplications.org/oagis/9")]
        public TotalAmount TotalAmount { get; set; }
    }

    [XmlRoot(ElementName = "PurchaseOrder", Namespace = "http://www.openapplications.org/oagis/9")]
    public class PurchaseOrder
    {
        [XmlElement(ElementName = "PurchaseOrderHeader", Namespace = "http://www.openapplications.org/oagis/9")]
        public PurchaseOrderHeader PurchaseOrderHeader { get; set; }
        [XmlElement(ElementName = "PurchaseOrderLine", Namespace = "http://www.openapplications.org/oagis/9")]
        public List<PurchaseOrderLine> PurchaseOrderLine { get; set; }
        [XmlElement(ElementName = "PurchaseOrderSummary", Namespace = "http://www.openapplications.org/oagis/9")]
        public PurchaseOrderSummary PurchaseOrderSummary { get; set; }
    }

    [XmlRoot(ElementName = "DataArea", Namespace = "http://www.openapplications.org/oagis/9")]
    public class DataArea
    {
        [XmlElement(ElementName = "Process", Namespace = "http://www.openapplications.org/oagis/9")]
        public string Process { get; set; }
        [XmlElement(ElementName = "PurchaseOrder", Namespace = "http://www.openapplications.org/oagis/9")]
        public PurchaseOrder PurchaseOrder { get; set; }
    }

  

}



