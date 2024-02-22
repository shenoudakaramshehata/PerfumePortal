using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageOrder
{
    public class DetailsModel : PageModel
    {
        private readonly PerfumeContext _context;


        public IEnumerable<OrderItem> OrderList { get; set; }
        public CRM.Models.Order OrderReceipt { get; set; }
        [BindProperty]
        public CRM.Models.Order order { get; set; }
        public CustomerAddress customerAddress { get; set; }
        public CustomerN Customer { get; set; }
        public double taxpercentage { get; set; }

        public double OrdernetByTax { get; set; }
        public string currencyName { get; set; }
        public string currencyNameAR { get; set; }

        public double Tax { get; set; }
        public double approximatedNumber { get; set; }
        public double ShippingCost { get; set; }
        public double DiscountAmount { get; set; }

        public List<OrderTraking> OrderTraking { get; set; }

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public DetailsModel(PerfumeContext perfumeContext, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
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
                                        .Where(o =>o.OrderId == Id).ToList();

           



            

            OrderReceipt = _context.Order.Where(i => i.OrderId == Id).Include(a => a.OrderStatus).FirstOrDefault();

            customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == OrderReceipt.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
            Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);
            OrderTraking = _context.OrderTrakings.Include(i => i.OrderStatus).Where(e => e.OrderId == Id).ToList();
            DiscountAmount = (OrderReceipt.DiscountAmount.HasValue ? OrderReceipt.DiscountAmount.Value : 0) + (OrderReceipt.CouponAmount.HasValue ? (OrderReceipt.CouponTypeId == 1 ? (OrderReceipt.CouponAmount.Value / 100) * OrderReceipt.OrderTotal : OrderReceipt.CouponAmount.Value) : 0);

            currencyName = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen;
            currencyNameAR = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlar;
            Tax = _context.Country.Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().tax.Value;
            ShippingCost = _context.Country.Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().ShippingCost.Value;
            OrdernetByTax = (OrderReceipt.OrderTotal * Tax);
            double originalNumber = OrderReceipt.OrderNet.Value;
            int decimalPlaces = 2;
             approximatedNumber = Math.Round(originalNumber, decimalPlaces);
            taxpercentage = Tax * 100;

        }

        public async Task<IActionResult> OnPost(int Id)
        {
            try
            {
                var orderObj = _context.Order.Where(e => e.OrderId == Id).FirstOrDefault();
                if (orderObj != null)
                {
                    if (orderObj.OrderStatusId == 6)
                    {
                        orderObj.OrderStatusId = 7;

                        OrderTraking orderTrakingObj = new OrderTraking()
                        {
                            OrderId = Id,
                            ActionDate = DateTime.Now,
                            Remarks = "Order Is Packing",
                            OrderStatusId = 7
                        };
                        _context.OrderTrakings.Add(orderTrakingObj);


                        var UpdatedOrder = _context.Order.Attach(orderObj);
                        UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Order Updated Successfully");
                    }
                    if (orderObj.OrderStatusId == 7)
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
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
            }


            return RedirectToPage("/Configurations/ManageStoredOrder/Orders");
        }

    }
}
