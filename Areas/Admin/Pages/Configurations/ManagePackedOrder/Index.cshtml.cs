using CRM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManagePackedOrder
{
    public class IndexModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        public string url { get; set; }

        public List<CRM.Models.Order> OrderList { get; set; }


        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment,
                                           IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
        }
        public void OnGet()
        {
            url = $"{this.Request.Scheme}://{this.Request.Host}";
        }
    }
}
