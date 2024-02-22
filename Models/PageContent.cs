using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public partial class PageContent
    {
        [Key]
        public int PageContentId { get; set; }

        [Required(ErrorMessage = "Please Fill the field")]
        public string PageTitleAr { get; set; } = string.Empty;
       
        public string ContentAr { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please Fill the field")]
        public string PageTitleEn { get; set; } = string.Empty;
      
        public string ContentEn { get; set; } = string.Empty;
    }
}
