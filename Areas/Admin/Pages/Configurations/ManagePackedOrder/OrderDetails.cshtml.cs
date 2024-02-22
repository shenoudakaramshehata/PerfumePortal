using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManagePackedOrder
{
    public class OrderDetailsModel : PageModel
    {
        private readonly PerfumeContext _context;


        public IEnumerable<OrderItem> OrderList { get; set; }
        public CRM.Models.Order OrderReceipt { get; set; }
        [BindProperty]
        public CRM.Models.Order order { get; set; }
        public CustomerAddress customerAddress { get; set; }
        public CustomerN Customer { get; set; }
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public OrderDetailsModel(PerfumeContext perfumeContext, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = perfumeContext;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            order = new Models.Order();
        }

        public void OnGet([FromQuery] int Id)
        {
            OrderList = _context.OrderItem.Include(i => i.Item).Include(i => i.Order)
                                            .ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN)
                                        .Where(Order => Order.OrderId == Id).ToList();

            var customerId = _context.Order.Where(o => o.OrderId == Id).FirstOrDefault().CustomerId;


            customerAddress = _context.customerAddresses.Where(c => c.CustomerId == customerId).FirstOrDefault();

            Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

            OrderReceipt = _context.Order.Where(i => i.OrderId == Id).Include(a => a.OrderStatus).FirstOrDefault();
        }

        public async Task<IActionResult> OnPost(int Id)
        {
            try
            {
                var orderObj = _context.Order.Where(e => e.OrderId == Id).FirstOrDefault();
                if (orderObj != null)
                {


                    orderObj.OrderStatusId = 5;
            
                    OrderTraking orderTrakingObj = new OrderTraking()
                    {
                        OrderId = Id,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Is OnDelivery",
                        OrderStatusId = 5
                    };
                    _context.OrderTrakings.Add(orderTrakingObj);


                    var UpdatedOrder = _context.Order.Attach(orderObj);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Order Updated Successfully");
                }
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
            }


            return RedirectToPage("/Configurations/ManagePackedOrder/Index");
        }
        
    }
}
