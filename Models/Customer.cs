
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CRM.Models
{
    public partial class Customer
    {
        //public Customer()
        //{
        //    Order = new HashSet<Order>();
        //}

        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RegisterDate { get; set; }
        //[InverseProperty("Customer")]
        //public virtual ICollection<Order> Order { get; set; }
    }
}