using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace CRM.Models
{
    public class ItemPriceN
    {
        [Key]

        public int ItemPriceId { get; set; }
        [Required(ErrorMessage ="Please Select an Item")]
        public int ItemId { get; set; }
        [Required(ErrorMessage = "Please Select a Country")]
        public int? CountryId { get; set; }
        [Required(ErrorMessage = "Please Enter A price")]
        public double? Price { get; set; }

        public double? BeforePrice { get; set; }
        public double? ShippingPrice { get; set; }

        [ForeignKey("ItemId")]
        [InverseProperty("ItemPriceNs")]
        [JsonIgnore]
        public virtual Item Item { get; set; }
        [JsonIgnore]
        public virtual Country Country { get; set; }
        
        

    }
}
