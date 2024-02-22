using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CRM.Models
{
    public class CouponType
    {
        [Key]
        public int CouponTypeId { get; set; }
        public string? CouponTypeAR { get; set; }
        public string? CouponTypeEN { get; set; }
        [JsonIgnore]
        public ICollection<Coupon> Coupons { get; set; } =new HashSet<Coupon>();
    }
}