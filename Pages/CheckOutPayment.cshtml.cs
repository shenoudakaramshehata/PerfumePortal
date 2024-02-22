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
using Microsoft.AspNetCore.Localization;
using RestSharp;
using Newtonsoft.Json.Linq;
using CRM.Services.TabbyModels;
using DevExpress.DataAccess.Native.Web;

namespace CRM.Pages
{
    public class CheckOutPaymentModel : PageModel
    {
        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailSender _emailSender;

        public TabbyResponse TabbyApiResponse { get; set; }
        public InvoiceVm invoiceVm { get; set; }
        private readonly IRazorPartialToStringRenderer _renderer;

        //[BindProperty(SupportsGet = true)]
        //public string CouponSerial { get; set; }
        public string url { get; set; }
        public double Subtotal { get; set; }
        public double OrdernetByTax { get; set; }
        public double tabbyorderNet { get; set; }


        public double OrdernetByInstallments { get; set; }

        [BindProperty]
        public List<ShoppingCart> shoppingCarts { get; set; }
        [BindProperty]
        public List<PaymentMehod> paymentMehods { get; set; }
        [BindProperty]
        public CustomerAddress? customerAddr { get; set; }
        //[BindProperty]
        //public CustomerAddressVM newCustomerAddressVM { get; set; }
        [BindProperty]
        public int Payment { get; set; }
        public Coupon? coupon { get; set; }
        public double TotalAmount { get; set; }
        //public int couponID { get; set; }
        //[BindProperty]
        //public int countryId { get; set; } 
        //public bool hasAddress { get; set; }
        public string newserial { get; set; }
        [BindProperty]
        public int PaymentId { get; set; }
        public double taxvalue { get; set; }
        //[BindProperty]
        public int FattorahPaymentId { get; set; }

        public int CahshPaymentId { get; set; }
        public int TabbyPaymentId { get; set; }

        public HttpClient httpClient { get; set; }
        private readonly ILogger<CheckOutFastOrderModel> _logger;

        public double Discount { get; set; }
        public double? ShippingCost { get; set; }
        public Models.CustomerN customerobj { get; set; }
        public bool IsDiscounted { get; set; } = false;
        public double TotalAmountAfterDiscount { get; set; }

        public string CurrencyNameAr { get; set; }
        [BindProperty]
        public FastOrderVM FastOrderVM { get; set; }
        public string CurrencyNameEN { get; set; }
        bool IsInstallment = false;
        public string CountryENName { get; set; }
        public string CountryARName { get; set; }
        public double DeliveryCost { get; set; }
        public ApplicationUser user { get; set; }
        public string CountryToShow { get; set; }
        public int CountryId { get; set; }
        public double tax { get; set; }
        public int OrderID { get; set; }
        public List<ShippingCartObjectVM> shippingCartObjectVMs { get; set; }
        public static string rejectionReason { get; set; }
        public List<ShippingMatrix> shippingMatrices { get; set; }
        public double TotalWeight { get; set; }
        public double DeliverCost { get; set; }
        public CheckOutPaymentModel(ILogger<CheckOutFastOrderModel> logger, IRazorPartialToStringRenderer renderer, PerfumeContext perfumeContext, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IConfiguration configuration, IWebHostEnvironment hostEnvironment, IEmailSender emailSender)
        {
            _logger = logger;
            _renderer = renderer;
            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;
            this.userManager = userManager;
            this._configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            httpClient = new HttpClient();
            shippingCartObjectVMs = new List<ShippingCartObjectVM>();
            TabbyApiResponse = new TabbyResponse();
        }

        public async Task<IActionResult> OnGet(string Serial, int orderId)
        {
            OrderID = orderId;
            user = await userManager.GetUserAsync(User);
            var Country = HttpContext.Session.GetString("country");
            if (Country != null)
            {
                CountryId = int.Parse(Country);
                tax = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value * 100;
                taxvalue = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value;
                CountryENName = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;
                CurrencyNameEN = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlen;
                CurrencyNameAr = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlar;
                DeliveryCost = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().ShippingCost.Value;
                shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();
            }

            if (user != null)
            {
                var customerId = perfumeContext.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;
                shoppingCarts = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item).Include(i => i.CustomerN).ToList();
                TotalAmount = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);
                DeliveryCost = perfumeContext.Country.Where(e => e.CountryId == CountryId).FirstOrDefault().ShippingCost.Value;
                customerAddr = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerId).FirstOrDefault();

                foreach (var item in shoppingCarts)
                {
                    if (item != null)
                    {
                        if (item.ItemQty >= 1)
                        {
                            TotalWeight += (item.ItemQty * (item.Item.Weight ?? 0.0));
                        }


                    }

                }
                foreach (var shipCost in shippingMatrices)
                {
                    if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                    {
                        DeliveryCost = shipCost.ActualPrice;
                    }

                }
                DeliverCost = DeliveryCost;
                //TotalAmount += DeliveryCost+tax;
                Subtotal = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);

                OrdernetByTax = (TotalAmount * taxvalue);
                TotalAmount = Subtotal + DeliveryCost + OrdernetByTax;
                tabbyorderNet = TotalAmount;

                OrdernetByInstallments = TotalAmount / 4;
            }
            else
            {
                var order = perfumeContext.Order.Where(e => e.OrderId == orderId).FirstOrDefault();
                customerobj = perfumeContext.CustomerNs.Where(e => e.CustomerId == order.CustomerId).FirstOrDefault();
                customerAddr = perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
                var data = HttpContext.Session.GetString("parsecartItems");
                if (data != null)
                {

                    //List<ShippingCartObjectVM> shippingCartObjectVMs = JsonSerializer.Deserialize<List<ShippingCartObjectVM>>(data);
                    shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(data);
                    if (shippingCartObjectVMs != null)
                    {
                        foreach (var item in shippingCartObjectVMs)
                        {
                            if (item != null)
                            {

                                if (item.Qunatity >= 1)
                                {
                                    var productWeight = perfumeContext.Item.Where(a => a.ItemId == item.ItemId).FirstOrDefault().Weight;
                                    if (productWeight != null)
                                    {
                                        TotalWeight += (item.Qunatity * (productWeight ?? 0.0));

                                    }
                                }


                            }

                        }
                    }
                        foreach (var shipCost in shippingMatrices)
                        {
                        if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                        {
                            DeliveryCost = shipCost.ActualPrice;
                        }

                    }
                    DeliverCost = DeliveryCost;
                        foreach (var item in shippingCartObjectVMs)
                    {
                        Subtotal = TotalAmount + (item.priceCart * item.Qunatity);

                        TotalAmount = TotalAmount + (item.priceCart * item.Qunatity);


                    }
                    OrdernetByTax = (TotalAmount * taxvalue);
                    TotalAmount = Subtotal + DeliveryCost + OrdernetByTax;
                    tabbyorderNet = TotalAmount;

                    OrdernetByInstallments = TotalAmount / 4;
                    //TotalAmount += DeliveryCost + tax ;


                }
            };
            var CouponSerialdata = HttpContext.Session.GetString("newCouponSerial");
            newserial = CouponSerialdata;



            PaymentId = perfumeContext.paymentMehods.FirstOrDefault().PaymentMethodId;
            FattorahPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 1).FirstOrDefault().PaymentMethodId;
            CahshPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 2).FirstOrDefault().PaymentMethodId;
            TabbyPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 3).FirstOrDefault().PaymentMethodId;

            paymentMehods = perfumeContext.paymentMehods.ToList();

            coupon = perfumeContext.Coupon.Where(i => i.Serial == newserial).FirstOrDefault();


            GetDiscountFromCoupon(TotalAmount, coupon);


            return Page();
        }



        public async Task<IActionResult> OnPostAddCustomerAddress(string Serial, int orderId)
        {
            try
            {
                var orders = perfumeContext.Order.Include(e => e.OrderItem).Where(e => e.OrderId == orderId).FirstOrDefault();
                string serialParameter = Serial != null ? "&Serial=" + Serial.ToString() : "";
                orders.PaymentMethodId = PaymentId;
                var UpdatedOrder = perfumeContext.Order.Attach(orders);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                perfumeContext.SaveChanges();
                var Country = HttpContext.Session.GetString("country");
                var CountryObj = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault();
                string currencyISO = perfumeContext.Currency.Where(i => i.CurrencyId == CountryObj.CurrencyId).FirstOrDefault().CurrencyTlen;
                shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();

                var customer = perfumeContext.CustomerNs.Where(e => e.CustomerId == orders.CustomerId).FirstOrDefault();

                double coupondiscount = 0;
                double total = 0;
                var Countryobj = perfumeContext.Country.Where(e => e.CountryId == orders.CountryId).FirstOrDefault();
                var orderTotalbyTax = (orders.OrderTotal * Countryobj.tax) + orders.OrderTotal;
                var TotalWeight = orders.TotalWeight;
                foreach (var shipCost in shippingMatrices)
                {
                    if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                    {
                        DeliveryCost = shipCost.ActualPrice;
                    }

                }
                var shippingCost = Countryobj.ShippingCost + DeliveryCost;

                total = (orderTotalbyTax.Value + shippingCost.Value);
                if (orders.CouponTypeId == 2)
                {
                    var coupon = perfumeContext.Coupon.Where(e => e.CouponId == orders.CouponId).FirstOrDefault().Amount;

                    total = total - coupon.Value;
                }
                if (orders.CouponTypeId == 1)
                {
                    var coupon = perfumeContext.Coupon.Where(e => e.CouponId == orders.CouponId).FirstOrDefault().Amount;

                    total = total - (total * (coupon.Value / 100));
                }

                var orderItemsCount = orders.OrderItem.Count();
                var orderItemsCountByDiscount = total / orderItemsCount;
                //orderItemsCountByDiscount= Math.Round((double)(orderItemsCountByDiscount), 2);
                if (PaymentId == 1) // My Fatoorah
                {
                    var orderItemsList = perfumeContext.OrderItem.Include(e => e.Item).Where(e => e.OrderId == orders.OrderId).ToList();
                    List<OrderItems> SendorderItems = new List<OrderItems>();

                    foreach (var item in orderItemsList)
                    {
                        if (item.ItemQuantity > 1)
                        {
                            for (int i = 0; i < item.ItemQuantity; i++)
                            {
                                var orderItemobj = new OrderItems
                                {
                                    ItemName = perfumeContext.Item.Where(e => e.ItemId == item.ItemId).FirstOrDefault().ItemTitleEn,
                                    UnitPrice = orderItemsCountByDiscount / item.ItemQuantity /*Math.Round((double)(orderItemsCountByDiscount / item.ItemQuantity), 2)*/,

                                    Quantity = 1,

                                };
                                SendorderItems.Add(orderItemobj);
                            }
                        }
                        else
                        {
                            var orderItemobj = new OrderItems
                            {
                                ItemName = perfumeContext.Item.Where(e => e.ItemId == item.ItemId).FirstOrDefault().ItemTitleEn,
                                UnitPrice = (orderItemsCountByDiscount),

                                Quantity = item.ItemQuantity,

                            };
                            SendorderItems.Add(orderItemobj);
                        }

                        //var itemRound = Math.Round((double) (item.Total), 2);
                        //var unitPrice = Math.Round((double)(itemRound + orderItemsCountByDiscount), 2);


                    }

                    if (int.Parse(Country) == 2)
                    {
                        bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                        var TestToken = _configuration["Test_All_Saudi"];
                        var LiveToken = _configuration["Live_All_Saudi"];
                        if (Fattorahstatus) // fattorah Saudi live
                        {
                            var sendPaymentRequest = new
                            {
                                CustomerName = customer.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = orders.OrderNet, // Order Total + Delivery
                                InvoiceAmount = orders.OrderTotal,

                                CustomerReference = orders.OrderId,
                                CallBackUrl = "https://mashaer.store/fattorahsucess",// Success Page
                                ErrorUrl = "https://mashaer.store/FattorahError",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customer.Email,//Customer Email
                                DisplayCurrencyIso = "SAR",
                                InvoiceItems = SendorderItems
                            };

                            var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                            string url = "https://api-sa.myfatoorah.com/v2/SendPayment";
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                            var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                            var responseMessage = httpClient.PostAsync(url, httpContent);
                            var res = await responseMessage.Result.Content.ReadAsStringAsync();
                            var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                            if (FattoraRes.IsSuccess == true)
                            {
                                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                                var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                                //var CustShoppingCart = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();
                                //perfumeContext.Remove(CustShoppingCart);
                                //perfumeContext.SaveChanges();
                                return Redirect(InvoiceRes.InvoiceURL);

                            }
                            else
                            {
                                return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                            }
                        }
                        else               // fattorah Saudi test
                        {

                            var sendPaymentRequest = new
                            {

                                CustomerName = customer.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = orders.OrderNet, // Order Total + Delivery
                                CustomerReference = orders.OrderId,
                                CallBackUrl = "https://mashaer.store/fattorahsucess",// Success Page
                                ErrorUrl = "https://mashaer.store/FattorahError",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customer.Email, //Customer Email
                                DisplayCurrencyIso = "SAR",
                                InvoiceItems = SendorderItems
                            };

                            var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                            string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                            var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                            var responseMessage = httpClient.PostAsync(url, httpContent);
                            var res = await responseMessage.Result.Content.ReadAsStringAsync();
                            var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                            if (FattoraRes.IsSuccess == true)
                            {
                                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                                var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();

                                return Redirect(InvoiceRes.InvoiceURL);

                            }
                            else
                            {
                                return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                            }
                        }
                    }
                    else
                    {
                        bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                        var TestToken = _configuration["Test_All_Country"];
                        var LiveToken = _configuration["Live_All_Country"];
                        if (Fattorahstatus) // fattorah All countries live
                        {
                            var sendPaymentRequest = new
                            {
                                CustomerName = customer.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = orders.OrderNet, // Order Total + Delivery
                                CustomerReference = orders.OrderId,
                                CallBackUrl = "https://mashaer.store/AllCountriesCallBack",// Success Page
                                ErrorUrl = "https://mashaer.store/AllCountryfattoraherror",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customer.Email,
                                DisplayCurrencyIso = currencyISO,
                                InvoiceItems = SendorderItems
                            };

                            var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                            string url = "https://api.myfatoorah.com/v2/SendPayment";
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                            var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                            var responseMessage = httpClient.PostAsync(url, httpContent);
                            var res = await responseMessage.Result.Content.ReadAsStringAsync();
                            var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                            if (FattoraRes.IsSuccess == true)
                            {
                                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                                var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                                //var CustShoppingCart = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();
                                //perfumeContext.Remove(CustShoppingCart);
                                //perfumeContext.SaveChanges();
                                return Redirect(InvoiceRes.InvoiceURL);

                            }
                            else
                            {
                                return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                            }
                        }
                        else               // fattorah All countries test
                        {

                            var sendPaymentRequest = new
                            {

                                CustomerName = customer.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = orders.OrderNet, // Order Total + Delivery
                                CustomerReference = orders.OrderId,
                                CallBackUrl = "https://mashaer.store/AllCountriesCallBack",// Success Page
                                ErrorUrl = "https://mashaer.store/AllCountryfattoraherror",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customer.Email, //Customer Email
                                DisplayCurrencyIso = currencyISO,
                                InvoiceItems = SendorderItems
                            };

                            var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                            string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                            var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                            var responseMessage = httpClient.PostAsync(url, httpContent);
                            var res = await responseMessage.Result.Content.ReadAsStringAsync();
                            var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                            if (FattoraRes.IsSuccess == true)
                            {
                                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                                var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();

                                return Redirect(InvoiceRes.InvoiceURL);

                            }
                            else
                            {
                                return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                            }
                        }
                    }
                    //var orderId = perfumeContext.Order.Where(e => e.CustomerId == customerObj.CustomerId).FirstOrDefault().OrderId;




                }

                else if (PaymentId == 2) //Cash On Delivery --> Not Exists on This Applicaiton
                {
                    var Cost = perfumeContext.Country.Where(e => e.CountryId == orders.CountryId).FirstOrDefault().ShippingCost;
                    var Customer = perfumeContext.CustomerNs.Where(e => e.CustomerId == orders.CustomerId).FirstOrDefault();
                    if (Customer != null)
                    {
                        var carts = perfumeContext.ShoppingCart.Where(e => e.CustomerId == orders.CustomerId);
                        perfumeContext.ShoppingCart.RemoveRange(carts);

                    }

                    perfumeContext.SaveChanges();
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
                        Tax = i.tax.HasValue ? i.tax.Value : 0,
                        Discount = i.OrderDiscount,
                        InvoiceNumber = i.UniqeId.Value,
                        WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                        SuppEmail = perfumeContext.SocialMediaLinks.FirstOrDefault().ContactMail,
                        ConntactNumber = perfumeContext.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                        ShippingCost = i.Deliverycost.HasValue ? i.Deliverycost.Value : 0,
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


                    return RedirectToPage("/Thankyou", new { orderId = orders.OrderId });
                }

                else if (PaymentId == 3) //Tabby
                {
                    var testtoken = "pk_1f548018-c867-4f5c-b7e2-add860900eab";
                    //var testtokenUAE = "pk_test_40574f63-09c5-453d-b904-0b192ea2a14d";
                    var testtokenUAE = "pk_1f548018-c867-4f5c-b7e2-add860900eab";
                    var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                    var BrowserCulture = locale.RequestCulture.UICulture.ToString();

                    string formattedDate = orders.OrderDate.ToString("yyyy-MM-dd");
                    string updatedDate = orders.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    var country = perfumeContext.Country.FirstOrDefault(c => c.CountryId == int.Parse(Country));
                    var currencyEN = perfumeContext.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlen;
                    var currencyAR = perfumeContext.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlar;
                    var phoneCustomer = customer.Phone.Split("+")[1];
                    var customerAddress = perfumeContext.customerAddresses.FirstOrDefault(c => c.CustomerAddressId == orders.CustomerAddressId);
                    var orderItemsList = perfumeContext.OrderItem.Include(e => e.Item).Where(e => e.OrderId == orders.OrderId).ToList();
                    List<TabbyOrderITems> SendorderItems = new List<TabbyOrderITems>();
                    List<TabbyOrderHistory> SendOrderHistory = new List<TabbyOrderHistory>();

                    var customerOrderHistory = perfumeContext.CustomerNs.Where(e => e.CustomerId == orders.CustomerId).FirstOrDefault();
                    var customerpayment = perfumeContext.Order.Where(e => e.CustomerId == customerOrderHistory.CustomerId && e.PaymentMethodId == 3).ToList();
                    var orderHistory = perfumeContext.OrderItem.Include(e => e.Order).ThenInclude(e => e.CustomerAddress).ThenInclude(e => e.CustomerN).Where(e => e.Order.CustomerId == customerOrderHistory.CustomerId && e.Order.PaymentMethodId == 3).ToList();
                    foreach (var item in orderItemsList)
                    {

                        var orderItemobj = new TabbyOrderITems
                        {
                            title = perfumeContext.Item.Where(e => e.ItemId == item.ItemId).FirstOrDefault().ItemTitleEn,
                            unit_price = item.ItemPrice /*Math.Round((double)(orderItemsCountByDiscount / item.ItemQuantity), 2)*/,
                            description = perfumeContext.Item.Where(e => e.ItemId == item.ItemId).FirstOrDefault().ItemDescriptionEn,
                            quantity = item.ItemQuantity,
                            reference_id = item.ItemId.ToString()
                        };
                        SendorderItems.Add(orderItemobj);
                    }
                    foreach (var orderhistor in orderHistory)
                    {
                        string orderformattedDate = orderhistor.Order.OrderDate.ToString("yyyy-MM-dd");
                        var phoneCustomerAdd = orderhistor.Order.CustomerAddress.CustomerN.Phone.Split("+")[1];
                        string purchasedatDate = orderhistor.Order.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        var buyerobj = new buyer
                        {
                            email = orderhistor.Order.CustomerAddress.CustomerN.Email,
                            name = orderhistor.Order.CustomerAddress.CustomerN.CustomerName,
                            phone = phoneCustomerAdd,
                            dob = orderformattedDate
                        };
                        var shippingobj = new shipping_addressTabby
                        {
                            address = orderhistor.Order.CustomerAddress.Address,
                            city = orderhistor.Order.CustomerAddress.CityName,
                            zip = orderhistor.Order.CustomerAddress.ZIPCode,
                        };
                        var orderHistoryobj = new TabbyOrderHistory
                        {
                            amount = Math.Round((double)orders.OrderNet, 2),
                            payment_method = "card",
                            purchased_at = purchasedatDate,
                            status = "new",
                            buyer = buyerobj,
                            items = SendorderItems,
                            shipping_address = shippingobj
                        };
                        SendOrderHistory.Add(orderHistoryobj);
                    }
                    if (int.Parse(Country) == 2)
                    {

                        var sendPaymentRequest = new
                        {
                            payment = new
                            {
                                amount = Math.Round((double)orders.OrderNet, 2),
                                currency = "SAR",
                                description = "Perfume",
                                buyer = new
                                {
                                    id = customer.CustomerId.ToString(),
                                    phone = phoneCustomer,
                                    email = customer.Email,
                                    name = customer.CustomerName,
                                    dob = formattedDate
                                },
                                buyer_history = new
                                {
                                    registered_since = updatedDate,
                                    loyalty_level = 0,
                                    wishlist_count = 0,
                                    is_social_networks_connected = true,
                                    is_phone_number_verified = true,
                                    is_email_verified = true
                                },
                                order = new
                                {
                                    tax_amount = orders.tax,
                                    shipping_amount = orders.Deliverycost,
                                    discount_amount = orders.DiscountAmount + orders.OrderDiscount,
                                    updated_at = updatedDate,
                                    reference_id = orders.OrderId.ToString(),
                                    items = SendorderItems
                                },

                                order_history = SendOrderHistory,



                                shipping_address = new
                                {
                                    city = customerAddress.CityName,
                                    address = customerAddress.Address,
                                    zip = customerAddress.ZIPCode,
                                },

                                meta = new
                                {
                                    order_id = orders.OrderId,
                                    customer = customer.CustomerId
                                }
                            },
                            lang = "en",
                            merchant_code = "MKSA",
                            merchant_urls = new
                            {
                                success = "https://mashaer.store/TabbySuccess",
                                cancel = "https://mashaer.store/TabbyCancel",
                                failure = "https://mashaer.store/TabbyFailed"
                            },
                            create_token = false
                        };

                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://api.tabby.ai/api/v2/checkout";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        orders.Tabbyres = res;
                        perfumeContext.Attach(orders).State = EntityState.Modified;
                        var UpdatedAsset = perfumeContext.Order.Attach(orders);
                        UpdatedAsset.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();
                        var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);

                        if (tabbyRes != null)
                        {
                            if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                            {
                                if (tabbyRes.Configuration.available_products.Installments[0].web_url != null)
                                {


                                    var webUrl = tabbyRes.Configuration.available_products.Installments[0].web_url;
                                    if (tabbyRes.Payment.Id != null)
                                    {
                                        var paymentId = tabbyRes.Payment.Id;
                                        orders.TabbyPaymentId = tabbyRes.Payment.Id;
                                        var Updatedorder = perfumeContext.Order.Attach(orders);
                                        Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                        perfumeContext.SaveChanges();
                                    }
                                    return Redirect(webUrl);
                                }
                            }
                            //if (tabbyRes.Payment.Id != null)
                            //{
                            //    orders.TabbyPaymentId = tabbyRes.Payment.Id;
                            //    var Updatedorder = perfumeContext.Order.Attach(orders);
                            //    Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            //    perfumeContext.SaveChanges();
                            //}


                            //if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                            //{
                            //    var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            //    var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            //    var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            //    IsInstallment = true;
                            //    var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //    //List<Installment> installments = installmentInfo.Installments;
                            //    return new JsonResult(result);
                            //}
                            else if (tabbyRes.status == "rejected" || tabbyRes.status == "expired")
                            {
                                var rejectionReasonText = tabbyRes.Configuration.Products.Installments.rejection_reason;
                                if (rejectionReasonText != null)
                                {
                                    if (rejectionReasonText == "not_available")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "Sorry, Tabby is unable to approve this purchase. Please use an alternative payment method for your order.";

                                        }
                                        else
                                        {
                                            rejectionReason = "نأسف، تابي غير قادرة على الموافقة على هذه العملية. الرجاء استخدام طريقة دفع أخرى.";

                                        }
                                    }
                                    else if (rejectionReasonText == "order_amount_too_high")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "This purchase is above your current spending limit with Tabby, try a smaller cart or use another payment method";
                                        }
                                        else
                                        {
                                            rejectionReason = "هذه الشراء تجاوزت حد الإنفاق الحالي مع تابي. حاول عربة أصغر أو استخدم وسيلة دفع أخرى.";

                                        }
                                    }
                                    else if (rejectionReasonText == "order_amount_too_low")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "The purchase amount is below the minimum amount required to use Tabby, try adding more items or use another payment method";
                                        }
                                        else
                                        {
                                            rejectionReason = "مبلغ الشراء أقل من الحد الأدنى المطلوب لاستخدام تابي. حاول إضافة المزيد من العناصر أو استخدم وسيلة دفع أخرى.";

                                        }
                                    }
                                }
                                //var result = new { rejectionReason, IsInstallment };
                                ////List<Installment> installments = installmentInfo.Installments;
                                //return new JsonResult(result);
                                return Redirect("/CheckOutPayment?orderId=" + orderId + serialParameter);
                            }
                        }




                    }

                    else if (int.Parse(Country) == 4) ////UAE 
                    {
                        var sendPaymentRequest = new
                        {
                            payment = new
                            {
                                amount = Math.Round((double)orders.OrderNet, 2),
                                currency = "AED",
                                description = "Perfume",

                                buyer = new
                                {
                                    id = customer.CustomerId.ToString(),
                                    phone = phoneCustomer,
                                    email = customer.Email,
                                    name = customer.CustomerName,
                                    dob = formattedDate
                                },
                                buyer_history = new
                                {
                                    registered_since = updatedDate,
                                    loyalty_level = 0,
                                    wishlist_count = 0,
                                    is_social_networks_connected = true,
                                    is_phone_number_verified = true,
                                    is_email_verified = true
                                },
                                order = new
                                {
                                    tax_amount = orders.tax,
                                    shipping_amount = orders.Deliverycost,
                                    discount_amount = orders.DiscountAmount + orders.OrderDiscount,
                                    updated_at = updatedDate,
                                    reference_id = orders.OrderId.ToString(),
                                    items = SendorderItems
                                },

                                order_history = SendOrderHistory,



                                shipping_address = new
                                {
                                    city = customerAddress.CityName,
                                    address = customerAddress.Address,
                                    zip = customerAddress.ZIPCode,
                                },

                                meta = new
                                {
                                    order_id = orders.OrderId,
                                    customer = customer.CustomerId
                                }
                            },
                            lang = "en",

                            merchant_code = "MUAE",
                            merchant_urls = new
                            {
                                success = "https://mashaer.store/TabbySuccess",
                                cancel = "https://mashaer.store/TabbyCancel",
                                failure = "https://mashaer.store/TabbyFailed"
                            },
                            create_token = false
                        };

                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://api.tabby.ai/api/v2/checkout";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtokenUAE);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        orders.Tabbyres = res;
                        perfumeContext.Attach(orders).State = EntityState.Modified;
                        var UpdatedAsset = perfumeContext.Order.Attach(orders);
                        UpdatedAsset.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();
                        var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);
                        if (tabbyRes != null)
                        {
                            if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                            {
                                if (tabbyRes.Configuration.available_products.Installments[0].web_url != null)
                                {


                                    var webUrl = tabbyRes.Configuration.available_products.Installments[0].web_url;
                                    if (tabbyRes.Payment.Id != null)
                                    {
                                        var paymentId = tabbyRes.Payment.Id;
                                        orders.TabbyPaymentId = tabbyRes.Payment.Id;
                                        var Updatedorder = perfumeContext.Order.Attach(orders);
                                        Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                        perfumeContext.SaveChanges();
                                    }
                                    return Redirect(webUrl);
                                }
                            }
                            //if (tabbyRes.Payment.Id != null)
                            //{
                            //    orders.TabbyPaymentId = tabbyRes.Payment.Id;
                            //    var Updatedorder = perfumeContext.Order.Attach(orders);
                            //    Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            //    perfumeContext.SaveChanges();
                            //}


                            //if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                            //{
                            //    var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            //    var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            //    var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            //    IsInstallment = true;
                            //    var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //    //List<Installment> installments = installmentInfo.Installments;
                            //    return new JsonResult(result);
                            //}
                            else if (tabbyRes.status == "rejected" || tabbyRes.status == "expired")
                            {
                                var rejectionReasonText = tabbyRes.Configuration.Products.Installments.rejection_reason;
                                if (rejectionReasonText != null)
                                {
                                    if (rejectionReasonText == "not_available")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "Sorry, Tabby is unable to approve this purchase. Please use an alternative payment method for your order.";

                                        }
                                        else
                                        {
                                            rejectionReason = "نأسف، تابي غير قادرة على الموافقة على هذه العملية. الرجاء استخدام طريقة دفع أخرى.";

                                        }
                                    }
                                    else if (rejectionReasonText == "order_amount_too_high")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "This purchase is above your current spending limit with Tabby, try a smaller cart or use another payment method";
                                        }
                                        else
                                        {
                                            rejectionReason = "هذه الشراء تجاوزت حد الإنفاق الحالي مع تابي. حاول عربة أصغر أو استخدم وسيلة دفع أخرى.";

                                        }
                                    }
                                    else if (rejectionReasonText == "order_amount_too_low")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "The purchase amount is below the minimum amount required to use Tabby, try adding more items or use another payment method";
                                        }
                                        else
                                        {
                                            rejectionReason = "مبلغ الشراء أقل من الحد الأدنى المطلوب لاستخدام تابي. حاول إضافة المزيد من العناصر أو استخدم وسيلة دفع أخرى.";

                                        }
                                    }
                                }
                                //var result = new { rejectionReason, IsInstallment };
                                ////List<Installment> installments = installmentInfo.Installments;
                                //return new JsonResult(result);
                                return Redirect("/CheckOutPayment?orderId=" + orderId + serialParameter);
                            }
                        }

                    }
                    else if (int.Parse(Country) == 1)
                    {
                        var sendPaymentRequest = new
                        {
                            payment = new
                            {
                                amount = Math.Round((double)orders.OrderNet, 2),
                                currency = "KWD",
                                description = "Perfume",

                                buyer = new
                                {
                                    id = customer.CustomerId.ToString(),
                                    phone = phoneCustomer,
                                    email = customer.Email,
                                    name = customer.CustomerName,
                                    dob = formattedDate
                                },
                                buyer_history = new
                                {
                                    registered_since = updatedDate,
                                    loyalty_level = 0,
                                    wishlist_count = 0,
                                    is_social_networks_connected = true,
                                    is_phone_number_verified = true,
                                    is_email_verified = true
                                },
                                order = new
                                {
                                    tax_amount = orders.tax,
                                    shipping_amount = orders.Deliverycost,
                                    discount_amount = orders.DiscountAmount + orders.OrderDiscount,
                                    updated_at = updatedDate,
                                    reference_id = orders.OrderId.ToString(),
                                    items = SendorderItems
                                },

                                order_history = SendOrderHistory,



                                shipping_address = new
                                {
                                    city = customerAddress.CityName,
                                    address = customerAddress.Address,
                                    zip = customerAddress.ZIPCode,
                                },

                                meta = new
                                {
                                    order_id = orders.OrderId,
                                    customer = customer.CustomerId
                                }
                            },
                            lang = "en",

                            merchant_code = "MKWT",
                            merchant_urls = new
                            {
                                success = "https://mashaer.store/TabbySuccess",
                                cancel = "https://mashaer.store/TabbyCancel",
                                failure = "https://mashaer.store/TabbyFailed"
                            },
                            create_token = false
                        };

                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://api.tabby.ai/api/v2/checkout";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        orders.Tabbyres = res;
                        perfumeContext.Attach(orders).State = EntityState.Modified;
                        var UpdatedAsset = perfumeContext.Order.Attach(orders);
                        UpdatedAsset.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();
                        var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);
                        if (tabbyRes != null)
                        {
                            if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                            {
                                if (tabbyRes.Configuration.available_products.Installments[0].web_url != null)
                                {


                                    var webUrl = tabbyRes.Configuration.available_products.Installments[0].web_url;
                                    if (tabbyRes.Payment.Id != null)
                                    {
                                        var paymentId = tabbyRes.Payment.Id;
                                        orders.TabbyPaymentId = tabbyRes.Payment.Id;
                                        var Updatedorder = perfumeContext.Order.Attach(orders);
                                        Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                        perfumeContext.SaveChanges();
                                    }
                                    return Redirect(webUrl);
                                }
                            }
                            //if (tabbyRes.Payment.Id != null)
                            //{
                            //    orders.TabbyPaymentId = tabbyRes.Payment.Id;
                            //    var Updatedorder = perfumeContext.Order.Attach(orders);
                            //    Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            //    perfumeContext.SaveChanges();
                            //}


                            //if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                            //{
                            //    var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            //    var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            //    var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            //    IsInstallment = true;
                            //    var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //    //List<Installment> installments = installmentInfo.Installments;
                            //    return new JsonResult(result);
                            //}
                            else if (tabbyRes.status == "rejected" || tabbyRes.status == "expired")
                            {
                                var rejectionReasonText = tabbyRes.Configuration.Products.Installments.rejection_reason;
                                if (rejectionReasonText != null)
                                {
                                    if (rejectionReasonText == "not_available")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "Sorry, Tabby is unable to approve this purchase. Please use an alternative payment method for your order.";

                                        }
                                        else
                                        {
                                            rejectionReason = "نأسف، تابي غير قادرة على الموافقة على هذه العملية. الرجاء استخدام طريقة دفع أخرى.";

                                        }
                                    }
                                    else if (rejectionReasonText == "order_amount_too_high")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "This purchase is above your current spending limit with Tabby, try a smaller cart or use another payment method";
                                        }
                                        else
                                        {
                                            rejectionReason = "هذه الشراء تجاوزت حد الإنفاق الحالي مع تابي. حاول عربة أصغر أو استخدم وسيلة دفع أخرى.";

                                        }
                                    }
                                    else if (rejectionReasonText == "order_amount_too_low")
                                    {
                                        if (BrowserCulture == "en-US")
                                        {
                                            rejectionReason = "The purchase amount is below the minimum amount required to use Tabby, try adding more items or use another payment method";
                                        }
                                        else
                                        {
                                            rejectionReason = "مبلغ الشراء أقل من الحد الأدنى المطلوب لاستخدام تابي. حاول إضافة المزيد من العناصر أو استخدم وسيلة دفع أخرى.";

                                        }
                                    }
                                }
                                //var result = new { rejectionReason, IsInstallment };
                                ////List<Installment> installments = installmentInfo.Installments;
                                //return new JsonResult(result);
                                return Redirect("/CheckOutPayment?orderId=" + orderId + serialParameter);
                            }
                        }

                    }
                }


                return Redirect("/CheckOutPayment?orderId=" + orderId + serialParameter);
            }







            catch (Exception ex)
            {
                toastNotification.AddErrorToastMessage("Something went wrong");
                string serialParameter = Serial != null ? "&Serial=" + Serial.ToString() : "";
                return Redirect("/CheckOutPayment?orderId=" + orderId + serialParameter);
            }

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
                TotalAmountAfterDiscount = /*AmountAfterDiscount*/ Math.Round(AmountAfterDiscount, 2);
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



        public string GetUserIpAddress()
        {
            string Ip = Response.HttpContext.Connection.RemoteIpAddress.ToString();

            if (Ip == "::1")
            {
                Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            }
            return Ip;
        }

        public async Task<IActionResult> OnPostTabbyCheckOut(int orderId)
        {
            var orders = perfumeContext.Order.Where(e => e.OrderId == orderId).FirstOrDefault();
            var Country = HttpContext.Session.GetString("country");
            var testtoken = "pk_test_84296aaa-587e-4e91-b6d2-aa7fbf9ce182";
            string formattedDate = orders.OrderDate.ToString("yyyy-MM-dd");
            string updatedDate = orders.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var country = perfumeContext.Country.FirstOrDefault(c => c.CountryId == int.Parse(Country));
            var currencyEN = perfumeContext.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlen;
            var currencyAR = perfumeContext.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlar;
            var customer = perfumeContext.CustomerNs.FirstOrDefault(c => c.CustomerId == orders.CustomerId);
            var phoneCustomer = customer.Phone.Split("+")[1];
            var customerAddress = perfumeContext.customerAddresses.FirstOrDefault(c => c.CustomerAddressId == orders.CustomerAddressId);
            var orderItemsList = perfumeContext.OrderItem.Include(e => e.Item).Where(e => e.OrderId == orders.OrderId).ToList();
            List<TabbyOrderITems> SendorderItems = new List<TabbyOrderITems>();

            foreach (var item in orderItemsList)
            {

                var orderItemobj = new TabbyOrderITems
                {
                    title = perfumeContext.Item.Where(e => e.ItemId == item.ItemId).FirstOrDefault().ItemTitleEn,
                    unit_price = item.ItemPrice /*Math.Round((double)(orderItemsCountByDiscount / item.ItemQuantity), 2)*/,
                    description = perfumeContext.Item.Where(e => e.ItemId == item.ItemId).FirstOrDefault().ItemDescriptionEn,
                    quantity = item.ItemQuantity,

                };
                SendorderItems.Add(orderItemobj);
            }

        
                if (int.Parse(Country) == 2)
            {

                var sendPaymentRequest = new
                {
                    payment = new
                    {
                        amount = Math.Round((double)orders.OrderNet, 2),
                        currency = "SAR",
                        description = "string",
                        buyer=new {
                           phone= phoneCustomer, 
                  email= "card.success @tabby.ai", 
                  name= customer.CustomerName, 
                    dob= formattedDate
    },
                       order = new
                        {
                            tax_amount = orders.tax,
                            shipping_amount = orders.Deliverycost,
                            discount_amount = orders.DiscountAmount + orders.OrderDiscount,
                            updated_at = updatedDate,
                            reference_id = orders.OrderId.ToString(),
                            items = SendorderItems
                       },
                //buyer_history = new
                //{
                //    registered_since = orders.OrderDate,
                //    loyalty_level = 0,
                //    wishlist_count = 0,
                //    is_social_networks_connected = true,
                //    is_phone_number_verified = true,
                //    is_email_verified = true
                //},

                //                            order_history = new[]
                //{
                //    new
                //    {
                //        purchased_at = "2019-08-24T14:15:22Z",
                //        amount = "0.00",
                //        payment_method = "card",
                //        status = "new",
                //        buyer = new
                //        {
                //            phone = "string",
                //            email = "user@example.com",
                //            name = "string",
                //            dob = "2019-08-24"
                //        },
                //        shipping_address = new
                //        {
                //            city = "string",
                //            address = "string",
                //            zip = "string"
                //        },
                //        items = new[]
                //        {
                //            new
                //            {
                //                title = "string",
                //                description = "string",
                //                quantity = 1,
                //                unit_price = "0.00",
                //                discount_amount = "0.00",
                //                reference_id = "string",
                //                image_url = "http://example.com",
                //                product_url = "http://example.com",
                //                ordered = 0,
                //                captured = 0,
                //                shipped = 0,
                //                refunded = 0,
                //                gender = "Male",
                //                category = "string",
                //                color = "string",
                //                product_material = "string",
                //                size_type = "string",
                //                size = "string",
                //                brand = "string"
                //            }
                //        }
                //    }
                //},


                //attachment = new
                //{
                //    body = "{\"flight_reservation_details\": {\"pnr\": \"TR9088999\",\"itinerary\": [...],\"insurance\": [...],\"passengers\": [...],\"affiliate_name\": \"some affiliate\"}}",
                //    content_type = "application/vnd.tabby.v1+json"
                //}
            
                     shippingAddress = new
                    {
                        city = customerAddress.CityName,
                        address = customerAddress.Address,
                      
                    },

                 meta = new
                {
                    order_id = orders.OrderId,
                    customer = customer.CustomerId
                }
                },
                lang = "en",
                    merchant_code = "MKSA",
                    merchant_urls = new
                    {
                        success = "https://mashaer.store/TabbySuccess",
                        cancel = "https://mashaer.store/TabbyFailed",
                        failure = "https://mashaer.store/TabbyFailed"
                    },
                    create_token = false
                };

                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                string url = "https://api.tabby.ai/api/v2/checkout";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                var responseMessage = httpClient.PostAsync(url, httpContent);
                var res = await responseMessage.Result.Content.ReadAsStringAsync();
                orders.Tabbyres = res;
                perfumeContext.Attach(orders).State = EntityState.Modified;
                var UpdatedAsset = perfumeContext.Order.Attach(orders);
                UpdatedAsset.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                perfumeContext.SaveChanges();
                var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);

                if (tabbyRes != null)
                {
                    if (tabbyRes.Payment.Id != null)
                    {
                        orders.TabbyPaymentId = tabbyRes.Payment.Id;
                        var Updatedorder = perfumeContext.Order.Attach(orders);
                        Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();
                    }

                    if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                    {
                        if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                        {
                            var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            IsInstallment = true;
                            var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //List<Installment> installments = installmentInfo.Installments;
                            return new JsonResult(result);
                        }

                    }
                    else if (tabbyRes.status == "rejected" || tabbyRes.status == "expired")
                    {
                        var rejectionReason = tabbyRes.Configuration.Products.Installments.rejection_reason;

                        var result = new { rejectionReason, IsInstallment };
                        //List<Installment> installments = installmentInfo.Installments;
                        return new JsonResult(result);
                    }



                }
            }
            else if (int.Parse(Country) == 4) ////UAE 
            {
                var sendPaymentRequest = new
                {
                    payment = new
                    {
                        amount = Math.Round((double)orders.OrderNet, 2),
                        currency = "AED",
                        description = "Perfume",

                        buyer = new
                        {
                            phone = phoneCustomer,
                            email = "card.success @tabby.ai",
                            name = customer.CustomerName,
                            dob = formattedDate
                        },
                        order = new
                        {
                            tax_amount = orders.tax,
                            shipping_amount = orders.Deliverycost,
                            discount_amount = orders.DiscountAmount + orders.OrderDiscount,
                            updated_at = updatedDate,
                            reference_id = orders.OrderId.ToString(),
                            items = SendorderItems
                        },
                        //buyer_history = new
                        //{
                        //    registered_since = orders.OrderDate,
                        //    loyalty_level = 0,
                        //    wishlist_count = 0,
                        //    is_social_networks_connected = true,
                        //    is_phone_number_verified = true,
                        //    is_email_verified = true
                        //},

                        //                            order_history = new[]
                        //{
                        //    new
                        //    {
                        //        purchased_at = "2019-08-24T14:15:22Z",
                        //        amount = "0.00",
                        //        payment_method = "card",
                        //        status = "new",
                        //        buyer = new
                        //        {
                        //            phone = "string",
                        //            email = "user@example.com",
                        //            name = "string",
                        //            dob = "2019-08-24"
                        //        },
                        //        shipping_address = new
                        //        {
                        //            city = "string",
                        //            address = "string",
                        //            zip = "string"
                        //        },
                        //        items = new[]
                        //        {
                        //            new
                        //            {
                        //                title = "string",
                        //                description = "string",
                        //                quantity = 1,
                        //                unit_price = "0.00",
                        //                discount_amount = "0.00",
                        //                reference_id = "string",
                        //                image_url = "http://example.com",
                        //                product_url = "http://example.com",
                        //                ordered = 0,
                        //                captured = 0,
                        //                shipped = 0,
                        //                refunded = 0,
                        //                gender = "Male",
                        //                category = "string",
                        //                color = "string",
                        //                product_material = "string",
                        //                size_type = "string",
                        //                size = "string",
                        //                brand = "string"
                        //            }
                        //        }
                        //    }
                        //},


                        //attachment = new
                        //{
                        //    body = "{\"flight_reservation_details\": {\"pnr\": \"TR9088999\",\"itinerary\": [...],\"insurance\": [...],\"passengers\": [...],\"affiliate_name\": \"some affiliate\"}}",
                        //    content_type = "application/vnd.tabby.v1+json"
                        //}
                    
                    shippingAddress = new
                    {
                        city = customerAddress.CityName,
                        address = customerAddress.Address,

                    },

                    meta = new
                    {
                        order_id = orders.OrderId,
                        customer = customer.CustomerId
                    }
                    },
                    lang = "en",

                    merchant_code = "MUAE",
                    merchant_urls = new
                    {
                        success = "https://mashaer.store/TabbySuccess",
                        cancel = "https://mashaer.store/TabbyFailed",
                        failure = "https://mashaer.store/TabbyFailed"
                    },
                    create_token = false
                };

                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                string url = "https://api.tabby.ai/api/v2/checkout";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                var responseMessage = httpClient.PostAsync(url, httpContent);
                var res = await responseMessage.Result.Content.ReadAsStringAsync();
                orders.Tabbyres = res;
                perfumeContext.Attach(orders).State = EntityState.Modified;
                var UpdatedAsset = perfumeContext.Order.Attach(orders);
                UpdatedAsset.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                perfumeContext.SaveChanges();
                var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);
                if (tabbyRes != null)
                {
                    if (tabbyRes.Payment.Id != null)
                    {
                        orders.TabbyPaymentId = tabbyRes.Payment.Id;
                        var Updatedorder = perfumeContext.Order.Attach(orders);
                        Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();
                    }
                    if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                    {
                        if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                        {
                            var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            IsInstallment = true;
                            var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //List<Installment> installments = installmentInfo.Installments;
                            return new JsonResult(result);
                        }

                    }
                    else if (tabbyRes.status == "rejected" || tabbyRes.status == "expired")
                    {
                        var rejectionReason = tabbyRes.Configuration.Products.Installments.rejection_reason;

                        var result = new { rejectionReason, IsInstallment };
                        //List<Installment> installments = installmentInfo.Installments;
                        return new JsonResult(result);
                    }



                }
            }
            else if (int.Parse(Country) == 1)
            {
                var sendPaymentRequest = new
                {
                    payment = new
                    {
                        amount = Math.Round((double)orders.OrderNet, 2),
                        currency = "KWD",
                        description = "Perfume",

                        buyer = new
                        {
                            phone = phoneCustomer,
                            email = "card.success @tabby.ai",
                            name = customer.CustomerName,
                            dob = formattedDate
                        },
                        order = new
                        {
                            tax_amount = orders.tax,
                            shipping_amount = orders.Deliverycost,
                            discount_amount = orders.DiscountAmount + orders.OrderDiscount,
                            updated_at = updatedDate,
                            reference_id = orders.OrderId.ToString(),
                            items = SendorderItems
                        },
                        //buyer_history = new
                        //{
                        //    registered_since = orders.OrderDate,
                        //    loyalty_level = 0,
                        //    wishlist_count = 0,
                        //    is_social_networks_connected = true,
                        //    is_phone_number_verified = true,
                        //    is_email_verified = true
                        //},

                        //                            order_history = new[]
                        //{
                        //    new
                        //    {
                        //        purchased_at = "2019-08-24T14:15:22Z",
                        //        amount = "0.00",
                        //        payment_method = "card",
                        //        status = "new",
                        //        buyer = new
                        //        {
                        //            phone = "string",
                        //            email = "user@example.com",
                        //            name = "string",
                        //            dob = "2019-08-24"
                        //        },
                        //        shipping_address = new
                        //        {
                        //            city = "string",
                        //            address = "string",
                        //            zip = "string"
                        //        },
                        //        items = new[]
                        //        {
                        //            new
                        //            {
                        //                title = "string",
                        //                description = "string",
                        //                quantity = 1,
                        //                unit_price = "0.00",
                        //                discount_amount = "0.00",
                        //                reference_id = "string",
                        //                image_url = "http://example.com",
                        //                product_url = "http://example.com",
                        //                ordered = 0,
                        //                captured = 0,
                        //                shipped = 0,
                        //                refunded = 0,
                        //                gender = "Male",
                        //                category = "string",
                        //                color = "string",
                        //                product_material = "string",
                        //                size_type = "string",
                        //                size = "string",
                        //                brand = "string"
                        //            }
                        //        }
                        //    }
                        //},


                        //attachment = new
                        //{
                        //    body = "{\"flight_reservation_details\": {\"pnr\": \"TR9088999\",\"itinerary\": [...],\"insurance\": [...],\"passengers\": [...],\"affiliate_name\": \"some affiliate\"}}",
                        //    content_type = "application/vnd.tabby.v1+json"
                        //}
                   
                    shippingAddress = new
                    {
                        city = customerAddress.CityName,
                        address = customerAddress.Address,

                    },

                    meta = new
                    {
                        order_id = orders.OrderId,
                        customer = customer.CustomerId
                    }
                    },
                    lang = "en",

                    merchant_code = "MKWT",
                    merchant_urls = new
                    {
                        success = "https://mashaer.store/TabbySuccess",
                        cancel = "https://mashaer.store/TabbyFailed",
                        failure = "https://mashaer.store/TabbyFailed"
                    },
                    create_token = false
                };

                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                string url = "https://api.tabby.ai/api/v2/checkout";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                var responseMessage = httpClient.PostAsync(url, httpContent);
                var res = await responseMessage.Result.Content.ReadAsStringAsync();
                orders.Tabbyres = res;
                perfumeContext.Attach(orders).State = EntityState.Modified;
                var UpdatedAsset = perfumeContext.Order.Attach(orders);
                UpdatedAsset.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                perfumeContext.SaveChanges();
                var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);
                if (tabbyRes != null)
                {
                    if (tabbyRes.Payment.Id != null)
                    {
                        orders.TabbyPaymentId = tabbyRes.Payment.Id;
                        var Updatedorder = perfumeContext.Order.Attach(orders);
                        Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        perfumeContext.SaveChanges();
                    }
                    if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                    {
                        if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                        {
                            var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            IsInstallment = true;
                            var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //List<Installment> installments = installmentInfo.Installments;
                            return new JsonResult(result);
                        }

                    }
                    else if ((tabbyRes.status == "rejected" || tabbyRes.status == "expired") && tabbyRes.Configuration.Products.Installments.rejection_reason != null)
                    {
                        var rejectionReason = tabbyRes.Configuration.Products.Installments.rejection_reason;

                        var result = new { rejectionReason, IsInstallment };
                        //List<Installment> installments = installmentInfo.Installments;
                        return new JsonResult(result);
                    }



                }
            }

            return new JsonResult(new { success = true, message = "TabbyCheckOut successful" });
        }
        public string GetUserCountryByIp(string IpAddress)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + IpAddress);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
                CurrencyNameEN = myRI1.ISOCurrencySymbol;
                CurrencyNameAr = myRI1.CurrencySymbol;


            }
            catch
            {
                CurrencyNameEN = "";
                CurrencyNameAr = "";
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }
    }
}
