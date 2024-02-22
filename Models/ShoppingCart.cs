using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CRM.Models
{
    public partial class ShoppingCart
    {
        [Key]
        public int ShoppingCartId { get; set; }
        public string? SessionShoppingCartId { get; set; }
        public int? CustomerId { get; set; }
        public int? ItemId { get; set; }
        public double ItemPrice { get; set; }
        public int ItemQty { get; set; }
        public double ItemTotal { get; set; }
        public double? Deliverycost { get; set; }
        [JsonIgnore]
        [ForeignKey("CustomerId")]
        public virtual CustomerN CustomerN { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}