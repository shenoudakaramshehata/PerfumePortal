using CRM.Data;
using CRM.Reports;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageReport
{
    public class teestModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;
        public testrpt Report { get; set; }
        public teestModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public IActionResult OnGet()
        {
            try
            {
                Report = new testrpt();

               
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");
                return Page();
            }
            return Page();
        }
    }
}
