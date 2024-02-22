using CRM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageCreateOrder
{
    public class ThankYouModel : PageModel
    {
        public int OrderNo { get; set; }
        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ThankYouModel(PerfumeContext perfumeContext, IToastNotification toastNotification, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {

            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;

            this._configuration = configuration;
            _hostEnvironment = hostEnvironment;


        }
        public void OnGet(int orderId)
        {
            OrderNo = orderId;
        }

        public async Task<IActionResult> OnGetPayment(int orderId)
        {
            var order = perfumeContext.Order.Include(e => e.OrderItem).Where(e => e.OrderId == orderId).FirstOrDefault();
            var country = perfumeContext.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault();
            var currency = perfumeContext.Currency.Where(e => e.CurrencyId == country.CurrencyId).FirstOrDefault().CurrencyTlar;
            var result = new { currency = currency, total = order.OrderNet };
            return new JsonResult(result);
        }
    }
}
