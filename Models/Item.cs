#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CRM.Models
{
    public partial class Item
    {
        public Item()
        {
            ItemImageNavigation = new HashSet<ItemImage>();
            ItemPriceNs = new HashSet<ItemPriceN>();
            OrderItem = new HashSet<OrderItem>();
        }

        [Key]
        public int ItemId { get; set; }
        [Required(ErrorMessage = "Please fill the field")]
        public string ItemTitleAr { get; set; }
        [Required(ErrorMessage = "Please fill the field")]
        public string ItemTitleEn { get; set; }
        public string ItemImage { get; set; }
        public string ItemDescriptionAr { get; set; }
        public string ItemDescriptionEn { get; set; }
        public bool IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public int? Stock { get; set; }
        public bool OutOfStock { get; set; }
        public int CategoryId { get; set; }
        //public int? CountryId { get; set; }
        public double? Weight { get; set; }
        //[JsonIgnore]
        //public virtual Country? Country { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("Item")]
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [InverseProperty("Item")]
        [JsonIgnore]
        public virtual ICollection<ItemImage> ItemImageNavigation { get; set; }

        [InverseProperty("Item")]
        [JsonIgnore]
        public virtual ICollection<ItemPriceN> ItemPriceNs { get; set; }

        [InverseProperty("Item")]
        [JsonIgnore]
        public virtual ICollection<OrderItem> OrderItem { get; set; }
    }
}