using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManagePageContent
{
    public class IndexModel : PageModel
    {

        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        [BindProperty]
        public PageContent PageContent { get; set; }
        public List<PageContent> pageContents { set; get; }


        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }
        public ActionResult OnGet()
        {
            pageContents = _context.PageContents.ToList();

            return Page();

        }

    }
}
