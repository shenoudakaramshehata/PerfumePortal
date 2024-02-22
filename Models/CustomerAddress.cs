using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CRM.Models
{
    
    public class CustomerAddress
    {
        [Key]
        public int CustomerAddressId { get; set; }
        public int? CustomerId { get; set; }
        public int? CountryId { get; set; }
        [StringLength(50)]
        public string CityName { get; set; }
        [StringLength(50)]
        public string AreaName { get; set; }
        public string Address { get; set; }
        [StringLength(50)]
        public string BuildingNo { get; set; }
        [Required(ErrorMessage = "Zip Code is required.")]
        [RegularExpression(@"^(?:\d{5}(?:\-\d{4})?)?$", ErrorMessage = "Invalid zip code.")]
        public string? ZIPCode { get; set; }

        public string Mobile { get; set; }
        [JsonIgnore]
        [ForeignKey("CustomerId")]
        public virtual CustomerN CustomerN { get; set; }
        [JsonIgnore]
        public virtual Country Country { get; set; }
    }
}