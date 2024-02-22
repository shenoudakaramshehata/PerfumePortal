using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Configurations.ManageOrder
{
    public class OrderDetailsModel : PageModel
    {

        private readonly PerfumeContext _context;


        public IEnumerable<OrderItem> OrderList { get; set; }
        [BindProperty]
        public CRM.Models.Order OrderReceipt { get; set; }
        public string currencyName { get; set; }
        public string currencyNameAR { get; set; }

        public double Tax { get; set; }
        public double ShippingCost { get; set; }

        public CustomerAddress customerAddress { get; set; }
        public CustomerN Customer { get; set; }

        public OrderDetailsModel(PerfumeContext perfumeContext)
        {
            _context = perfumeContext;
        }

        public void OnGet([FromQuery]int Id)
        {
            OrderList = _context.OrderItem.Include(i => i.Item).Include(i => i.Order)
                                            .ThenInclude(a=>a.CustomerAddress).ThenInclude(a=>a.CustomerN)
                                        .Where(Order => Order.OrderId == Id).ToList();

            var customerId = _context.Order.Where(o => o.OrderId == Id).FirstOrDefault().CustomerId;


            customerAddress = _context.customerAddresses.Where(c => c.CustomerId == customerId).FirstOrDefault();

            Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

            OrderReceipt = _context.Order.Where(i => i.OrderId == Id).Include(a=>a.OrderStatus).FirstOrDefault();

           
        }

    }
}
