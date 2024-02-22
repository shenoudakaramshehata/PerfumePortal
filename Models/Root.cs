namespace CRM.Models
{
    public class Root
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public string expires_at { get; set; }
        public bool Test { get; set; }
        public bool is_expired { get; set; }
        public string status { get; set; }
        public bool Cancelable { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public string Description { get; set; }
        public Buyer Buyer { get; set; }
        public Product Product { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public Order Order { get; set; }
        public List<object> Captures { get; set; }
        public List<object> Refunds { get; set; }
        public BuyerHistory BuyerHistory { get; set; }
        public List<object> OrderHistory { get; set; }
        public Meta meta { get; set; }
    }
}
