using CRM.Services.TabbyModels;

namespace CRM.Models
{
    public class Payment
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; }
        public bool IsTest { get; set; }
        public Product Product { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public Buyer Buyer { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public orderpayment Order { get; set; }
        public List<object> Captures { get; set; }
        public List<object> Refunds { get; set; }
        public BuyerHistory BuyerHistory { get; set; }
        public List<object> OrderHistory { get; set; }
        public Meta meta { get; set; }
        public bool Cancelable { get; set; }
    }
}
