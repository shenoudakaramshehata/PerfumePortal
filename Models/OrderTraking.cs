using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CRM.Models
{
    public class OrderTraking
    {
        public int OrderTrakingId { get; set; }
        public int OrderStatusId { get; set; }
        public int OrderId { get; set; }
        public DateTime ActionDate { get; set; }
        public string Remarks { get; set; } = "";
        [JsonIgnore]
        public virtual OrderStatus OrderStatus { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}
