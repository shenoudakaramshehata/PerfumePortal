namespace CRM.Models
{
    public class OrdersCaptures
    {
        public int OrdersCapturesId { get; set; }
        public double amount { get; set; }
        public string captureId { get; set; }
        public string Createdate { get; set; }
        public int OrderId { get; set; }
        public Order order { get; set; }
        public string? currency { get; set; }
        public string? PaymentId { get; set; } 
    }
}
