using CRM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageDashboard
{
    public class InvoiceDetailsModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly IToastNotification _toastNotification;

        public InvoiceDetailsModel(PerfumeContext context,IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
       
        
        
        public void OnGet()
        {

        }
    }
}
