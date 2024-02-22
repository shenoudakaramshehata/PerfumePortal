using System.ComponentModel.DataAnnotations;

namespace CRM.ViewModels
{
    public class SearchOrder
    {
        [Required(ErrorMessage = "Is Required")]
        public string? OrderSearchItem { get; set; }
    }
}
