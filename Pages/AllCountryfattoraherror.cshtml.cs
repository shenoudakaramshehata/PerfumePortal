using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using CRM.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using CRM.Services;
using CRM.ViewModels;

namespace CRM.Pages
{
    public class AllCountryfattoraherrorModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        public Order order { get; set; }
        private readonly IRazorPartialToStringRenderer _renderer;
        public ApplicationUser user { set; get; }
        public InvoiceVm invoiceVm { get; set; }

        private readonly IConfiguration _configuration;
        FattorhResult FattoraResStatus { set; get; }
        //public static bool expired = false;
        string res { set; get; }
        public AllCountryfattoraherrorModel(IRazorPartialToStringRenderer renderer, PerfumeContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _renderer = renderer;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _env = env;
        }
        public FattorahPaymentResult fattorahPaymentResult { get; set; }
        static string token = "rLtt6JWvbUHDDhsZnfpAhpYk4dxYDQkbcPTyGaKp2TYqQgG7FGZ5Th_WD53Oq8Ebz6A53njUoo1w3pjU1D4vs_ZMqFiz_j0urb_BH9Oq9VZoKFoJEDAbRZepGcQanImyYrry7Kt6MnMdgfG5jn4HngWoRdKduNNyP4kzcp3mRv7x00ahkm9LAK7ZRieg7k1PDAnBIOG3EyVSJ5kK4WLMvYr7sCwHbHcu4A5WwelxYK0GMJy37bNAarSJDFQsJ2ZvJjvMDmfWwDVFEVe_5tOomfVNt6bOg9mexbGjMrnHBnKnZR1vQbBtQieDlQepzTZMuQrSuKn-t5XZM7V6fCW7oP-uXGX-sMOajeX65JOf6XVpk29DP6ro8WTAflCDANC193yof8-f5_EYY-3hXhJj7RBXmizDpneEQDSaSz5sFk0sV5qPcARJ9zGG73vuGFyenjPPmtDtXtpx35A-BVcOSBYVIWe9kndG3nclfefjKEuZ3m4jL9Gg1h2JBvmXSMYiZtp9MR5I6pvbvylU_PP5xJFSjVTIz7IQSjcVGO41npnwIxRXNRxFOdIUHn0tjQ-7LwvEcTXyPsHXcMD8WtgBh-wxR8aKX7WPSsT1O8d8reb2aR7K3rkV3K82K_0OgawImEpwSvp9MNKynEAJQS6ZHe_J_l77652xwPNxMRTMASk1ZsJL";
        static string testURL = "https://apitest.myfatoorah.com/v2/GetPaymentStatus";
        static string liveURL = "https://api.myfatoorah.com/v2/GetPaymentStatus";


        public async Task<IActionResult> OnGet(string paymentId)
        {
            if (paymentId == null)
            {
                return RedirectToPage("SomethingwentError");
            }

            var GetPaymentStatusRequest = new
            {
                Key = paymentId,
                KeyType = "paymentId"
            };

            bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
            var TestToken = _configuration["Test_All_Country"];
            var LiveToken = _configuration["Live_All_Country"];

            var GetPaymentStatusRequestJSON = JsonConvert.SerializeObject(GetPaymentStatusRequest);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (Fattorahstatus) // fattorah live
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                var httpContent = new StringContent(GetPaymentStatusRequestJSON, System.Text.Encoding.UTF8, "application/json");
                var responseMessage = client.PostAsync(liveURL, httpContent);
                res = await responseMessage.Result.Content.ReadAsStringAsync();
                FattoraResStatus = JsonConvert.DeserializeObject<FattorhResult>(res);
            }
            else                 // fattorah test
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                var httpContent = new StringContent(GetPaymentStatusRequestJSON, System.Text.Encoding.UTF8, "application/json");
                var responseMessage = client.PostAsync(testURL, httpContent);
                res = await responseMessage.Result.Content.ReadAsStringAsync();
                FattoraResStatus = JsonConvert.DeserializeObject<FattorhResult>(res);
            }


            if (FattoraResStatus.IsSuccess == true)
            {
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                fattorahPaymentResult = jObject["Data"].ToObject<FattorahPaymentResult>();
                int orderId = 0;
                bool checkRes = int.TryParse(fattorahPaymentResult.UserDefinedField, out orderId);
                if (fattorahPaymentResult.InvoiceStatus == "Paid")
                {
                    try
                    {
                        if (fattorahPaymentResult.UserDefinedField != null)
                        {

                            if (checkRes)
                            {
                                //if (expired == false)
                                //{
                                Coupon? coupon = null;


                                order = await _context.Order.Where(e => e.OrderId == orderId).FirstOrDefaultAsync();


                                order.IsPaid = true;

                                order.PaymentId = paymentId;
                                order.PostDate = DateTime.Now;
                                order.OrderStatusId = 2;
                                var TrakingOrderObj = new OrderTraking()
                                {
                                    OrderId = order.OrderId,
                                    OrderStatusId = 2,
                                    ActionDate = DateTime.Now,
                                    Remarks = "Order Paid Successfully"
                                };
                                _context.OrderTrakings.Add(TrakingOrderObj);

                                if (order.CouponId != null)
                                {
                                    coupon = _context.Coupon.FirstOrDefault(c => c.CouponId == order.CouponId);

                                    if (coupon != null)
                                    {
                                        coupon.Used = true;
                                        var UpdatedCoupon = _context.Coupon.Attach(coupon);
                                        UpdatedCoupon.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                        _context.SaveChanges();
                                    }
                                }
                                var UpdatedOrder = _context.Order.Attach(order);
                                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                                _context.SaveChanges();
                                var shoppingCost = _context.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault().ShippingCost;
                                var Customer = _context.CustomerNs.Where(e => e.CustomerId == order.CustomerId).FirstOrDefault();
                                if (Customer != null)
                                {
                                    var carts = _context.ShoppingCart.Where(e => e.CustomerId == order.CustomerId);
                                    _context.ShoppingCart.RemoveRange(carts);

                                }

                                _context.SaveChanges();
                                var webRoot = _env.WebRootPath;

                                var pathToFile = _env.WebRootPath
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
                                //   shoppingCost,
                                //   order.OrderDiscount,
                                //   order.OrderNet,
                                //   Customer.CustomerName,
                                //   order.OrderTotal,
                                //   order.OrderSerial

                                //   );
                                invoiceVm = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderId == orderId).Select(i => new InvoiceVm
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
                                    currencyName = _context.Currency.Where(e => e.CurrencyId == i.Country.CurrencyId).FirstOrDefault().CurrencyTlen

                                }).FirstOrDefault();
                                if (invoiceVm == null)
                                {
                                    return RedirectToPage("SomethingwentError");
                                }
                                else
                                {
                                    var orderItemVm = _context.OrderItem.Include(e => e.Item).Where(e => e.OrderId == invoiceVm.OrderId).Select(i => new OrderItemVm
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

                                //expired = true;



                                return Page();
                                //}
                            }
                        }
                        return RedirectToPage("SomethingwentError", new { Message = "SomeThing Went Error,try again" });

                    }
                    catch (Exception ex)
                    {
                        return RedirectToPage("SomethingwentError", new { Message = ex.Message });
                    }


                }
                else
                {
                    try
                    {
                        if (fattorahPaymentResult.UserDefinedField != null)
                        {
                            if (checkRes)
                            {

                                order = _context.Order.Where(e => e.OrderId == orderId).FirstOrDefault();
                                order.IsPaid = false;
                                order.PaymentId = paymentId;
                                order.PostDate = DateTime.Now;
                                order.OrderStatusId = 3;
                                var TrakingOrderObj = new OrderTraking()
                                {
                                    OrderId = order.OrderId,
                                    OrderStatusId = 3,
                                    ActionDate = DateTime.Now,
                                    Remarks = "There Is Something Error During Transaction"
                                };
                                _context.OrderTrakings.Add(TrakingOrderObj);
                                var UpdatedOrder = _context.Order.Attach(order);
                                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _context.SaveChanges();
                                //expired = true;

                                return Page();
                            }
                            return RedirectToPage("SomethingwentError", new { Message = "SomeThing Went Error,try again" });
                        }
                    }

                    catch (Exception)
                    {
                        return RedirectToPage("SomethingwentError", new { Message = "Something went wrong" });
                    }
                }

            }
            return Page();
        }
    }
}
