namespace CRM.Services.TabbyModels
{

    #nullable disable

    public class Attachment
    {
        public string body { get; set; }
        public string content_type { get; set; }
    }

    public class Buyer
    {
        public string phone { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string dob { get; set; }
    }

    public class BuyerHistory
    {
        public DateTime registered_since { get; set; }
        public int loyalty_level { get; set; }
        public int wishlist_count { get; set; }
        public bool is_social_networks_connected { get; set; }
        public bool is_phone_number_verified { get; set; }
        public bool is_email_verified { get; set; }
    }

    public class Item
    {
        public string title { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public string unit_price { get; set; }
        public string discount_amount { get; set; }
        public string reference_id { get; set; }
        public string image_url { get; set; }
        public string product_url { get; set; }
        public string gender { get; set; }
        public string category { get; set; }
        public string color { get; set; }
        public string product_material { get; set; }
        public string size_type { get; set; }
        public string size { get; set; }
        public string brand { get; set; }
        public int ordered { get; set; }
        public int captured { get; set; }
        public int shipped { get; set; }
        public int refunded { get; set; }
    }

    public class MerchantUrls
    {
        public string success { get; set; }
        public string cancel { get; set; }
        public string failure { get; set; }
    }

    public class Meta
    {
        public object order_id { get; set; }
        public object customer { get; set; }
    }

    public class TabbyOrder
    {
        public string tax_amount { get; set; }
        public string shipping_amount { get; set; }
        public string discount_amount { get; set; }
        public DateTime updated_at { get; set; }
        public string reference_id { get; set; }
        public List<Item> items { get; set; }
    }

    public class OrderHistory
    {
        public DateTime purchased_at { get; set; }
        public string amount { get; set; }
        public string payment_method { get; set; }
        public string status { get; set; }
        public Buyer buyer { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public List<Item> items { get; set; }
    }

    public class Payment
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public Buyer buyer { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public TabbyOrder order { get; set; }
        public BuyerHistory buyer_history { get; set; }
        public List<OrderHistory> order_history { get; set; }
        public Meta meta { get; set; }
        public Attachment attachment { get; set; }
    }

    public class TabbyCheckRequestMessage
    {
        public Payment payment { get; set; }
        public string lang { get; set; }
        public string merchant_code { get; set; }
        public MerchantUrls merchant_urls { get; set; }
        public bool create_token { get; set; }
        public object token { get; set; }
    }

    public class ShippingAddress
    {
        public string city { get; set; }
        public string address { get; set; }
        public string zip { get; set; }
    }
}
