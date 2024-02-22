
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Configurations.ManageNewsLetters
{

#nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        [BindProperty]
        public Newsletter Newsletter { get; set; }
        public List<Newsletter> newsletters = new List<Newsletter>();

        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
        }
        public void OnGet()
        {
            newsletters = _context.Newsletters.ToList();

            url = $"{this.Request.Scheme}://{this.Request.Host}";
        }


    }
}
