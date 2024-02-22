namespace CRM.Models
{
    public class ItemView
    {
        public int? OrderId { get; set; }
        public int? ItemId { get; set; }
        public double ItemPrice { get; set; }
        public int ItemQuantity { get; set; }
        public double Total { get; set; }
        public string ItemTitle { get; set; }
    }
}
