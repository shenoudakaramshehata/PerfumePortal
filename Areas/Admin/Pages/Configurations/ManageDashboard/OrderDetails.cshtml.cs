using CRM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageDashboard
{
    public class OrderDetailsModel : PageModel
    {

        private readonly PerfumeContext _context;
        private readonly IToastNotification _toastNotification;

        public OrderDetailsModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }



        public void OnGet(int OrderId)
        {

        }
    }
}
