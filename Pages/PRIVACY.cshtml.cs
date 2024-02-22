using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
namespace CRM.Pages
{
    public class PRIVACYModel : PageModel
    {
        public PageContent content { get; set; }
        private PerfumeContext _context;
        public IRequestCultureFeature locale;
        public string BrowserCulture;
        public string ContentAr { get; set; }

        public string ContentEn { get; set; }
        public PRIVACYModel(PerfumeContext context)
        {
            _context = context;

        }
        public void OnGet()
        {
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            var pageContent = _context.PageContents.FirstOrDefault(p => p.PageContentId == 3);
            if (pageContent != null)
            {
                ContentAr = pageContent.ContentAr;
                ContentEn = pageContent.ContentEn;

            }
        }
    }
}
