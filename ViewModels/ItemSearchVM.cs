using System.ComponentModel.DataAnnotations;

namespace CRM.ViewModels
{
    public class ItemSearchVM
    {
        [Required(ErrorMessage = "Is Required")]
        public string? SearchItem { get; set; }
    }
}
