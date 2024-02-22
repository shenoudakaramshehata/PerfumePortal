using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Threading.Tasks;

namespace CRM.Pages
{
    public class AddNewPageModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;

        public AddNewPageModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public void OnGet()
        {
            try
            {
                //var orderList = _context.Order.OrderByDescending(e=>e.OrderId).ToList();
                var payList = _context.paymentMehods.OrderByDescending(e=>e.PaymentMethodId).ToList();



            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");

            }
        }
    }
}
      