using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace CRM.Models
{
    public class CustomerN
    {

        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Is Required"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Not Valid")]
        public string Email { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RegisterDate { get; set; }

        [Required(ErrorMessage = "Is Required"), RegularExpression("^[0-9]+$", ErrorMessage = " Accept Number Only")]
        public string? Phone { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Order { get; set; }

    }
}
