using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Areas.Admin.Pages.Configurations.ManageReport
{
    public class MergeReportModel : PageModel
    {
        public int OrderInvoiceParam { get; set; }

        public IActionResult OnGet(int id)
        {
            OrderInvoiceParam = id;
            return Page();

        }
    }
}
