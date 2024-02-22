using CRM.Data;
using CRM.Reports;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CRM.Areas.Admin.Pages.Configurations.ManageReport
{
    public class OrdersReportModel : PageModel
    {
        [BindProperty]
        public FinancialFilterVm financialFilterVm { get; set; }
        public OrdersRPT Report { get; set; }
        private readonly IToastNotification _toastNotification;

        private readonly PerfumeContext _context;

        public OrdersReportModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
            financialFilterVm = new FinancialFilterVm();
        }

        public async Task<IActionResult> OnGet()
        {
            Report = new OrdersRPT();
            decimal totalOrderNetSum = (decimal)_context.Order.Where(e=>e.IsPaid==true).Sum(e => e.OrderNet.Value);
            var orderCount = _context.Order.Count();
            List<OrdersReportVM> ds = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(i => i.OrderItem).ThenInclude(o => o.Item).Select(i => new OrdersReportVM
            {

                OrderId = i.OrderId,
                OrderDate = i.OrderDate,
                CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName + " , " + i.CustomerAddress.Address + " , " + i.CustomerAddress.BuildingNo,
                CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
                Tax = i.tax.HasValue ? i.tax.Value * 100 : 0,
                orderNetByTax =i.tax.HasValue ? Math.Round((i.OrderTotal * i.tax.Value), 2):0,
                Discount = (i.DiscountAmount.HasValue ? i.DiscountAmount.Value : 0) + (i.CouponAmount.HasValue ? (i.CouponTypeId == 1 ? (i.CouponAmount.Value / 100) * i.OrderTotal : i.CouponAmount.Value) : 0),
                Status= _context.OrderStatuses.Where(e=>e.OrderStatusId== i.OrderStatusId).FirstOrDefault().Status,
                OrderStatusId= i.OrderStatusId,
                //NetOrder = i.Order.OrderNet.Value,
                 NetOrder = i.OrderNet.HasValue?(decimal)Math.Round(i.OrderNet.Value, 2):0,
                Country= _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryTlen,

                InvoiceNumber = i.OrderId,
                WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
                ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                ShippingCost =i.Deliverycost.HasValue ? i.Deliverycost.Value:0,
                AdminAddress = _context.SocialMediaLinks.FirstOrDefault().Address,
                InvoiceDate = i.OrderDate.Date,
                CustomerEmail = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().Email,
                CustomerPhone = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().Phone,
                CountryId = _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryId,
                currencyName = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen,
                PaymentMethod =i.PaymentMethodId!=0? _context.paymentMehods.Where(e => e.PaymentMethodId == i.PaymentMethodId).FirstOrDefault().PaymentMethodEN:"",
                orderItemVms = i.OrderItem.Select(e => new OrderItemVm
                {
                    ItemTitleEn=  e.Item.ItemTitleEn,
                    ItemPrice= e.ItemPrice,
                    ItemQuantity=  e.ItemQuantity,
                    Total=  e.Total
                }).ToList()
            }).ToList();
            Report.DataSource = ds;
            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            try
            {
                List<OrdersReportVM> ds = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(i => i.OrderItem).ThenInclude(o => o.Item).Select(i => new OrdersReportVM
                {

                    OrderId = i.OrderId,
                    OrderDate = i.OrderDate,
                    CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName + " , " + i.CustomerAddress.Address + " , " + i.CustomerAddress.BuildingNo,
                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
                    Tax = _context.Country.Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryId).FirstOrDefault().tax.Value * 100,
                    orderNetByTax = Math.Round((i.OrderTotal * _context.Country.Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryId).FirstOrDefault().tax.Value), 2),
                    Discount = (i.DiscountAmount.HasValue ? i.DiscountAmount.Value : 0) + (i.CouponAmount.HasValue ? (i.CouponTypeId == 1 ? (i.CouponAmount.Value / 100) * i.OrderTotal : i.CouponAmount.Value) : 0),
                    Status = _context.OrderStatuses.Where(e => e.OrderStatusId == i.OrderStatusId).FirstOrDefault().Status,
                    OrderStatusId = i.OrderStatusId,
                    //NetOrder = i.Order.OrderNet.Value,
                    NetOrder = (decimal)Math.Round(i.OrderNet.Value, 2),
                    Country = _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryTlen,

                    InvoiceNumber = i.OrderId,
                    WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                    SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
                    ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                    ShippingCost = _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().ShippingCost.Value,
                    AdminAddress = _context.SocialMediaLinks.FirstOrDefault().Address,
                    InvoiceDate = i.OrderDate.Date,
                    CustomerEmail = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().Email,
                    CustomerPhone = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().Phone,
                    CountryId = _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryId,
                    currencyName = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen,
                    PaymentMethod = _context.paymentMehods.Where(e => e.PaymentMethodId == i.PaymentMethodId).FirstOrDefault().PaymentMethodEN,
                    orderItemVms = i.OrderItem.Select(e => new OrderItemVm
                    {
                        ItemTitleEn = e.Item.ItemTitleEn,
                        ItemPrice = e.ItemPrice,
                        ItemQuantity = e.ItemQuantity,
                        Total = e.Total
                    }).ToList()
                }).ToList();

                if (financialFilterVm.From != null && financialFilterVm.To == null)
            {
                ds = null;
            }
            if (financialFilterVm.From == null && financialFilterVm.To != null)
            {
                ds = null;
            }
            if (financialFilterVm.From != null && financialFilterVm.To != null)
            {
                ds = ds.Where(i => i.OrderDate.Date <= financialFilterVm.To.Value.Date && i.OrderDate.Date >= financialFilterVm.From.Value.Date).ToList();
            }
            if (financialFilterVm.OrderStatusId != null)
            {
                ds = ds.Where(i => i.OrderStatusId == financialFilterVm.OrderStatusId).ToList();
            }

            if (financialFilterVm.OrderStatusId == null && financialFilterVm.From == null && financialFilterVm.To == null)
            {
                ds = new List<OrdersReportVM>();
            }


            Report = new OrdersRPT();
            Report.DataSource = ds;
                return Page();

            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();
            }
        }
    }
}
