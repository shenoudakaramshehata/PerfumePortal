using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class PaymentMehod
    {
        [Key]
        public int PaymentMethodId { get; set; }
        public string? PaymentMethodAR { get; set; }
        public string? PaymentMethodEN { get; set; }
        public string? PaymentMethodPic { get; set; }
    }
}
