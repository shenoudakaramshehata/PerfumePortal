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
using NToastNotify;
using RestSharp;
using Newtonsoft.Json.Linq;
using CRM.Services.TabbyModels;
using System.Text;
using CRM.Migrations;
using System.Globalization;
using DevExpress.DataAccess.Native.Web;
using Microsoft.Extensions.Hosting;

namespace CRM.Pages
{
    public class TabbySuccessModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        public Order order { get; set; }
        public InvoiceVm invoiceVm { get; set; }
        public string PaymentStatus { get; set; }

        public ApplicationUser user { set; get; }
        private readonly IRazorPartialToStringRenderer _renderer;

        private readonly IConfiguration _configuration;
        public HttpClient httpClient { get; set; }
        public Root ResStatus { get; set; }
        FattorhResult FattoraResStatus { set; get; }
        //public static bool expired = false;
        string res { set; get; }
        public TabbySuccessModel(IRazorPartialToStringRenderer renderer, PerfumeContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _env = env;
            _renderer = renderer;
            httpClient = new HttpClient();


        }
        public int OrderFB { get; set; }
        public FattorahPaymentResult fattorahPaymentResult { get; set; }
        string token = "sk_74eb86f5-f780-4b08-b73d-1de94a8270ff";
        //static string testURL = "https://apitest.myfatoorah.com/v2/GetPaymentStatus";
        //static string liveURL = "https://api.myfatoorah.com/v2/GetPaymentStatus";

        public async Task<IActionResult> OnGet(string payment_id)
        {
            if (payment_id == null)
            {
                //return RedirectToPage("SomethingwentError");
            }
            string url = "https://api.tabby.ai/api/v1/payments/" + payment_id;


            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //request.Headers.Add("Authorization", "Bearer sk_test_bf04ca3f-b6e5-451b-8013-8aefeaeffd60");
            //request.Headers.Add("Cookie", "_cfuvid=R6P0.2ESKgiMKGYmO3NQ4qEZQuDr.W5EPlICzx3a2iQ-1691234705097-0-604800000");
            var response = await client.SendAsync(request);
            var res = await response.Content.ReadAsStringAsync();
            if (res != null)
            {
                ResStatus = JsonConvert.DeserializeObject<Root>(res);
                if (ResStatus.status == "AUTHORIZED")
                {

                    int orderId = 0;
                    bool checkRes = int.TryParse(ResStatus.meta.order_id, out orderId);

                    try
                    {
                        if (ResStatus.meta.order_id != null)
                        {

                            if (checkRes)
                            {
                                //if (expired == false)
                                //{
                                Coupon? coupon = null;

                                order = await _context.Order.Include(e => e.CustomerN).Include(e => e.CustomerAddress).Include(e => e.OrderStatus).Include(e => e.OrderItem).Where(e => e.OrderId == orderId).FirstOrDefaultAsync();
                                OrderFB = order.OrderId;

                                order.IsPaid = true;


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
                                foreach (var orderItem in order.OrderItem)
                                {
                                    var item = _context.Item.Find(orderItem.ItemId);
                                    item.Stock -= orderItem.ItemQuantity;
                                    var UpdatedItem = _context.Item.Attach(item);
                                    UpdatedItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    _context.SaveChanges();

                                }
                                //string restabby = order.Tabbyres;
                                //var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(restabby);
                                //var TotalAmount = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                                var totalAmount = ResStatus.amount;

                                CultureInfo cultureInfo = CultureInfo.InvariantCulture; // Use the appropriate culture

                                bool checkResamount = double.TryParse(totalAmount, NumberStyles.Any, cultureInfo, out double doubleValue);
                                if (checkResamount)
                                {
                                    var sendPaymentRequest = new
                                    {
                                        amount = doubleValue
                                    };

                                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                                    string catureurl = "https://api.tabby.ai/api/v1/payments/" + payment_id + "/captures";
                                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                                    var responseMessage = httpClient.PostAsync(catureurl, httpContent);
                                    var rescatpture = await responseMessage.Result.Content.ReadAsStringAsync();

                                }
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

                                invoiceVm = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.TabbyPaymentId == payment_id).Select(i => new InvoiceVm
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

                                try
                                {
                                    await _emailSender.SendEmailAsync(Customer.Email, "Order Details", messageBody);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        var options = new RestClientOptions("https://api.rmlconnect.net")
                                        {
                                            MaxTimeout = -1,
                                        };
                                        var clientOption = new RestClient(options);
                                        var requestTabby = new RestRequest("/bulksms/bulksms", Method.Post)
                       .AddParameter("username", "mashaer")
                       .AddParameter("password", "5!V[Ej4o")
                       .AddParameter("type", "0")
                       .AddParameter("dlr", "1")
                       .AddParameter("destination", Customer.Phone.Split("+")[1])
                       .AddParameter("source", "mashaer")
                       .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
                                        string requestInfo = $"Method: {requestTabby.Method}\nResource: {requestTabby.Resource}\nParameters: {string.Join(", ", requestTabby.Parameters)}";

                                        int RequestfileCount = 1;
                                        string RequestwebRootPath = _env.WebRootPath;
                                        string RequestdirectoryPath = Path.Combine(RequestwebRootPath, "ApiRequests");
                                        Directory.CreateDirectory(RequestdirectoryPath);

                                        // Check if the count file exists
                                        string RequestcountFilePath = Path.Combine(RequestdirectoryPath, "admin_count.txt");
                                        if (System.IO.File.Exists(RequestcountFilePath))
                                        {
                                            string countStr = System.IO.File.ReadAllText(RequestcountFilePath);
                                            int.TryParse(countStr, out RequestfileCount);
                                        }

                                        // Generate the filename and increment the count
                                        string RequestfileName = $"admin{RequestfileCount}.txt";
                                        string RequestfilePath = Path.Combine(RequestdirectoryPath, RequestfileName);
                                        System.IO.File.WriteAllText(RequestfilePath, requestInfo);

                                        // Increment the count and save it
                                        RequestfileCount++;
                                        System.IO.File.WriteAllText(RequestcountFilePath, RequestfileCount.ToString());




                                        RestResponse responseTabby = await clientOption.ExecuteAsync(requestTabby);
                                        int fileCount = 1;
                                        string webRootPath = _env.WebRootPath;
                                        string directoryPath = Path.Combine(webRootPath, "ApiResponses");
                                        Directory.CreateDirectory(directoryPath);

                                        // Check if the count file exists
                                        string countFilePath = Path.Combine(directoryPath, "admin_count.txt");
                                        if (System.IO.File.Exists(countFilePath))
                                        {
                                            string countStr = System.IO.File.ReadAllText(countFilePath);
                                            int.TryParse(countStr, out fileCount);
                                        }

                                        // Generate the filename and increment the count
                                        string fileName = $"admin{fileCount}.txt";
                                        string filePath = Path.Combine(directoryPath, fileName);
                                        System.IO.File.WriteAllText(filePath, responseTabby.Content);

                                        // Increment the count and save it
                                        fileCount++;
                                        System.IO.File.WriteAllText(countFilePath, fileCount.ToString());

                                        //System.IO.File.WriteAllText(filePath, response.Content);
                                        //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                        //string webRootPath = _hostEnvironment.WebRootPath;
                                        //string directoryPath = Path.Combine(webRootPath, "ApiResponses");
                                        //Directory.CreateDirectory(directoryPath);
                                        //string filePath = Path.Combine(directoryPath, $"ApiResponse_{timestamp}.txt");

                                        //// Save the response to a text file
                                        //System.IO.File.WriteAllText(filePath, response.Content);
                                        Console.WriteLine(response.Content);
                                    }
                                    catch (Exception e)
                                    {
                                        return Page();

                                    }

                                    return Page();

                                }
                                

                                try
                                {
                                    var options = new RestClientOptions("https://api.rmlconnect.net")
                                    {
                                        MaxTimeout = -1,
                                    };
                                    var clientOption = new RestClient(options);
                                    var requestTabby = new RestRequest("/bulksms/bulksms", Method.Post)
                   .AddParameter("username", "mashaer")
                   .AddParameter("password", "5!V[Ej4o")
                   .AddParameter("type", "0")
                   .AddParameter("dlr", "1")
                   .AddParameter("destination", Customer.Phone.Split("+")[1])
                   .AddParameter("source", "mashaer")
                   .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
                                    string requestInfo = $"Method: {requestTabby.Method}\nResource: {requestTabby.Resource}\nParameters: {string.Join(", ", requestTabby.Parameters)}";

                                    int RequestfileCount = 1;
                                    string RequestwebRootPath = _env.WebRootPath;
                                    string RequestdirectoryPath = Path.Combine(RequestwebRootPath, "ApiRequests");
                                    Directory.CreateDirectory(RequestdirectoryPath);

                                    // Check if the count file exists
                                    string RequestcountFilePath = Path.Combine(RequestdirectoryPath, "admin_count.txt");
                                    if (System.IO.File.Exists(RequestcountFilePath))
                                    {
                                        string countStr = System.IO.File.ReadAllText(RequestcountFilePath);
                                        int.TryParse(countStr, out RequestfileCount);
                                    }

                                    // Generate the filename and increment the count
                                    string RequestfileName = $"admin{RequestfileCount}.txt";
                                    string RequestfilePath = Path.Combine(RequestdirectoryPath, RequestfileName);
                                    System.IO.File.WriteAllText(RequestfilePath, requestInfo);

                                    // Increment the count and save it
                                    RequestfileCount++;
                                    System.IO.File.WriteAllText(RequestcountFilePath, RequestfileCount.ToString());




                                    RestResponse responseTabby = await clientOption.ExecuteAsync(requestTabby);
                                    int fileCount = 1;
                                    string webRootPath = _env.WebRootPath;
                                    string directoryPath = Path.Combine(webRootPath, "ApiResponses");
                                    Directory.CreateDirectory(directoryPath);

                                    // Check if the count file exists
                                    string countFilePath = Path.Combine(directoryPath, "admin_count.txt");
                                    if (System.IO.File.Exists(countFilePath))
                                    {
                                        string countStr = System.IO.File.ReadAllText(countFilePath);
                                        int.TryParse(countStr, out fileCount);
                                    }

                                    // Generate the filename and increment the count
                                    string fileName = $"admin{fileCount}.txt";
                                    string filePath = Path.Combine(directoryPath, fileName);
                                    System.IO.File.WriteAllText(filePath, responseTabby.Content);

                                    // Increment the count and save it
                                    fileCount++;
                                    System.IO.File.WriteAllText(countFilePath, fileCount.ToString());

                                    //System.IO.File.WriteAllText(filePath, response.Content);
                                    //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                    //string webRootPath = _hostEnvironment.WebRootPath;
                                    //string directoryPath = Path.Combine(webRootPath, "ApiResponses");
                                    //Directory.CreateDirectory(directoryPath);
                                    //string filePath = Path.Combine(directoryPath, $"ApiResponse_{timestamp}.txt");

                                    //// Save the response to a text file
                                    //System.IO.File.WriteAllText(filePath, response.Content);
                                    Console.WriteLine(response.Content);
                                }
                                catch (Exception e)
                                {
                                    return Page();
                                }
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

            }
            return Page();




        }

        public IActionResult OnGetPayment(int order)
        {
            var orderobj = _context.Order.Where(e => e.OrderId == order).FirstOrDefault();
            var country = _context.Country.Where(e => e.CountryId == orderobj.CountryId).FirstOrDefault();
            var currency = _context.Currency.Where(e => e.CurrencyId == country.CurrencyId).FirstOrDefault().CurrencyTlen;
            var result = new { currency = currency, total = orderobj.OrderNet };

            return new JsonResult(result);
        }
        //public async Task<IActionResult> OnGet(string payment_id)
        //{
        //    if (payment_id == null)
        //    {
        //        return RedirectToPage("SomethingwentError");
        //    }



        //    try
        //    {



        //                //if (expired == false)
        //                //{
        //                Coupon? coupon = null;


        //                order = await _context.Order.Include(e => e.CustomerN).Include(e => e.CustomerAddress).Include(e => e.OrderStatus).Include(e => e.OrderItem).Where(e => e.TabbyPaymentId == payment_id).FirstOrDefaultAsync();


        //                //string restabby = order.Tabbyres;
        //                //var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(restabby);
        //                //var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;


        //                //CultureInfo cultureInfo = CultureInfo.InvariantCulture; // Use the appropriate culture

        //                //bool checkResamount = double.TryParse(downpayment_total, NumberStyles.Any, cultureInfo, out double doubleValue);
        //                //if (checkResamount)
        //                //{
        //                //    var sendPaymentRequest = new
        //                //    {
        //                //        amount = doubleValue
        //                //    };

        //                //    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

        //                //    string catureurl = "https://api.tabby.ai/api/v1/payments/" + payment_id + "/captures";
        //                //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //                //    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
        //                //    var responseMessage = httpClient.PostAsync(catureurl, httpContent);
        //                //    var rescatpture = await responseMessage.Result.Content.ReadAsStringAsync();

        //                //}

        //                order.IsPaid = true;


        //                order.OrderStatusId = 2;
        //                var TrakingOrderObj = new OrderTraking()
        //                {
        //                    OrderId = order.OrderId,
        //                    OrderStatusId = 2,
        //                    ActionDate = DateTime.Now,
        //                    Remarks = "Order Paid Successfully"
        //                };
        //                _context.OrderTrakings.Add(TrakingOrderObj);

        //                if (order.CouponId != null)
        //                {
        //                    coupon = _context.Coupon.FirstOrDefault(c => c.CouponId == order.CouponId);

        //                    if (coupon != null)
        //                    {
        //                        coupon.Used = true;
        //                        var UpdatedCoupon = _context.Coupon.Attach(coupon);
        //                        UpdatedCoupon.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //                        _context.SaveChanges();
        //                    }
        //                }
        //                var UpdatedOrder = _context.Order.Attach(order);
        //                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        //                _context.SaveChanges();
        //                foreach (var orderItem in order.OrderItem)
        //                {
        //                    var item = _context.Item.Find(orderItem.ItemId);
        //                    item.Stock -= orderItem.ItemQuantity;
        //                    var UpdatedItem = _context.Item.Attach(item);
        //                    UpdatedItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //                    _context.SaveChanges();

        //                }
        //                var shoppingCost = _context.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault().ShippingCost;
        //                var Customer = _context.CustomerNs.Where(e => e.CustomerId == order.CustomerId).FirstOrDefault();
        //                if (Customer != null)
        //                {
        //                    var carts = _context.ShoppingCart.Where(e => e.CustomerId == order.CustomerId);
        //                    _context.ShoppingCart.RemoveRange(carts);

        //                }

        //                _context.SaveChanges();
        //                var webRoot = _env.WebRootPath;

        //                var pathToFile = _env.WebRootPath
        //                       + Path.DirectorySeparatorChar.ToString()
        //                       + "Templates"
        //                       + Path.DirectorySeparatorChar.ToString()
        //                       + "EmailTemplate"
        //                       + Path.DirectorySeparatorChar.ToString()
        //                       + "Email.html";
        //                var builder = new BodyBuilder();
        //                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
        //                {

        //                    builder.HtmlBody = SourceReader.ReadToEnd();

        //                }
        //                //string messageBody = string.Format(builder.HtmlBody,
        //                //   shoppingCost,
        //                //   order.OrderDiscount,
        //                //   order.OrderNet,
        //                //   Customer.CustomerName,
        //                //   order.OrderTotal,
        //                //   order.OrderSerial

        //                //   );

        //                invoiceVm = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.TabbyPaymentId == payment_id).Select(i => new InvoiceVm
        //                {
        //                    OrderId = i.OrderId,
        //                    OrderDate = i.OrderDate.Date.Year + " , " + i.OrderDate.Date.Month + " , " + i.OrderDate.Date.Day,
        //                    OrderTime = i.OrderDate.TimeOfDay.Hours + " : " + i.OrderDate.TimeOfDay.Minutes,
        //                    Country = i.Country.CountryTlen,
        //                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
        //                    CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
        //                    NetOrder = i.OrderNet.Value,
        //                    OrderTotal = i.OrderTotal,
        //                    Status = i.OrderStatus.Status,
        //                    Discount = i.OrderDiscount,
        //                    InvoiceNumber = i.UniqeId.Value,
        //                    WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
        //                    SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
        //                    ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
        //                    ShippingCost = _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().ShippingCost.Value,
        //                    ShippingAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName + " , " + i.CustomerAddress.BuildingNo,
        //                    Address = i.CustomerAddress.Address,
        //                    ShippingAddressPhone = i.CustomerAddress.Mobile,
        //                    PaymentTit = _context.paymentMehods.Where(e => e.PaymentMethodId == i.PaymentMethodId).FirstOrDefault().PaymentMethodEN,
        //                    currencyName = _context.Currency.Where(e => e.CurrencyId == i.Country.CurrencyId).FirstOrDefault().CurrencyTlen

        //                }).FirstOrDefault();
        //                if (invoiceVm == null)
        //                {
        //                    return RedirectToPage("SomethingwentError");
        //                }
        //                else
        //                {
        //                    var orderItemVm = _context.OrderItem.Include(e => e.Item).Where(e => e.OrderId == invoiceVm.OrderId).Select(i => new OrderItemVm
        //                    {
        //                        ItemImage = i.Item.ItemImage,
        //                        ItemPrice = i.ItemPrice,
        //                        ItemQuantity = i.ItemQuantity,
        //                        ItemTitleEn = i.Item.ItemTitleEn,
        //                        Total = i.Total,
        //                        ItemDis = i.Item.ItemDescriptionEn
        //                    }).ToList();
        //                    invoiceVm.orderItemVms = orderItemVm;
        //                }



        //                var messageBody = await _renderer.RenderPartialToStringAsync("_Invoice", invoiceVm);

        //                await _emailSender.SendEmailAsync(Customer.Email, "Order Details", messageBody);

        //                try
        //                {
        //                    var options = new RestClientOptions("https://api.rmlconnect.net")
        //                    {
        //                        MaxTimeout = -1,
        //                    };
        //                    var clientOption = new RestClient(options);
        //                    var requestTabby = new RestRequest("/bulksms/bulksms", Method.Post)
        //   .AddParameter("username", "mashaer")
        //   .AddParameter("password", "5!V[Ej4o")
        //   .AddParameter("type", "0")
        //   .AddParameter("dlr", "1")
        //   .AddParameter("destination", Customer.Phone.Split("+")[1])
        //   .AddParameter("source", "mashaer")
        //   .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
        //                    string requestInfo = $"Method: {requestTabby.Method}\nResource: {requestTabby.Resource}\nParameters: {string.Join(", ", requestTabby.Parameters)}";

        //                    int RequestfileCount = 1;
        //                    string RequestwebRootPath = _env.WebRootPath;
        //                    string RequestdirectoryPath = Path.Combine(RequestwebRootPath, "ApiRequests");
        //                    Directory.CreateDirectory(RequestdirectoryPath);

        //                    // Check if the count file exists
        //                    string RequestcountFilePath = Path.Combine(RequestdirectoryPath, "admin_count.txt");
        //                    if (System.IO.File.Exists(RequestcountFilePath))
        //                    {
        //                        string countStr = System.IO.File.ReadAllText(RequestcountFilePath);
        //                        int.TryParse(countStr, out RequestfileCount);
        //                    }

        //                    // Generate the filename and increment the count
        //                    string RequestfileName = $"admin{RequestfileCount}.txt";
        //                    string RequestfilePath = Path.Combine(RequestdirectoryPath, RequestfileName);
        //                    System.IO.File.WriteAllText(RequestfilePath, requestInfo);

        //                    // Increment the count and save it
        //                    RequestfileCount++;
        //                    System.IO.File.WriteAllText(RequestcountFilePath, RequestfileCount.ToString());




        //                    RestResponse responseTabby = await clientOption.ExecuteAsync(requestTabby);
        //                    int fileCount = 1;
        //                    string webRootPath = _env.WebRootPath;
        //                    string directoryPath = Path.Combine(webRootPath, "ApiResponses");
        //                    Directory.CreateDirectory(directoryPath);

        //                    // Check if the count file exists
        //                    string countFilePath = Path.Combine(directoryPath, "admin_count.txt");
        //                    if (System.IO.File.Exists(countFilePath))
        //                    {
        //                        string countStr = System.IO.File.ReadAllText(countFilePath);
        //                        int.TryParse(countStr, out fileCount);
        //                    }

        //                    // Generate the filename and increment the count
        //                    string fileName = $"admin{fileCount}.txt";
        //                    string filePath = Path.Combine(directoryPath, fileName);
        //                    System.IO.File.WriteAllText(filePath, responseTabby.Content);

        //                    // Increment the count and save it
        //                    fileCount++;
        //                    System.IO.File.WriteAllText(countFilePath, fileCount.ToString());

        //                    //System.IO.File.WriteAllText(filePath, response.Content);
        //                    //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //                    //string webRootPath = _hostEnvironment.WebRootPath;
        //                    //string directoryPath = Path.Combine(webRootPath, "ApiResponses");
        //                    //Directory.CreateDirectory(directoryPath);
        //                    //string filePath = Path.Combine(directoryPath, $"ApiResponse_{timestamp}.txt");

        //                    //// Save the response to a text file
        //                    //System.IO.File.WriteAllText(filePath, response.Content);

        //                }
        //                catch (Exception e)
        //                {
        //                    return Page();
        //                }
        //                //expired = true;



        //                return Page();
        //                //}



        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToPage("SomethingwentError", new { Message = ex.Message });
        //    }

        //    return Page();



        //    //string url = "https://api.tabby.ai/api/v1/payments/" + payment_id;


        //    //var client = new HttpClient();
        //    //var request = new HttpRequestMessage(HttpMethod.Get, url);
        //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    ////request.Headers.Add("Authorization", "Bearer sk_test_bf04ca3f-b6e5-451b-8013-8aefeaeffd60");
        //    ////request.Headers.Add("Cookie", "_cfuvid=R6P0.2ESKgiMKGYmO3NQ4qEZQuDr.W5EPlICzx3a2iQ-1691234705097-0-604800000");
        //    //var response = await client.SendAsync(request);
        //    //var res = await response.Content.ReadAsStringAsync();
        //    //if(res != null)
        //    //{
        //    //    ResStatus = JsonConvert.DeserializeObject<Root>(res);
        //    //    if (ResStatus.status == "AUTHORIZED")
        //    //    {

        //    //            JObject json = JObject.Parse(res);

        //    //            // Extract reference ID
        //    //            string referenceId = (string)json["order"]["reference_id"];
        //    //            int orderId = 0;
        //    //            bool checkRes = int.TryParse(ResStatus.meta.order_id, out orderId);

        //    //            try
        //    //            {
        //    //                if (ResStatus.meta.order_id != null)
        //    //                {

        //    //                    if (checkRes)
        //    //                    {
        //    //                        //if (expired == false)
        //    //                        //{
        //    //                        Coupon? coupon = null;


        //    //                        order = await _context.Order.Include(e => e.CustomerN).Include(e => e.CustomerAddress).Include(e => e.OrderStatus).Include(e => e.OrderItem).Where(e => e.OrderId == orderId).FirstOrDefaultAsync();


        //    //                    //string restabby = order.Tabbyres;
        //    //                    //var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(restabby);
        //    //                    //var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;


        //    //                    //CultureInfo cultureInfo = CultureInfo.InvariantCulture; // Use the appropriate culture

        //    //                    //bool checkResamount = double.TryParse(downpayment_total, NumberStyles.Any, cultureInfo, out double doubleValue);
        //    //                    //if (checkResamount)
        //    //                    //{
        //    //                    //    var sendPaymentRequest = new
        //    //                    //    {
        //    //                    //        amount = doubleValue
        //    //                    //    };

        //    //                    //    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

        //    //                    //    string catureurl = "https://api.tabby.ai/api/v1/payments/" + payment_id + "/captures";
        //    //                    //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    //                    //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    //                    //    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
        //    //                    //    var responseMessage = httpClient.PostAsync(catureurl, httpContent);
        //    //                    //    var rescatpture = await responseMessage.Result.Content.ReadAsStringAsync();

        //    //                    //}

        //    //                    order.IsPaid = true;


        //    //                        order.OrderStatusId = 2;
        //    //                        var TrakingOrderObj = new OrderTraking()
        //    //                        {
        //    //                            OrderId = order.OrderId,
        //    //                            OrderStatusId = 2,
        //    //                            ActionDate = DateTime.Now,
        //    //                            Remarks = "Order Paid Successfully"
        //    //                        };
        //    //                        _context.OrderTrakings.Add(TrakingOrderObj);

        //    //                        if (order.CouponId != null)
        //    //                        {
        //    //                            coupon = _context.Coupon.FirstOrDefault(c => c.CouponId == order.CouponId);

        //    //                            if (coupon != null)
        //    //                            {
        //    //                                coupon.Used = true;
        //    //                                var UpdatedCoupon = _context.Coupon.Attach(coupon);
        //    //                                UpdatedCoupon.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //    //                                _context.SaveChanges();
        //    //                            }
        //    //                        }
        //    //                        var UpdatedOrder = _context.Order.Attach(order);
        //    //                        UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        //    //                        _context.SaveChanges();
        //    //                        foreach (var orderItem in order.OrderItem)
        //    //                        {
        //    //                            var item = _context.Item.Find(orderItem.ItemId);
        //    //                            item.Stock -= orderItem.ItemQuantity;
        //    //                            var UpdatedItem = _context.Item.Attach(item);
        //    //                            UpdatedItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //    //                            _context.SaveChanges();

        //    //                        }
        //    //                        var shoppingCost = _context.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault().ShippingCost;
        //    //                        var Customer = _context.CustomerNs.Where(e => e.CustomerId == order.CustomerId).FirstOrDefault();
        //    //                        if (Customer != null)
        //    //                        {
        //    //                            var carts = _context.ShoppingCart.Where(e => e.CustomerId == order.CustomerId);
        //    //                            _context.ShoppingCart.RemoveRange(carts);

        //    //                        }

        //    //                        _context.SaveChanges();
        //    //                        var webRoot = _env.WebRootPath;

        //    //                        var pathToFile = _env.WebRootPath
        //    //                               + Path.DirectorySeparatorChar.ToString()
        //    //                               + "Templates"
        //    //                               + Path.DirectorySeparatorChar.ToString()
        //    //                               + "EmailTemplate"
        //    //                               + Path.DirectorySeparatorChar.ToString()
        //    //                               + "Email.html";
        //    //                        var builder = new BodyBuilder();
        //    //                        using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
        //    //                        {

        //    //                            builder.HtmlBody = SourceReader.ReadToEnd();

        //    //                        }
        //    //                        //string messageBody = string.Format(builder.HtmlBody,
        //    //                        //   shoppingCost,
        //    //                        //   order.OrderDiscount,
        //    //                        //   order.OrderNet,
        //    //                        //   Customer.CustomerName,
        //    //                        //   order.OrderTotal,
        //    //                        //   order.OrderSerial

        //    //                        //   );

        //    //                        invoiceVm = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderId == orderId).Select(i => new InvoiceVm
        //    //                        {
        //    //                            OrderId = i.OrderId,
        //    //                            OrderDate = i.OrderDate.Date.Year + " , " + i.OrderDate.Date.Month + " , " + i.OrderDate.Date.Day,
        //    //                            OrderTime = i.OrderDate.TimeOfDay.Hours + " : " + i.OrderDate.TimeOfDay.Minutes,
        //    //                            Country = i.Country.CountryTlen,
        //    //                            CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
        //    //                            CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
        //    //                            NetOrder = i.OrderNet.Value,
        //    //                            OrderTotal = i.OrderTotal,
        //    //                            Status = i.OrderStatus.Status,
        //    //                            Discount = i.OrderDiscount,
        //    //                            InvoiceNumber = i.UniqeId.Value,
        //    //                            WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
        //    //                            SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
        //    //                            ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
        //    //                            ShippingCost = _context.Country.Where(e => e.CountryId == i.CountryId).FirstOrDefault().ShippingCost.Value,
        //    //                            ShippingAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName + " , " + i.CustomerAddress.BuildingNo,
        //    //                            Address = i.CustomerAddress.Address,
        //    //                            ShippingAddressPhone = i.CustomerAddress.Mobile,
        //    //                            PaymentTit = _context.paymentMehods.Where(e => e.PaymentMethodId == i.PaymentMethodId).FirstOrDefault().PaymentMethodEN,
        //    //                            currencyName = _context.Currency.Where(e => e.CurrencyId == i.Country.CurrencyId).FirstOrDefault().CurrencyTlen

        //    //                        }).FirstOrDefault();
        //    //                        if (invoiceVm == null)
        //    //                        {
        //    //                            return RedirectToPage("SomethingwentError");
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            var orderItemVm = _context.OrderItem.Include(e => e.Item).Where(e => e.OrderId == invoiceVm.OrderId).Select(i => new OrderItemVm
        //    //                            {
        //    //                                ItemImage = i.Item.ItemImage,
        //    //                                ItemPrice = i.ItemPrice,
        //    //                                ItemQuantity = i.ItemQuantity,
        //    //                                ItemTitleEn = i.Item.ItemTitleEn,
        //    //                                Total = i.Total,
        //    //                                ItemDis = i.Item.ItemDescriptionEn
        //    //                            }).ToList();
        //    //                            invoiceVm.orderItemVms = orderItemVm;
        //    //                        }



        //    //                        var messageBody = await _renderer.RenderPartialToStringAsync("_Invoice", invoiceVm);

        //    //                        await _emailSender.SendEmailAsync(Customer.Email, "Order Details", messageBody);

        //    //                        try
        //    //                        {
        //    //                            var options = new RestClientOptions("https://api.rmlconnect.net")
        //    //                            {
        //    //                                MaxTimeout = -1,
        //    //                            };
        //    //                            var clientOption = new RestClient(options);
        //    //                            var requestTabby = new RestRequest("/bulksms/bulksms", Method.Post)
        //    //           .AddParameter("username", "mashaer")
        //    //           .AddParameter("password", "5!V[Ej4o")
        //    //           .AddParameter("type", "0")
        //    //           .AddParameter("dlr", "1")
        //    //           .AddParameter("destination", Customer.Phone.Split("+")[1])
        //    //           .AddParameter("source", "mashaer")
        //    //           .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
        //    //                            string requestInfo = $"Method: {requestTabby.Method}\nResource: {requestTabby.Resource}\nParameters: {string.Join(", ", requestTabby.Parameters)}";

        //    //                            int RequestfileCount = 1;
        //    //                            string RequestwebRootPath = _env.WebRootPath;
        //    //                            string RequestdirectoryPath = Path.Combine(RequestwebRootPath, "ApiRequests");
        //    //                            Directory.CreateDirectory(RequestdirectoryPath);

        //    //                            // Check if the count file exists
        //    //                            string RequestcountFilePath = Path.Combine(RequestdirectoryPath, "admin_count.txt");
        //    //                            if (System.IO.File.Exists(RequestcountFilePath))
        //    //                            {
        //    //                                string countStr = System.IO.File.ReadAllText(RequestcountFilePath);
        //    //                                int.TryParse(countStr, out RequestfileCount);
        //    //                            }

        //    //                            // Generate the filename and increment the count
        //    //                            string RequestfileName = $"admin{RequestfileCount}.txt";
        //    //                            string RequestfilePath = Path.Combine(RequestdirectoryPath, RequestfileName);
        //    //                            System.IO.File.WriteAllText(RequestfilePath, requestInfo);

        //    //                            // Increment the count and save it
        //    //                            RequestfileCount++;
        //    //                            System.IO.File.WriteAllText(RequestcountFilePath, RequestfileCount.ToString());




        //    //                            RestResponse responseTabby = await clientOption.ExecuteAsync(requestTabby);
        //    //                            int fileCount = 1;
        //    //                            string webRootPath = _env.WebRootPath;
        //    //                            string directoryPath = Path.Combine(webRootPath, "ApiResponses");
        //    //                            Directory.CreateDirectory(directoryPath);

        //    //                            // Check if the count file exists
        //    //                            string countFilePath = Path.Combine(directoryPath, "admin_count.txt");
        //    //                            if (System.IO.File.Exists(countFilePath))
        //    //                            {
        //    //                                string countStr = System.IO.File.ReadAllText(countFilePath);
        //    //                                int.TryParse(countStr, out fileCount);
        //    //                            }

        //    //                            // Generate the filename and increment the count
        //    //                            string fileName = $"admin{fileCount}.txt";
        //    //                            string filePath = Path.Combine(directoryPath, fileName);
        //    //                            System.IO.File.WriteAllText(filePath, responseTabby.Content);

        //    //                            // Increment the count and save it
        //    //                            fileCount++;
        //    //                            System.IO.File.WriteAllText(countFilePath, fileCount.ToString());

        //    //                            //System.IO.File.WriteAllText(filePath, response.Content);
        //    //                            //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //    //                            //string webRootPath = _hostEnvironment.WebRootPath;
        //    //                            //string directoryPath = Path.Combine(webRootPath, "ApiResponses");
        //    //                            //Directory.CreateDirectory(directoryPath);
        //    //                            //string filePath = Path.Combine(directoryPath, $"ApiResponse_{timestamp}.txt");

        //    //                            //// Save the response to a text file
        //    //                            //System.IO.File.WriteAllText(filePath, response.Content);
        //    //                            Console.WriteLine(response.Content);
        //    //                        }
        //    //                        catch (Exception e)
        //    //                        {
        //    //                            return Page();
        //    //                        }
        //    //                        //expired = true;



        //    //                        return Page();
        //    //                        //}
        //    //                    }
        //    //                }
        //    //                return RedirectToPage("SomethingwentError", new { Message = "SomeThing Went Error,try again" });

        //    //            }
        //    //            catch (Exception ex)
        //    //            {
        //    //                return RedirectToPage("SomethingwentError", new { Message = ex.Message });
        //    //            }







        //}




    } 
        }
    

