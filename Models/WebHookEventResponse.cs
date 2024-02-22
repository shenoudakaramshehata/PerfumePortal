namespace CRM.Models
{
    public class WebHookEventResponse
    {
        public string? id { get; set; }
        public string? created_at { get; set; }
        public string? expires_at { get; set; }
        public string? closed_at { get; set; }
        public string? status { get; set; }
        public bool? is_test { get; set; }
        public bool? is_expired { get; set; }
        public string? amount { get; set; }
        public string? currency { get; set; }
        public OrderReference? order { get; set; }
        public List<Capture>? captures { get; set; }
        public List<Refund>? refunds { get; set; }
    }
}
