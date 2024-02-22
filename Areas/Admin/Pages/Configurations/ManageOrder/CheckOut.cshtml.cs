using CRM.Data;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Runtime;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using DevExpress.XtraRichEdit.Import.Html;
using CRM.Migrations;
using MimeKit;
using System.Net.Http.Headers;
using System.Text;
using CRM.Services;
using RestSharp;
using Newtonsoft.Json.Linq;


namespace CRM.Areas.Admin.Pages.Configurations.ManageOrder
{
    public class CheckOutModel : PageModel
    {

        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailSender _emailSender;
        public InvoiceVm invoiceVm { get; set; }
        private readonly IRazorPartialToStringRenderer _renderer;

        //[BindProperty(SupportsGet = true)]
        //public string CouponSerial { get; set; }
        
        public double Subtotal { get; set; }

        [BindProperty]
        public List<ShoppingCart> shoppingCarts { get; set; }
        [BindProperty]
        public List<PaymentMehod> paymentMehods { get; set; }
        
       
        [BindProperty]
        public int Payment { get; set; }
        
        public double TotalAmount { get; set; }
        //public int couponID { get; set; }
        //[BindProperty]
        //public int countryId { get; set; } 
        //public bool hasAddress { get; set; }
        
        [BindProperty]
        public int PaymentId { get; set; }

        //[BindProperty]
        public int FattorahPaymentId { get; set; }

        public int CahshPaymentId { get; set; }
        public HttpClient httpClient { get; set; }

        public double Discount { get; set; }
        public double? ShippingCost { get; set; }
        public bool IsDiscounted { get; set; } = false;

        public string CurrencyNameAr { get; set; }
        [BindProperty]
        public FastOrderVM FastOrderVM { get; set; }
        public string CurrencyNameEN { get; set; }
        public string CountryENName { get; set; }
        public string CountryARName { get; set; }
        public Coupon? coupon { get; set; }
        public double DeliveryCost { get; set; }
        public ApplicationUser user { get; set; }
        public double TotalAmountAfterDiscount { get; set; }
        public int CountryId { get; set; }
        public double tax { get; set; }
        public double OrdernetByTax { get; set; }
        public double itemtotal { get; set; }
        public string newserial { get; set; }
        public List<ShippingCartObjectVM> shippingCartObjectVMs { get; set; }

        public CheckOutModel(IRazorPartialToStringRenderer renderer, PerfumeContext perfumeContext, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IConfiguration configuration, IWebHostEnvironment hostEnvironment, IEmailSender emailSender)
        {
            _renderer = renderer;
            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;
            this.userManager = userManager;
            this._configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            httpClient = new HttpClient();
            shippingCartObjectVMs = new List<ShippingCartObjectVM>();
            FastOrderVM = new FastOrderVM(); 

        }

        public async Task<IActionResult> OnGet(string Serial)
        {
           
            user = await userManager.GetUserAsync(User);
            var Country = HttpContext.Session.GetString("country");
            if (Country != null)
            {
                CountryId = int.Parse(Country);
                tax = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value;
                CountryENName = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;
                CurrencyNameEN = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlen;
                CurrencyNameAr = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlar;
            }

            if (user != null)
            {
                if (Country != null)
                {
                    var customerId = perfumeContext.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;
                shoppingCarts = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item).Include(i => i.CustomerN).ToList();
                TotalAmount = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);
                DeliveryCost = perfumeContext.Country.Where(e => e.CountryId == CountryId).FirstOrDefault().ShippingCost.Value;
                
                Subtotal = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);
                    //itemtotal =
                    //    TotalAmount += DeliveryCost + tax;

                    OrdernetByTax = (TotalAmount * tax) / 100;
                    TotalAmount = Subtotal + DeliveryCost + OrdernetByTax;
                }
            }
            var CouponSerialdata = HttpContext.Session.GetString("newCouponSerial");
            newserial = CouponSerialdata;



            PaymentId = perfumeContext.paymentMehods.FirstOrDefault().PaymentMethodId;
            FattorahPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 1).FirstOrDefault().PaymentMethodId;
            CahshPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 2).FirstOrDefault().PaymentMethodId;
            paymentMehods = perfumeContext.paymentMehods.ToList();

            coupon = perfumeContext.Coupon.Where(i => i.Serial == newserial).FirstOrDefault();


            GetDiscountFromCoupon(TotalAmount, coupon);


            return Page();
        }




        public async Task<IActionResult> OnPost(string Serial)
        {
            try
            {
                var payment = Request.Form["paymentMethod"];
                var paymentId = int.Parse(payment);
                var user = await userManager.GetUserAsync(User);

               
                    var Country = HttpContext.Session.GetString("country");
                    var customerObj = perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                    if (customerObj == null)
                    {
                        return NotFound();

                    }


                    var countryId = perfumeContext.Country.Where(c => c.CountryId == int.Parse(Country)).FirstOrDefault().CountryId;
                    var tax = perfumeContext.Country.Where(c => c.CountryId == int.Parse(Country)).FirstOrDefault().tax;
                var customer = new CustomerN()
                {
                    CustomerName = FastOrderVM.FullName,
                    Email = FastOrderVM.Email,
                    Phone = FastOrderVM.PhoneNumber,
                    RegisterDate = DateTime.Now
                };
                perfumeContext.CustomerNs.Add(customer);
                perfumeContext.SaveChanges();
                var CustomerAddress = new CustomerAddress()
                {
                    Address = FastOrderVM.Address,
                    AreaName = FastOrderVM.Areaname,
                    BuildingNo = FastOrderVM.BuildingNo,
                    CityName = FastOrderVM.Cityname,
                    CustomerId = customer.CustomerId,
                    CountryId = int.Parse(Country),
                    Mobile = FastOrderVM.PhoneNumber
                };
                perfumeContext.customerAddresses.Add(CustomerAddress);
                perfumeContext.SaveChanges();


                var existedAddressForCustomer = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();

                    var CountryObj = perfumeContext.Country.Where(i => i.CountryId == countryId).FirstOrDefault();
                    string currencyISO = perfumeContext.Currency.Where(i => i.CurrencyId == CountryObj.CurrencyId).FirstOrDefault().CurrencyTlen;

                    var customerShoppingCartList = perfumeContext.
                     ShoppingCart.Include(s => s.CustomerN)
                     .Include(s => s.Item)
                     .Where(c => c.CustomerId == customerObj.CustomerId);

                    double shoppingCost = 0.0;

                    shoppingCost = CountryObj.ShippingCost.Value;

                    var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);

                    coupon = perfumeContext.Coupon.Where(i => i.Serial == Serial).FirstOrDefault();

                    double discount = 0.0;

                    GetDiscountFromCoupon(totalOfAll, coupon);


                    int maxUniqe = 1;
                    var newList = perfumeContext.Order.ToList();
                    var maxserial = Convert.ToInt64(1);
                    perfumeContext.Order.ToList().Max(e => Convert.ToInt64(e.OrderSerial));
                    if (newList != null)
                    {
                        if (newList.Count > 0)
                        {
                            maxUniqe = newList.Max(e => e.UniqeId).Value;
                            maxserial = newList.Max(e => Convert.ToInt64(e.OrderSerial));
                        }
                    }
                var TaxRateValue = CountryObj.tax;

                var orders =
                new Models.Order
                {
                    OrderDate = DateTime.Now,
                    OrderSerial = Convert.ToString(maxserial + 1),
                    CustomerId = customerObj.CustomerId,
                    CustomerAddressId = CustomerAddress.CustomerAddressId,
                    OrderTotal = totalOfAll,
                    CouponId = coupon != null ? coupon.CouponId : null,
                    CouponTypeId = coupon != null ? coupon.CouponTypeId : null,
                    CouponAmount = coupon != null ? (float?)coupon.Amount : null,
                    Deliverycost = shoppingCost,
                    OrderNet = TaxRateValue == 0 ? (TotalAmountAfterDiscount + shoppingCost) : ((TotalAmountAfterDiscount) * (1 + TaxRateValue)) + shoppingCost,
                    //OrderNet = TotalAmountAfterDiscount + shoppingCost + CountryObj.tax,
                    PaymentMethodId = paymentId,
                    OrderDiscount = Discount,
                    IsCanceled = false,
                    OrderStatusId = 1,
                    CountryId = countryId,
                    tax = tax,
                    UniqeId = maxUniqe + 1,
                    IsDeliverd = false,
                    Notes = FastOrderVM.Notes
                };


                    perfumeContext.Order.Add(orders);
                    perfumeContext.SaveChanges();


                    List<OrderItem> orderItems = new List<OrderItem>();

                    foreach (var itemObj in customerShoppingCartList)
                    {

                        OrderItem orderItem = new OrderItem
                        {
                            ItemId = itemObj.ItemId,
                            ItemPrice = itemObj.ItemPrice,
                            Total = itemObj.ItemTotal,
                            ItemQuantity = itemObj.ItemQty,
                            OrderId = orders.OrderId
                        };

                        perfumeContext.OrderItem.Add(orderItem);

                    }

                    var TrakingOrderObj = new OrderTraking()
                    {
                        OrderId = orders.OrderId,
                        OrderStatusId = 1,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Initiated"
                    };

                    perfumeContext.OrderTrakings.Add(TrakingOrderObj);

                    perfumeContext.SaveChanges();

                    if (paymentId == 1) // My Fatoorah
                    {
                    var Cost = perfumeContext.Country.Where(e => e.CountryId == orders.CountryId).FirstOrDefault().ShippingCost;

                    var order = perfumeContext.Order.Where(e => e.OrderId == orders.OrderId).FirstOrDefault();

                    order.IsPaid = true;


                    order.OrderStatusId = 2;
                    var UpdatedOrder = perfumeContext.Order.Attach(orders);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    perfumeContext.SaveChanges();
                    var TrakingOrder = new OrderTraking()
                    {
                        OrderId = order.OrderId,
                        OrderStatusId = 2,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Paid Successfully"
                    };
                    perfumeContext.OrderTrakings.Add(TrakingOrder);
                    perfumeContext.SaveChanges();
                    foreach (var orderItem in order.OrderItem)
                    {
                        var item = perfumeContext.Item.Find(orderItem.ItemId);
                        item.Stock -= orderItem.ItemQuantity;
                        var UpdatedItem = perfumeContext.Item.Attach(item);
                        UpdatedItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();

                    }
                    var Customer = perfumeContext.CustomerNs.Where(e => e.CustomerId == orders.CustomerId).FirstOrDefault();
                    if (Customer != null)
                    {
                        var carts = perfumeContext.ShoppingCart.Where(e => e.CustomerId == orders.CustomerId);
                        perfumeContext.ShoppingCart.RemoveRange(carts);
                        perfumeContext.SaveChanges();
                    }
                
                    
                    var webRoot = _hostEnvironment.WebRootPath;

                    var pathToFile = _hostEnvironment.WebRootPath
                           + Path.DirectorySeparatorChar.ToString()
                           + "Templates"
                           + Path.DirectorySeparatorChar.ToString()
                           + "EmailTemplate"
                           + Path.DirectorySeparatorChar.ToString()
                           + "Email.html";
                    var builder = new BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {

                        builder.HtmlBody = SourceReader.ReadToEnd();

                    }
                    //string messageBody = string.Format(builder.HtmlBody,
                    //   Cost,
                    //   orders.OrderDiscount,
                    //   orders.OrderNet,
                    //   Customer.CustomerName,
                    //   orders.OrderTotal,
                    //   orders.OrderSerial

                    //   );
                    invoiceVm = perfumeContext.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderId == orders.OrderId).Select(i => new InvoiceVm
                    {
                        OrderId = i.OrderId,
                        OrderDate = i.OrderDate.Date.Year + " , " + i.OrderDate.Date.Month + " , " + i.OrderDate.Date.Day,
                        OrderTime = i.OrderDate.TimeOfDay.Hours + " : " + i.OrderDate.TimeOfDay.Minutes,
                        Country = i.Country.CountryTlen,
                        CusName = perfumeContext.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        OrderTotal = i.OrderTotal,
                        Status = i.OrderStatus.Status,
                        Discount = i.OrderDiscount,
                        InvoiceNumber = i.UniqeId.Value,
                        WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                        SuppEmail = perfumeContext.SocialMediaLinks.FirstOrDefault().ContactMail,
                        ConntactNumber = perfumeContext.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                        ShippingCost = perfumeContext.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().ShippingCost.Value,
                        ShippingAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName + " , " + i.CustomerAddress.BuildingNo,
                        Address = i.CustomerAddress.Address,
                        ShippingAddressPhone = i.CustomerAddress.Mobile,
                        PaymentTit = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == i.PaymentMethodId).FirstOrDefault().PaymentMethodEN,
                        currencyName = perfumeContext.Currency.Where(e => e.CurrencyId == i.Country.CurrencyId).FirstOrDefault().CurrencyTlen

                    }).FirstOrDefault();
                    if (invoiceVm == null)
                    {
                        return RedirectToPage("SomethingwentError");
                    }
                    else
                    {
                        var orderItemVm = perfumeContext.OrderItem.Include(e => e.Item).Where(e => e.OrderId == invoiceVm.OrderId).Select(i => new OrderItemVm
                        {
                            ItemImage = i.Item.ItemImage,
                            ItemPrice = i.ItemPrice,
                            ItemQuantity = i.ItemQuantity,
                            ItemTitleEn = i.Item.ItemTitleEn,
                            Total = i.Total,
                            ItemDis = i.Item.ItemDescriptionEn
                        }).ToList();
                        invoiceVm.orderItemVms = orderItemVm;
                    }



                    var messageBody = await _renderer.RenderPartialToStringAsync("_Invoice", invoiceVm);
                    await _emailSender.SendEmailAsync(Customer.Email, "Order Details", messageBody);

                    return RedirectToPage("/Configurations/ManageOrder/Thankyou", new { orderId = orders.OrderId });
                    



                }
                    else if (paymentId == 2) //Cash On Delivery --> Not Exists on This Applicaiton
                    {
                        var Cost = perfumeContext.Country.Where(e => e.CountryId == orders.CountryId).FirstOrDefault().ShippingCost;

                    var order = perfumeContext.Order.Include(e => e.OrderItem).Where(e => e.OrderId == orders.OrderId).FirstOrDefault();

                    order.IsPaid = true;


                    order.OrderStatusId = 2;
                    var UpdatedOrder = perfumeContext.Order.Attach(orders);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    perfumeContext.SaveChanges();
                    var TrakingOrder = new OrderTraking()
                    {
                        OrderId = order.OrderId,
                        OrderStatusId = 2,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Paid Successfully"
                    };
                    perfumeContext.OrderTrakings.Add(TrakingOrder);
                    perfumeContext.SaveChanges();
                    foreach (var orderItem in order.OrderItem)
                    {
                        var item = perfumeContext.Item.Find(orderItem.ItemId);
                        item.Stock -= orderItem.ItemQuantity;
                        var UpdatedItem = perfumeContext.Item.Attach(item);
                        UpdatedItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();

                    }
                    var Customer = perfumeContext.CustomerNs.Where(e => e.CustomerId == orders.CustomerId).FirstOrDefault();
                        if (Customer != null)
                        {
                            var carts = perfumeContext.ShoppingCart.Where(e => e.CustomerId == orders.CustomerId);
                            perfumeContext.ShoppingCart.RemoveRange(carts);
                            perfumeContext.SaveChanges();
                    }
                  
                    var webRoot = _hostEnvironment.WebRootPath;

                        var pathToFile = _hostEnvironment.WebRootPath
                               + Path.DirectorySeparatorChar.ToString()
                               + "Templates"
                               + Path.DirectorySeparatorChar.ToString()
                               + "EmailTemplate"
                               + Path.DirectorySeparatorChar.ToString()
                               + "Email.html";
                        var builder = new BodyBuilder();
                        using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                        {

                            builder.HtmlBody = SourceReader.ReadToEnd();

                        }
                        //string messageBody = string.Format(builder.HtmlBody,
                        //   Cost,
                        //   orders.OrderDiscount,
                        //   orders.OrderNet,
                        //   Customer.CustomerName,
                        //   orders.OrderTotal,
                        //   orders.OrderSerial

                        //   );
                        invoiceVm = perfumeContext.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderId == orders.OrderId).Select(i => new InvoiceVm
                        {
                            OrderId = i.OrderId,
                            OrderDate = i.OrderDate.Date.Year + " , " + i.OrderDate.Date.Month + " , " + i.OrderDate.Date.Day,
                            OrderTime = i.OrderDate.TimeOfDay.Hours + " : " + i.OrderDate.TimeOfDay.Minutes,
                            Country = i.Country.CountryTlen,
                            CusName = perfumeContext.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                            CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                            NetOrder = i.OrderNet.Value,
                            OrderTotal = i.OrderTotal,
                            Status = i.OrderStatus.Status,
                            Discount = i.OrderDiscount,
                            InvoiceNumber = i.UniqeId.Value,
                            WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                            SuppEmail = perfumeContext.SocialMediaLinks.FirstOrDefault().ContactMail,
                            ConntactNumber = perfumeContext.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                            ShippingCost = perfumeContext.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().ShippingCost.Value,
                            ShippingAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName + " , " + i.CustomerAddress.BuildingNo,
                            Address = i.CustomerAddress.Address,
                            ShippingAddressPhone = i.CustomerAddress.Mobile,
                            PaymentTit = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == i.PaymentMethodId).FirstOrDefault().PaymentMethodEN,
                            currencyName = perfumeContext.Currency.Where(e => e.CurrencyId == i.Country.CurrencyId).FirstOrDefault().CurrencyTlen

                        }).FirstOrDefault();
                        if (invoiceVm == null)
                        {
                            return RedirectToPage("SomethingwentError");
                        }
                        else
                        {
                            var orderItemVm = perfumeContext.OrderItem.Include(e => e.Item).Where(e => e.OrderId == invoiceVm.OrderId).Select(i => new OrderItemVm
                            {
                                ItemImage = i.Item.ItemImage,
                                ItemPrice = i.ItemPrice,
                                ItemQuantity = i.ItemQuantity,
                                ItemTitleEn = i.Item.ItemTitleEn,
                                Total = i.Total,
                                ItemDis = i.Item.ItemDescriptionEn
                            }).ToList();
                            invoiceVm.orderItemVms = orderItemVm;
                        }



                        var messageBody = await _renderer.RenderPartialToStringAsync("_Invoice", invoiceVm);
                        await _emailSender.SendEmailAsync(Customer.Email, "Order Details", messageBody);


                    return RedirectToPage("/Configurations/ManageOrder/Thankyou", new { orderId = orders.OrderId });
                    
                }

            }





            catch (Exception ex)
            {
                toastNotification.AddErrorToastMessage("Something went wrong");
                return RedirectToPage("/Configurations/ManageOrder/CheckOut");
            }
            return RedirectToPage("/Configurations/ManageOrder/CheckOut");

        }


        public void GetDiscountFromCoupon(double totalOfAll, Coupon coupon)
        {
            Discount = 0;
            double sumItemTotal = totalOfAll;
            var percent = sumItemTotal / totalOfAll;

            if (coupon == null)
            {
                Discount = 0;
                TotalAmountAfterDiscount = sumItemTotal;
            }
            else if (coupon.CouponTypeId == 2)
            {
                IsDiscounted = true;
                Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - coupon.Amount * percent), 2);

                var AmountAfterDiscount = (double)(sumItemTotal - coupon.Amount * percent);
                TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
            }
            else
            {
                IsDiscounted = true;
                var couponAmount = totalOfAll * (coupon.Amount / 100);
                Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - couponAmount * percent), 2);
                var AmountAfterDiscount = (double)(sumItemTotal - couponAmount * percent);
                TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
            }
            if (TotalAmountAfterDiscount < 0)
            {
                TotalAmountAfterDiscount = 0;
            }


        }
    }
}
