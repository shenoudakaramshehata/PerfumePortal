using CRM.Controllers;
using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using CRM.ViewModels;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraRichEdit.Import.Html;
using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using NToastNotify;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace CRM.Pages
{
    public class CheckOutModel : PageModel
    {
        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailSender _emailSender;


        //[BindProperty(SupportsGet = true)]
        //public string CouponSerial { get; set; }
        public string url { get; set; }
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

        //[BindProperty]
        public int FattorahPaymentId { get; set; }

        public int CahshPaymentId { get; set; }
        public HttpClient httpClient { get; set; }

        public double Discount { get; set; }
        public double? ShippingCost { get; set; }

        public bool IsDiscounted { get; set; } = false;
        public double TotalAmountAfterDiscount { get; set; }

		public string CurrencyNameAr { get; set; }

		public string CurrencyNameEN { get; set; }
        public string CountryENName { get; set; }
        public string CountryARName { get; set; }
        public double DeliveryCost { get; set; }

        public string CountryToShow { get; set; }
        public int CountryId { get; set; }
        public List<ShippingCartObjectVM> shippingCartObjectVMs { get; set; }

        public List<ShippingMatrix> shippingMatrices { get; set; }
        public double TotalWeight { get; set; }
        public CheckOutModel(PerfumeContext perfumeContext, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IConfiguration configuration, IWebHostEnvironment hostEnvironment, IEmailSender emailSender)
        {
            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;
            this.userManager = userManager;
            this._configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            httpClient = new HttpClient();
            shippingCartObjectVMs = new List<ShippingCartObjectVM>();
        }

        public async Task<IActionResult> OnGet(string Serial)
        {
           
                var CouponSerialdata = HttpContext.Session.GetString("newCouponSerial");
                newserial = CouponSerialdata;
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {

                return RedirectToPage("/Login");
            }
            var Country = HttpContext.Session.GetString("country");
            if (Country != null)
            {
                CountryId = int.Parse(Country);
                DeliveryCost = perfumeContext.Country.Where(e => e.CountryId == CountryId).FirstOrDefault().ShippingCost.Value;

                CountryENName = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;
                CurrencyNameEN = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlen;
                CurrencyNameAr = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlar;

                shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();

            }

            PaymentId = perfumeContext.paymentMehods.FirstOrDefault().PaymentMethodId;
             FattorahPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 1).FirstOrDefault().PaymentMethodId;
             CahshPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 2).FirstOrDefault().PaymentMethodId;
            var customerId = perfumeContext.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;
            shoppingCarts = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item).Include(i => i.CustomerN).ToList();
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
                if (TotalWeight > shipCost.FromWeight && TotalWeight <= shipCost.ToWeight)
                {
                    DeliveryCost += shipCost.ActualPrice;
                }

            }
            TotalAmount = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);

            //var data = HttpContext.Session.GetString("parsecartItems");

            //if (data != null)
            //{
            //    //List<ShippingCartObjectVM> shippingCartObjectVMs = JsonSerializer.Deserialize<List<ShippingCartObjectVM>>(data);
            //    shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(data);

            //    foreach (var item in shippingCartObjectVMs)
            //    {
            //        ShippingCost = item.DeliveryCost;
            //        TotalAmount = TotalAmount + (item.priceCart * item.Qunatity);
            //        DeliveryCost = item.DeliveryCost;
            //    }
            //    TotalAmount += DeliveryCost;
            //}


            //var UserIpAddress = GetUserIpAddress();

            //var country = GetUserCountryByIp(UserIpAddress);

            //string CountryToUse;

            //if (country != null)
            //{
            //	var IsDbCountry = perfumeContext.Country.Any(i => i.CountryTlen == country);

            //             CountryToUse = country;

            //             if (!IsDbCountry)
            //	{
            //		var firstCountry = perfumeContext.Country.FirstOrDefault().CountryTlen;
            //		CountryToUse = firstCountry;

            //             }
            //}
            //else
            //{
            //	var firstCountry = perfumeContext.Country.FirstOrDefault().CountryTlen;
            //	CountryToUse = firstCountry;

            //}

            //         CountryToShow = CountryToUse ;

            //ShippingCost = perfumeContext.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().ShippingCost;

            //var dbCurrencyId = perfumeContext.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().CurrencyId;
            //var CurrencyName = perfumeContext.Currency.Where(i => i.CurrencyId == dbCurrencyId).FirstOrDefault();
            //CurrencyNameEN = CurrencyName.CurrencyTlen;
            //CurrencyNameAr = CurrencyName.CurrencyTlar;



            //shoppingCarts = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item).Include(i => i.CustomerN).ToList();

            //TotalAmount = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);

            paymentMehods = perfumeContext.paymentMehods.ToList();

            customerAddr = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerId).FirstOrDefault();

            //var DbCountry = perfumeContext.Country.ToList();

            //ViewData["selectCountryList"] = new SelectList(DbCountry, nameof(Country.CountryId), nameof(Country.CountryTlen));
            
            coupon = perfumeContext.Coupon.Where(i => i.Serial == newserial).FirstOrDefault();


            GetDiscountFromCoupon(TotalAmount, coupon);


            return Page();
        }

        public async Task<IActionResult> UpdatePayment(int Payment)
        {

            //Save Order and OrderItems --> Check sequence in in old api method 
            //Controller IntegrationController
            //[Route("CheckOut")]
            //public async Task<IActionResult> CheckOut(CRM.ViewModels.CheckOutVM checkOutVM)

            //Get Order record and Related information (Customer, Items, etc...)

            if (Payment == 1) // My Fatoorah
            {
                bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                var TestToken = _configuration["TestToken"];
                var LiveToken = _configuration["LiveToken"];
                if (Fattorahstatus) // fattorah live
                {
                    var sendPaymentRequest = new
                    {
                        CustomerName = "CustomerNameFromOrder", // Customer Name
                        NotificationOption = "LNK",//Fixed
                        InvoiceValue = 0.000, // Order Total + Delivery
                        CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderSuccess",// Success Page
                        ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderFaild",// Failed Page
                        UserDefinedField = 0, //Order ID
                        CustomerEmail = "" //Customer Email
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
                        return Redirect(InvoiceRes.InvoiceURL);

                    }
                    else
                    {
                        return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                    }
                }
                else               // fattorah test
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = "CustomerNameFromOrder", // Customer Name
                        NotificationOption = "LNK",//Fixed
                        InvoiceValue = 0.000, // Order Total + Delivery
                        CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderSuccess",// Success Page
                        ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderFaild",// Failed Page
                        UserDefinedField = 0, //Order ID
                        CustomerEmail = "" //Customer Email

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
            else if (Payment == 2) //Cash On Delivery --> Not Exists on This Applicaiton
            {
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

                string messageBody = string.Format(builder.HtmlBody, "Order Details", "Customer Details", "Address Details", string.Format("{0:dddd, d MMMM yyyy}", DateTime.Now));
                await _emailSender.SendEmailAsync("Customer Address", "Nursery Subscription", messageBody);

                return RedirectToPage("/Thankyou");
            }


            //if (Payment == null)
            //{
            //    toastNotification.AddErrorToastMessage("Payment Object Not Found");
            //    return RedirectToPage("", new { Payment });
            //}
            //else
            //{

            //}
            return RedirectToPage();
        }

        //public async Task<IActionResult> OnPostCheckOut(string parsecartItems, string Serial)
        //{
           
        //        var ShoppingCart = HttpContext.Session.GetString("parsecartItems");
        //        var user = await userManager.GetUserAsync(User);
        //        if (user == null)
        //        {
        //            return Page();
        //        }
        //        List<ShoppingCart> EmptyShippingCartList = new List<ShoppingCart>();
        //        shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(ShoppingCart);


        //        var customerObj = perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
        //        if (customerObj == null)
        //        {
        //            return NotFound();

        //        }
        //        if (shippingCartObjectVMs != null)
        //        {
        //            if (shippingCartObjectVMs.Count != 0)
        //            {
        //                foreach (var item in shippingCartObjectVMs)
        //                {
        //                    var shippingCartObj = new ShoppingCart()
        //                    {
        //                        CustomerId = customerObj.CustomerId,
        //                        ItemId = item.ItemId,
        //                        ItemPrice = item.priceCart,
        //                        ItemQty = item.Qunatity,
        //                        ItemTotal = item.priceCart * item.Qunatity
        //                    };
        //                    EmptyShippingCartList.Add(shippingCartObj);
        //                    perfumeContext.ShoppingCart.Add(shippingCartObj);
        //                    perfumeContext.SaveChanges();
        //                }

        //            }
        //        }
        //    return new JsonResult(parsecartItems);
        //    }
        public async Task<IActionResult> OnPostAddCustomerAddress(string Serial)
        {
            try
            {
                
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Page();
                }

                var Country = HttpContext.Session.GetString("country");
                var customerObj = perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return NotFound();

                }
                //var UserIpAddress = GetUserIpAddress();

                //var country = GetUserCountryByIp(UserIpAddress);

                //string CountryToUse;

                //if (country != null)
                //{
                //    var IsDbCountry = perfumeContext.Country.Any(i => i.CountryTlen == country);

                //    CountryToUse = country;

                //    if (!IsDbCountry)
                //    {
                //        var firstCountry = perfumeContext.Country.FirstOrDefault().CountryTlen;
                //        CountryToUse = firstCountry;
                     
                //    }
                //}
                //else
                //{
                //    var firstCountry = perfumeContext.Country.FirstOrDefault().CountryTlen;
                //    CountryToUse = firstCountry;
                    
                //}

                var countryId = perfumeContext.Country.Where(c => c.CountryId ==int.Parse(Country)).FirstOrDefault().CountryId;
                var AddressCustomer = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();
                if (AddressCustomer == null)
                {
                    var customerAddress = new CustomerAddress()
                    {

                        CustomerId = customerObj.CustomerId,
                        Address = customerAddr.Address,
                        CountryId = countryId, 
                        CityName = customerAddr.CityName,
                        AreaName = customerAddr.AreaName,
                        BuildingNo = customerAddr.BuildingNo,
                        Mobile = customerAddr.Mobile,

                    };


                    perfumeContext.customerAddresses.Add(customerAddress);
                    perfumeContext.SaveChanges();

                    toastNotification.AddSuccessToastMessage("Address Added Successfully");

                }
                else
                {
                    AddressCustomer.Address = customerAddr.Address;
                    AddressCustomer.AreaName = customerAddr.AreaName;
                    AddressCustomer.CityName = customerAddr.CityName;
                    AddressCustomer.BuildingNo = customerAddr.BuildingNo;
                    AddressCustomer.CountryId = countryId;
                    AddressCustomer.Mobile = customerAddr.Mobile;


                    perfumeContext.Attach(AddressCustomer).State = EntityState.Modified;
                    perfumeContext.SaveChanges();
                }


                var existedAddressForCustomer = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();

                var CountryObj = perfumeContext.Country.Where(i => i.CountryId == countryId).FirstOrDefault();
                string currencyISO= perfumeContext.Currency.Where(i=>i.CurrencyId==CountryObj.CurrencyId).FirstOrDefault().CurrencyTlen;
                var customerShoppingCartList = perfumeContext.
                 ShoppingCart.Include(s => s.CustomerN)
                 .Include(s => s.Item)
                 .Where(c => c.CustomerId == customerObj.CustomerId);

                double shoppingCost = 0.0;

                shoppingCost = CountryObj.ShippingCost.Value;

                var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);

                coupon = perfumeContext.Coupon.Where(i => i.Serial == Serial).FirstOrDefault();

                double discount = 0.0;

                GetDiscountFromCoupon(totalOfAll,coupon);

               
                int maxUniqe = 1;
                var newList = perfumeContext.Order.ToList();
                var maxserial = Convert.ToInt64(1);
                perfumeContext.Order.ToList().Max(e => Convert.ToInt64(e.OrderSerial));
                if (newList != null)
                {
                    if (newList.Count > 0)
                    {
                        maxUniqe = newList.Max(e => e.UniqeId).Value;
                        maxserial= newList.Max(e => Convert.ToInt64(e.OrderSerial));
                    }
                }
                var orders =
            new Order
            {
                OrderDate = DateTime.Now,
                OrderSerial = Convert.ToString(maxserial + 1),
                CustomerId = customerObj.CustomerId,
                CustomerAddressId = existedAddressForCustomer.CustomerAddressId,
                OrderTotal = totalOfAll,
                CouponId = coupon != null ? coupon.CouponId : null,
                CouponTypeId = coupon != null ? coupon.CouponTypeId : null,
                CouponAmount = coupon != null ? (float?)coupon.Amount : null,
                Deliverycost = shoppingCost,
                OrderNet = TotalAmountAfterDiscount + shoppingCost,
                PaymentMethodId = PaymentId,
                OrderDiscount = Discount,
                IsCanceled = false,
                OrderStatusId = 1,
                CountryId = countryId,
                UniqeId = maxUniqe + 1,
                IsDeliverd = false,
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

                if (PaymentId == 1 ) // My Fatoorah
                {
                    if(int.Parse(Country)== 5)
                    {
                        bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                        var TestToken = _configuration["Test_All_Saudi"];
                        var LiveToken = _configuration["Live_All_Saudi"];
                        if (Fattorahstatus) // fattorah Saudi live
                        {
                            var sendPaymentRequest = new
                            {
                                CustomerName = customerObj.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = TotalAmountAfterDiscount + shoppingCost, // Order Total + Delivery
                                CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/fattorahsucess",// Success Page
                                ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahError",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customerObj.Email ,//Customer Email
                                DisplayCurrencyIso = "SAR"
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
                        else               // fattorah Saudi test
                        {

                            var sendPaymentRequest = new
                            {

                                CustomerName = customerObj.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = orders.OrderNet, // Order Total + Delivery
                                CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/fattorahsucess",// Success Page
                                ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahError",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customerObj.Email, //Customer Email
                                DisplayCurrencyIso = "SAR"
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
                                CustomerName = customerObj.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = TotalAmountAfterDiscount + shoppingCost, // Order Total + Delivery
                                CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/fattorahsucess",// Success Page
                                ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahError",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customerObj.Email,
                                DisplayCurrencyIso = currencyISO 
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

                                CustomerName = customerObj.CustomerName, // Customer Name
                                NotificationOption = "LNK",//Fixed
                                InvoiceValue = orders.OrderNet, // Order Total + Delivery
                                CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/AllCountryfattorahsucess",// Success Page
                                ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/AllCountryfattoraherror",// Failed Page
                                UserDefinedField = orders.OrderId, //Order ID
                                CustomerEmail = customerObj.Email, //Customer Email
                                DisplayCurrencyIso = currencyISO
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
                else if (PaymentId == 2 ) //Cash On Delivery --> Not Exists on This Applicaiton
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
                    string messageBody = string.Format(builder.HtmlBody,
                       Cost,
                       orders.OrderDiscount,
                       orders.OrderNet,
                       Customer.CustomerName,
                       orders.OrderTotal,
                       orders.OrderSerial

                       );
                    await _emailSender.SendEmailAsync(Customer.Email, "Order Details", messageBody);

                    //var webRoot = _hostEnvironment.WebRootPath;

                    //var pathToFile = _hostEnvironment.WebRootPath
                    //       + Path.DirectorySeparatorChar.ToString()
                    //       + "Templates"
                    //       + Path.DirectorySeparatorChar.ToString()
                    //       + "EmailTemplate"
                    //       + Path.DirectorySeparatorChar.ToString()
                    //       + "Email.html";
                    //var builder = new BodyBuilder();
                    //using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    //{

                    //    builder.HtmlBody = SourceReader.ReadToEnd();

                    //}

                    //string messageBody = string.Format(builder.HtmlBody, "Order Details", "Customer Details", "Address Details", string.Format("{0:dddd, d MMMM yyyy}", DateTime.Now));
                    //await _emailSender.SendEmailAsync("Customer Address", "Nursery Subscription", messageBody);
                    return Redirect("/Thankyou");
                }


                return RedirectToPage("/CheckOut");
            }
            catch (Exception ex)
            {
                toastNotification.AddErrorToastMessage("Something went wrong");
                return RedirectToPage("/CheckOut");
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

		public string GetUserIpAddress()
		{
			string Ip = Response.HttpContext.Connection.RemoteIpAddress.ToString();

			if (Ip == "::1")
			{
				Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
			}
			return Ip;
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
