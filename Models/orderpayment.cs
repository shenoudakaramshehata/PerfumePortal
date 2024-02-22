namespace CRM.Models
{
    public class orderpayment
    {
        public string ReferenceId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TaxAmount { get; set; }
        public string ShippingAmount { get; set; }
        public string DiscountAmount { get; set; }
        public List<object> Items { get; set; }
    }
}
