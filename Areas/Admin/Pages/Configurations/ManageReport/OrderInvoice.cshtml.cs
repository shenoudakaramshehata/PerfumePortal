using CRM.Data;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.EntityFrameworkCore;
using CRM.Reports;
using CRM.Models;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using CRM.Migrations;

namespace CRM.Areas.Admin.Pages.Configurations.ManageReport
{
    public class OrderInvoiceModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;
        public OrdeInvoice Report { get; set; }
        public OrderInvoiceModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public IActionResult OnGet(int id)
        {
            try 
            { 
                Report = new OrdeInvoice();

                List<InvoiceVm> Order = _context.OrderItem.Include(i => i.Item).Include(o => o.Order).ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN).Where(e => e.OrderId == id).Select(i => new InvoiceVm
                {
                    Total = i.Total,
                    ItemPrice = i.ItemPrice,
                    ItemQuantity = i.ItemQuantity,
                    ItemTitleEn = i.Item.ItemTitleEn,
                    OrderId = i.Order.OrderId,
                    OrderDate = i.Order.OrderDate.ToString(),
                    CusAddress = i.Order.CustomerAddress.Country.CountryTlen + " , "  + i.Order.CustomerAddress.CityName + " , " + i.Order.CustomerAddress.AreaName + " , " + i.Order.CustomerAddress.Address + " , " + i.Order.CustomerAddress.BuildingNo,
                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.Order.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
                    Notes= i.Order.Notes!=null ? i.Order.Notes:"",
                    //NetOrder = i.Order.OrderNet.Value,
                    NetOrder = Math.Round(i.Order.OrderNet.Value, 2),

                    OrderTotal = i.Order.OrderTotal,

                    Discount = (i.Order.DiscountAmount.HasValue ? i.Order.DiscountAmount.Value : 0) + (i.Order.CouponAmount.HasValue ? (i.Order.CouponTypeId == 1 ? (i.Order.CouponAmount.Value / 100) * i.Order.OrderTotal : i.Order.CouponAmount.Value) : 0),


                    InvoiceNumber = i.Order.OrderId,
                    WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                    SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
                    ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                    ShippingCost = i.Order.Deliverycost.HasValue? i.Order.Deliverycost.Value:0,
                    AdminAddress = _context.SocialMediaLinks.FirstOrDefault().Address,
                    InvoiceDate = i.Order.OrderDate.Date,
                    CustomerEmail = _context.CustomerNs.Where(e => e.CustomerId == i.Order.CustomerAddress.CustomerId).FirstOrDefault().Email,
                    CustomerPhone = _context.CustomerNs.Where(e => e.CustomerId == i.Order.CustomerAddress.CustomerId).FirstOrDefault().Phone,
                    CountryId = _context.Country.Where(e=>e.CountryId== i.Order.CountryId).FirstOrDefault().CountryId,
                    currencyName= _context.Country.Include(e=>e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == i.Order.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen,
                    Tax = i.Order.tax.HasValue? i.Order.tax.Value * 100:0,
                    PaymentMethod = i.Order.PaymentMethodId != 0 ? _context.paymentMehods.Where(e => e.PaymentMethodId == i.Order.PaymentMethodId).FirstOrDefault().PaymentMethodEN : "",
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
