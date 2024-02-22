using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace CRM.Areas.Admin.Pages.Configurations.ManageStoredOrder
{
    public class ShippingLabelModel : PageModel
    {
        private readonly PerfumeContext _context;

        [BindProperty]
        public CRM.Models.Order order { get; set; }
        public ShippingLabelModel(PerfumeContext perfumeContext)
        {
            _context = perfumeContext;
            order = new Models.Order();
        }
        public void OnGet(int id)
        {
            order = _context.Order.Where(e => e.OrderId == id).FirstOrDefault();
        }
    }
}
