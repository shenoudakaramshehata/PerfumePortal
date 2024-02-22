using CRM.Data;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Configurations.ManageOrder
{
    public class OrderInvoiceModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;
        public InvoiceVm Order { get; set; }

        public OrderInvoiceModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public IActionResult OnGet(int id)
        {
            try
            {
                Order = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderId == id).Select(i => new InvoiceVm
                {
                    OrderId = i.OrderId,
                    OrderDate = i.OrderDate.Date.Year + " , " + i.OrderDate.Date.Month + " , " + i.OrderDate.Date.Day,
                    OrderTime = i.OrderDate.TimeOfDay.Hours + " : " + i.OrderDate.TimeOfDay.Minutes,
                    Country = i.Country.CountryTlen,
                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                    CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                    NetOrder = i.OrderNet.Value,
                    OrderTotal = i.OrderTotal,
                    Status = i.OrderStatus.Status,
                    Discount = i.OrderDiscount,
                    InvoiceNumber = i.UniqeId.Value,
                    WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                    SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
                    ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                    ShippingCost = _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().ShippingCost.Value,
                    ShippingAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName + " , " + i.CustomerAddress.BuildingNo,
                    Address = i.CustomerAddress.Address,
                    ShippingAddressPhone = i.CustomerAddress.Mobile,
                    PaymentTit = _context.paymentMehods.Where(e => e.PaymentMethodId == i.PaymentMethodId).FirstOrDefault().PaymentMethodEN,


                }).FirstOrDefault();
                if (Order == null)
                {
                    _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");
                    return RedirectToPage("SomethingwentError");
                }
                else
                {
                    var orderItemVm = _context.OrderItem.Include(e => e.Item).Where(e => e.OrderId == Order.OrderId).Select(i => new OrderItemVm
                    {
                        ItemImage = i.Item.ItemImage,
                        ItemPrice = i.ItemPrice,
                        ItemQuantity = i.ItemQuantity,
                        ItemTitleEn = i.Item.ItemTitleEn,
                        Total = i.Total,
                        ItemDis = i.Item.ItemDescriptionEn
                    }).ToList();
                    Order.orderItemVms = orderItemVm;
                }
                // Order = _context.Order.Where(e => e.OrderSerial == serialNo).Include(e=>e.OrderStatus).Include(e => e.CustomerAddress).Include(e => e.OrderItem).FirstOrDefault();
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");
                return RedirectToPage("SomethingwentError");
            }
            return Page();
        }
    }}