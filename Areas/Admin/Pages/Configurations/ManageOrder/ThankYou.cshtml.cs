using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Areas.Admin.Pages.Configurations.ManageOrder
{
    public class ThankYouModel : PageModel
    {
        public int OrderNo { get; set; }
        public void OnGet(int orderId)
        {
            OrderNo = orderId;
        }
    }
}
