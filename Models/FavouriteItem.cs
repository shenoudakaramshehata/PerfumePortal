using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CRM.Models
{
    public class FavouriteItem
    {
        [Key]
        public int FavouriteItemId { get; set; }
        public string? UserId  { get; set; }
        public int ItemId { get; set; }
        [JsonIgnore]
        public virtual Item Item { get; set; }
    }
}
