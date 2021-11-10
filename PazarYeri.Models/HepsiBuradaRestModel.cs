using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PazarYeri.Models
{
    //[JsonProperty(PropertyName = "totalCount")]

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class HepsiBuradaRestModel
    {
        public int totalCount { get; set; }
        public long limit { get; set; }
        public int offset { get; set; }
        public int pageCount { get; set; }
        public List<item> items { get; set; }
    }

    [JsonObject(Description = "Item")]
    public class item
    {
        public DateTime dueDate { get; set; }
        public DateTime lastStatusUpdateDate { get; set; }
        public string id { get; set; }
        public string sku { get; set; }
        public string orderId { get; set; }
        public string orderNumber { get; set; }
        public DateTime orderDate { get; set; }
        public int quantity { get; set; }
        public string merchantId { get; set; }
        public TotalPrice totalPrice { get; set; }
        public Unitprice unitPrice { get; set; }
        public HbDiscount hbDiscount { get; set; }
        public double vat { get; set; }
        public int vatRate { get; set; }
        public string customerName { get; set; }
        public string status { get; set; }
        public ShippingAddress shippingAddress { get; set; }
        public Invoice invoice { get; set; }
        public string sapNumber { get; set; }
        public int dispatchTime { get; set; }
        public Commission commission { get; set; }
        public int paymentTermInDays { get; set; }
        public int commissionType { get; set; }
        public CargoCompanyModel cargoCompanyModel { get; set; }
        public string cargoCompany { get; set; }
        public string customizedText01 { get; set; }
        public string customizedText02 { get; set; }
        public string customizedText03 { get; set; }
        public string customizedText04 { get; set; }
        public string customizedTextX { get; set; }
        public bool isCustomized { get; set; }
        public bool canCreatePackage { get; set; }
        public bool isCancellable { get; set; }
        public bool isCancellableByHbAdmin { get; set; }
        public string deliveryType { get; set; }
        public int deliveryOptionId { get; set; }
        public string merchantSKU { get; set; }
        public PurchasePrice purchasePrice { get; set; }
        public DeliveryNote deliveryNote { get; set; }
    }

    public class TotalPrice
    {
        public string currency { get; set; }
        public double amount { get; set; }
    }

    [JsonObject(Description = "UnitPrice")]
    public class Unitprice
    {
        public string currency { get; set; }
        public double amount { get; set; }
    }

    public class HbDiscount
    {
        public TotalPrice totalPrice { get; set; }
        public Unitprice unitPrice { get; set; }
    }

    public class ShippingAddress
    {
        public string addressId { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string countryCode { get; set; }
        public string phoneNumber { get; set; }
        public string alternatePhoneNumber { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string town { get; set; }
    }

    [JsonObject(Description = "Address")]
    public class Addres
    {
        public string addressId { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string countryCode { get; set; }
        public string phoneNumber { get; set; }
        public string alternatePhoneNumber { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string town { get; set; }
    }

    public class Invoice
    {
        public string turkishIdentityNumber { get; set; }
        public string taxNumber { get; set; }
        public Addres address { get; set; }
    }

    public class Commission
    {
        public string currency { get; set; }
        public double amount { get; set; }
    }

    public class CargoCompanyModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public string logoUrl { get; set; }
        public string trackingUrl { get; set; }
    }

    public class PurchasePrice
    {
        public string currency { get; set; }
        public int amount { get; set; }
    }

    public class DeliveryNote
    {
    }

   

    


}
