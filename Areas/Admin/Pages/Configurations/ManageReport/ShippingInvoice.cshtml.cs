using CRM.Data;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.EntityFrameworkCore;
using CRM.Reports;
using CRM.Migrations;
using CRM.Models;

namespace CRM.Areas.Admin.Pages.Configurations.ManageReport
{
    public class ShippingInvoiceModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;
        public ShippingInvoiceRpt Report { get; set; }
        public ShippingInvoiceModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public IActionResult OnGet(int id)
        {
            try
            {
                Report = new ShippingInvoiceRpt();
                List<InvoiceVm> Order = _context.OrderItem.Include(i => i.Item).Include(o => o.Order).ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN).Where(e => e.OrderId == id).Select(i => new InvoiceVm
                {
                    Total = i.Total,
                    ItemPrice = i.ItemPrice,
                    ItemQuantity = i.ItemQuantity,
                    ItemTitleEn = i.Item.ItemTitleEn,
                    OrderId = i.Order.OrderId,
                    OrderDate = i.Order.OrderDate.ToString(),
                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.Order.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
                    CusAddress = i.Order.CustomerAddress.Country.CountryTlen + " , " + i.Order.CustomerAddress.CityName + " , " + i.Order.CustomerAddress.AreaName + " , " + i.Order.CustomerAddress.Address + " , " + i.Order.CustomerAddress.BuildingNo,
                    OrderTotal = i.Order.OrderTotal,
                    Discount = (i.Order.DiscountAmount.HasValue ? i.Order.DiscountAmount.Value : 0) + (i.Order.CouponAmount.HasValue ? (i.Order.CouponTypeId == 1 ? (i.Order.CouponAmount.Value / 100) * i.Order.OrderTotal : i.Order.CouponAmount.Value) : 0),
                    InvoiceNumber = i.Order.OrderId,
                    WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                    SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
                    ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                    ShippingCost = _context.itemPriceNs.Where(e => e.CountryId == i.Order.CountryId).FirstOrDefault().ShippingPrice.Value,
                    
                    NetOrder = ((double)(i.Order.tax.HasValue? (i.Order.OrderTotal +(i.Order.OrderTotal * i.Order.tax) + _context.itemPriceNs.Where(e => e.CountryId == i.Order.CountryId).FirstOrDefault().ShippingPrice.Value) - i.Order.DiscountAmount.Value : i.Order.OrderTotal + _context.itemPriceNs.Where(e => e.CountryId == i.Order.CountryId).FirstOrDefault().ShippingPrice.Value)),

                    //NetOrder = i.Order.OrderTotal + ,
                    AdminAddress = _context.SocialMediaLinks.FirstOrDefault().Address,
                    InvoiceDate = i.Order.OrderDate.Date,
                    CustomerEmail = _context.CustomerNs.Where(e => e.CustomerId == i.Order.CustomerAddress.CustomerId).FirstOrDefault().Email,
                    CustomerPhone = _context.CustomerNs.Where(e => e.CustomerId == i.Order.CustomerAddress.CustomerId).FirstOrDefault().Phone,
                    currencyName = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == i.Order.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen,
                   Tax = i.Order.tax.HasValue? i.Order.tax.Value * 100:0,
                   PaymentMethod =i.Order.PaymentMethodId!=0? _context.paymentMehods.Where(e => e.PaymentMethodId == i.Order.PaymentMethodId).FirstOrDefault().PaymentMethodEN:"",
                    orderNetByTax = Math.Round((i.Order.OrderTotal * i.Order.tax.Value), 2)
                }).ToList();

                Report.DataSource = Order;

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");
                return Page();
            }
            return Page();
        }

    }
}
