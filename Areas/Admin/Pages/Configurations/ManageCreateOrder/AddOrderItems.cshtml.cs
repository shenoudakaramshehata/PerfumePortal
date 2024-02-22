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
using DevExpress.Printing.Core.PdfExport.Metafile;
using DevExpress.XtraCharts;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using CRM.Services.TabbyModels;
using DevExpress.XtraPrinting.Native;

namespace CRM.Areas.Admin.Pages.Configurations.ManageCreateOrder
{
    public class AddOrderItemsModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailSender _emailSender;
        public InvoiceVm invoiceVm { get; set; }
        private readonly IRazorPartialToStringRenderer _renderer;
        [BindProperty]
        public ViewModels.EditOrder editOrder { get; set; }
        [BindProperty]
        public OrderItem orderItem { get; set; }
        public List<OrderItem> OrderList { get; set; }
        public CRM.Models.Order OrderReceipt { get; set; }
        [BindProperty]
        public CRM.Models.Order order { get; set; }
        public CustomerAddress customerAddress { get; set; }
        public CustomerN Customer { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodAR { get; set; }
        public double tax { get; set; }
        public string CurrencyEN { get; set; }
        public string CurrencyNameAr { get; set; }
        public double DeliveryCost { get; set; }
        public double taxpercentage { get; set; }

        public double OrdernetByTax { get; set; }
        public double approximatedNumber { get; set; }
        [BindProperty]
        public CustomerVM customerVM { get; set; }

        public HttpClient httpClient { get; set; }

        public double DiscountAmount { get; set; }
        public List<OrderTraking> OrderTraking { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public int FattorahPaymentId { get; set; }
        public List<ShippingMatrix> shippingMatrices { get; set; }

        public double TotalWeight { get; set; }
        private bool _formSubmitted = false;
        public double DeliverCost { get; set; }
        public int CahshPaymentId { get; set; }
        //[BindProperty(SupportsGet = true)]
        //public string CouponSerial { get; set; }
        public AddOrderItemsModel(IRazorPartialToStringRenderer renderer,PerfumeContext perfumeContext, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IConfiguration configuration, IWebHostEnvironment hostEnvironment, IEmailSender emailSender)
        {
            _renderer = renderer;
            _context = perfumeContext;
            _toastNotification = toastNotification;
            this.userManager = userManager;
            this._configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            httpClient = new HttpClient();
            editOrder = new ViewModels.EditOrder();
            orderItem = new OrderItem();
            customerVM = new CustomerVM();
            
            order = new Models.Order();
        }
        public void OnGet(int Id)
        {
            var order = _context.Order.Where(e => e.OrderId == Id).FirstOrDefault();
            OrderList = _context.OrderItem.Include(i => i.Item).Include(i => i.Order)
                                            .ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN)
                                        .Where(o => o.OrderId == Id).ToList();
            if(order != null)
            {
                DeliverCost = order.Deliverycost.Value;

            }else
            {
                DeliverCost = 0;
            }

            OrderReceipt = _context.Order.Where(i => i.OrderId == Id).Include(a => a.OrderStatus).FirstOrDefault();

            customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == OrderReceipt.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
            Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

            CurrencyEN = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen;
            CurrencyNameAr = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlar;
            tax = _context.Country.Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().tax.Value;
           
            OrdernetByTax = (OrderReceipt.OrderTotal * tax);
            double originalNumber = OrderReceipt.OrderNet.HasValue ? OrderReceipt.OrderNet.Value: 0;
            int decimalPlaces = 2;
            approximatedNumber = Math.Round(originalNumber, decimalPlaces);

            DiscountAmount = (OrderReceipt.DiscountAmount.HasValue ? OrderReceipt.DiscountAmount.Value : 0) + (OrderReceipt.CouponAmount.HasValue ? (OrderReceipt.CouponTypeId == 1 ? (OrderReceipt.CouponAmount.Value / 100) * OrderReceipt.OrderTotal : OrderReceipt.CouponAmount.Value) : 0);

            taxpercentage = tax * 100;
            OrderTraking = _context.OrderTrakings.Include(i => i.OrderStatus).Where(e => e.OrderId == Id).ToList();
            var paymentMethod = _context.paymentMehods
                .Where(e => e.PaymentMethodId == OrderReceipt.PaymentMethodId)
                .FirstOrDefault();
            PaymentMethod = paymentMethod != null ? paymentMethod.PaymentMethodEN : "";
            PaymentMethodAR = paymentMethod != null ? paymentMethod.PaymentMethodAR : "";
            FattorahPaymentId = _context.paymentMehods.Where(e => e.PaymentMethodId == 1).FirstOrDefault().PaymentMethodId;
            CahshPaymentId = _context.paymentMehods.Where(e => e.PaymentMethodId == 2).FirstOrDefault().PaymentMethodId;
        }

        public async Task<IActionResult> OnPost(int Id)
        {
            try
            {
                var payment = Request.Form["paymentMethod"];
                var paymentId = int.Parse(payment);
                var notes= Request.Form["OrderNotes"]; 
                var user = await userManager.GetUserAsync(User);
                var order = _context.Order.Where(e=>e.OrderId== Id).FirstOrDefault();
                if (order == null)
                {
                    return NotFound();

                }
                var customerObj = _context.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return NotFound();

                }


                order.PaymentMethodId = paymentId;
                order.IsCanceled = false;
                order.IsDeliverd = false;
                order.Notes = notes;
                var Updatedorder = _context.Order.Attach(order);
                Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();

                
                var TrakingOrderObj = new OrderTraking()
                {
                    OrderId = order.OrderId,
                    OrderStatusId = 1,
                    ActionDate = DateTime.Now,
                    Remarks = "Order Initiated"
                };

                _context.OrderTrakings.Add(TrakingOrderObj);

                _context.SaveChanges();

                if (paymentId == 1) // My Fatoorah
                {
                    var Cost = _context.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault().ShippingCost;

                    var orderobj = _context.Order.Where(e => e.OrderId == order.OrderId).FirstOrDefault();

                    orderobj.IsPaid = true;


                    order.OrderStatusId = 2;
                    var UpdatedOrder = _context.Order.Attach(orderobj);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    var TrakingOrder = new OrderTraking()
                    {
                        OrderId = order.OrderId,
                        OrderStatusId = 2,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Paid Successfully"
                    };
                    _context.OrderTrakings.Add(TrakingOrder);
                    _context.SaveChanges();

                    var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
                    var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
                    var Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId); 
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
                    invoiceVm = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderId == order.OrderId).Select(i => new InvoiceVm
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
                        Tax = i.tax.HasValue ? i.tax.Value : 0,
                        Discount = i.OrderDiscount,
                        InvoiceNumber = i.UniqeId.Value,
                        WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                        SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
                        ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                        ShippingCost = i.Deliverycost.HasValue ? i.Deliverycost.Value : 0,
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
                            var client = new RestClient(options);
                            var request = new RestRequest("/bulksms/bulksms", Method.Post)
           .AddParameter("username", "mashaer")
           .AddParameter("password", "5!V[Ej4o")
           .AddParameter("type", "0")
           .AddParameter("dlr", "1")
           .AddParameter("destination", Customer.Phone.Split("+")[1])
           .AddParameter("source", "mashaer")
           .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
                            string requestInfo = $"Method: {request.Method}\nResource: {request.Resource}\nParameters: {string.Join(", ", request.Parameters)}";

                            int RequestfileCount = 1;
                            string RequestwebRootPath = _hostEnvironment.WebRootPath;
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




                            RestResponse response = await client.ExecuteAsync(request);
                            int fileCount = 1;
                            string webRootPath = _hostEnvironment.WebRootPath;
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
                            System.IO.File.WriteAllText(filePath, response.Content);

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
                            return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

                        }
                        return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

                    }
                    try
                    {
                        var options = new RestClientOptions("https://api.rmlconnect.net")
                        {
                            MaxTimeout = -1,
                        };
                        var client = new RestClient(options);
                        //var request = new RestRequest("/bulksms/bulksms?username=mashaer&password=5!V[Ej4o&type=0&dlr=1&destination=" + Customer.Phone.Split("+")[1]+"&source=mashaer&message=Your order on Mashaer perfumes is now Processing.", Method.Post);
                        var request = new RestRequest("/bulksms/bulksms", Method.Post)
       .AddParameter("username", "mashaer")
       .AddParameter("password", "5!V[Ej4o")
       .AddParameter("type", "0")
       .AddParameter("dlr", "1")
       .AddParameter("destination", Customer.Phone.Split("+")[1])
       .AddParameter("source", "mashaer")
       .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
                        string requestInfo = $"Method: {request.Method}\nResource: {request.Resource}\nParameters: {string.Join(", ", request.Parameters)}";

                        int RequestfileCount = 1;
                        string RequestwebRootPath = _hostEnvironment.WebRootPath;
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




                        RestResponse response = await client.ExecuteAsync(request);
                        int fileCount = 1;
                        string webRootPath = _hostEnvironment.WebRootPath;
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
                        System.IO.File.WriteAllText(filePath, response.Content);

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
                    catch(Exception e)
                    {
                        return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

                    }
                    //var SMSMessage = new
                    //{
                    //    username = "mashaer",
                    //    password = "5!V[Ej4o",
                    //    type = 0,
                    //    dlr = 1,
                    //    destination = Customer.Phone.Split("+")[1],
                    //    message = "Your order on Mashaer perfumes is now Processing."
                    //};
                    //var sendPaymentRequestJSON = JsonConvert.SerializeObject(SMSMessage);

                    //var client = new RestClient("http://api.rmlconnect.net/bulksms/bulksms");
                    //var request = new RestRequest();
                    //request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", sendPaymentRequestJSON, ParameterType.RequestBody);
                    //RestResponse response = await client.PostAsync(request);
                    //var stringrespone = response.Content.ToString();



                    return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);




                }
                else if (paymentId == 2) //Cash On Delivery --> Not Exists on This Applicaiton
                {
                    var Cost = _context.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault().ShippingCost;

                    var orderobj2 = _context.Order.Where(e => e.OrderId == order.OrderId).FirstOrDefault();

                    orderobj2.IsPaid = true;


                    orderobj2.OrderStatusId = 2;
                    var UpdatedOrder = _context.Order.Attach(orderobj2);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    var TrakingOrder = new OrderTraking()
                    {
                        OrderId = order.OrderId,
                        OrderStatusId = 2,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Paid Successfully"
                    };
                    _context.OrderTrakings.Add(TrakingOrder);
                    _context.SaveChanges();

                    var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
                    var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
                    var Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

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
                    invoiceVm = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderId == order.OrderId).Select(i => new InvoiceVm
                    {
                        OrderId = i.OrderId,
                        OrderDate = i.OrderDate.Date.Year + " , " + i.OrderDate.Date.Month + " , " + i.OrderDate.Date.Day,
                        OrderTime = i.OrderDate.TimeOfDay.Hours + " : " + i.OrderDate.TimeOfDay.Minutes,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        OrderTotal = i.OrderTotal,
                        Tax = i.tax.HasValue ? i.tax.Value : 0,
                        Status = i.OrderStatus.Status,
                        Discount = i.OrderDiscount,
                        InvoiceNumber = i.UniqeId.Value,
                        WebSite = $"{this.Request.Scheme}://{this.Request.Host}",
                        SuppEmail = _context.SocialMediaLinks.FirstOrDefault().ContactMail,
                        ConntactNumber = _context.SocialMediaLinks.FirstOrDefault().ContactPhone1,
                        ShippingCost = i.Deliverycost.HasValue ? i.Deliverycost.Value : 0,
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
                    catch(Exception ex) {
                        try
                        {
                            var options = new RestClientOptions("https://api.rmlconnect.net")
                            {
                                MaxTimeout = -1,
                            };
                            var client = new RestClient(options);
                            var request = new RestRequest("/bulksms/bulksms", Method.Post)
           .AddParameter("username", "mashaer")
           .AddParameter("password", "5!V[Ej4o")
           .AddParameter("type", "0")
           .AddParameter("dlr", "1")
           .AddParameter("destination", Customer.Phone.Split("+")[1])
           .AddParameter("source", "mashaer")
           .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
                            string requestInfo = $"Method: {request.Method}\nResource: {request.Resource}\nParameters: {string.Join(", ", request.Parameters)}";

                            int RequestfileCount = 1;
                            string RequestwebRootPath = _hostEnvironment.WebRootPath;
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




                            RestResponse response = await client.ExecuteAsync(request);
                            int fileCount = 1;
                            string webRootPath = _hostEnvironment.WebRootPath;
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
                            System.IO.File.WriteAllText(filePath, response.Content);

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
                            return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

                        }
                        return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

                    }

                    try
                    {
                        var options = new RestClientOptions("https://api.rmlconnect.net")
                        {
                            MaxTimeout = -1,
                        };
                        var client = new RestClient(options);
                        var request = new RestRequest("/bulksms/bulksms", Method.Post)
       .AddParameter("username", "mashaer")
       .AddParameter("password", "5!V[Ej4o")
       .AddParameter("type", "0")
       .AddParameter("dlr", "1")
       .AddParameter("destination", Customer.Phone.Split("+")[1])
       .AddParameter("source", "mashaer")
       .AddParameter("message", "Your order on Mashaer perfumes is now Processing.");
                        string requestInfo = $"Method: {request.Method}\nResource: {request.Resource}\nParameters: {string.Join(", ", request.Parameters)}";

                        int RequestfileCount = 1;
                        string RequestwebRootPath = _hostEnvironment.WebRootPath;
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




                        RestResponse response = await client.ExecuteAsync(request);
                        int fileCount = 1;
                        string webRootPath = _hostEnvironment.WebRootPath;
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
                        System.IO.File.WriteAllText(filePath, response.Content);

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
                        return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

                    }
                    //var SMSMessage = new
                    //{
                    //    username = "mashaer",
                    //    password = "5!V[Ej4o",
                    //    type = 0,
                    //    dlr = 1,
                    //    destination = Customer.Phone.Split("+")[1],
                    //    message = "Your order on Mashaer perfumes is now Processing."
                    //};
                    //var sendPaymentRequestJSON = JsonConvert.SerializeObject(SMSMessage);

                    //var client = new RestClient("http://api.rmlconnect.net/bulksms/bulksms");
                    //var request = new RestRequest();
                    //request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", sendPaymentRequestJSON, ParameterType.RequestBody);
                    //RestResponse response = await client.PostAsync(request);
                    //var stringrespone = response.Content.ToString();


                    return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

                }

            }


            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + order.OrderId);
            }
            return Redirect("/Admin/Configurations/ManageCreateOrder/ThankYou?orderId=" + order.OrderId);

        }
        public async Task<IActionResult> OnPostAddItem(int Id)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + Id);

            }
            try
            {

                var order = _context.Order.Where(c => c.OrderId == Id).FirstOrDefault();
                //var model = _context.OrderItem.Where(e => e.ItemId == ItemId && e.OrderId == order.OrderId).FirstOrDefault();
                if (order == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + Id);
                }
                var item = Request.Form["item"];
                var itemId = int.Parse(item);
                var itemprice = _context.itemPriceNs.Where(e => e.ItemId == itemId && e.CountryId == order.CountryId).FirstOrDefault().Price;
                var total= itemprice * orderItem.ItemQuantity;

                var model = _context.OrderItem.Where(e => e.OrderId == order.OrderId && e.ItemId == itemId).FirstOrDefault();
                var DbUserCart= await _context.OrderItem.AnyAsync(e => e.OrderId == order.OrderId && e.ItemId== itemId);
                int Quantity = 1;
                if (DbUserCart)
                {


                 

                    model.ItemQuantity += orderItem.ItemQuantity;

                    model.Total = (model.ItemQuantity) * model.ItemPrice;
                    var UpdatedCart = _context.OrderItem.Attach(model);
                    UpdatedCart.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Item exist in cart");
                    //_toastNotification.AddSuccessToastMessage("Item added to cart");

                }
                else
                {
                    OrderItem orderItemobj = new OrderItem
                    {
                        ItemId = itemId,
                        ItemPrice = itemprice.Value,
                        Total = total.Value,
                        ItemQuantity = orderItem.ItemQuantity,
                        OrderId = order.OrderId
                    };

                    _context.OrderItem.Add(orderItemobj);
                    _context.SaveChanges();
                }
            
               
                //var itemprice = _context.itemPriceNs.Where(e => e.ItemId == ItemId && e.CountryId == order.CountryId).FirstOrDefault().Price;

                //var item = Request.Form["item"];
                //var itemId = int.Parse(item);
                //model.ItemId = itemId;
                //model.ItemQuantity = orderItem.ItemQuantity;
                //model.ItemPrice = itemprice.Value;
                //model.Total = model.ItemPrice * model.ItemQuantity;
                //var UpdatedorderItem = _context.OrderItem.Attach(model);
                //UpdatedorderItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

               
                order.OrderTotal = _context.OrderItem.Where(oi => oi.OrderId == order.OrderId).Sum(oi => oi.Total); ;
                var Updatedorder = _context.Order.Attach(order);
                Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
                Recalculate(order.OrderId);


                _toastNotification.AddSuccessToastMessage("Item Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + Id);

        }
        public IActionResult OnGetSingleorderForEdit(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            if (order != null)
            {
                editOrder = new ViewModels.EditOrder()
                {
                    OrderId = order.OrderId,

                    DiscountAmount = order.DiscountAmount != null ? order.DiscountAmount.Value : 0, // Handle the case when DiscountAmount is null
                    OrderDate = order.OrderDate
                };


                return new JsonResult(editOrder);
            }
            else
            {
                return new JsonResult("not found");
            }
        }

        public IActionResult OnGetSingleorderstatusForEdit(int OrderId)
        {
            order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(order);

        }



        public IActionResult OnGetSinglepaymentMethodForEdit(int OrderId)
        {
            order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(order);

        }


        public async Task<IActionResult> OnPostEditpaymentmethod(int OrderId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);

            }
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
                }
                var PaymentMethod = Request.Form["PaymentMethod"];
                var PaymentMethodId = int.Parse(PaymentMethod);

                model.PaymentMethodId = PaymentMethodId;


                var UpdatedOrder = _context.Order.Attach(model);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Payment Method Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);

        }
        public async Task<IActionResult> OnPostEditOrderStatus(int OrderId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);

            }
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
                }
                var orderstatus = Request.Form["OrderStatus"];
                var orderStatusId = int.Parse(orderstatus);

                model.OrderStatusId = orderStatusId;

                OrderTraking orderTrakingObj = new OrderTraking()
                {
                    OrderId = OrderId,
                    ActionDate = DateTime.Now,
                    Remarks = "",
                    OrderStatusId = orderStatusId
                };
                _context.OrderTrakings.Add(orderTrakingObj);


                var UpdatedOrder = _context.Order.Attach(model);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Order Status Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);

        }
        public IActionResult OnGetSingleCustomerForEdit(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
            var customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);
           
            customerVM = new CustomerVM()
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                Mobile = customer.Phone,
                Email = customer.Email,
                OrderId = OrderId

            };

            return new JsonResult(customerVM);

        }
        public IActionResult OnGetSingleCustomerAddressForEdit(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();

            customerVM = new CustomerVM()
            {

                Address = customerAddress.Address != null ? customerAddress.Address : "",
                CountryId = customerAddress.CountryId.Value != null ? customerAddress.CountryId.Value : 0,
                AreaName = customerAddress.AreaName != null ? customerAddress.AreaName : "",
                BuildingNo = customerAddress.BuildingNo != null ? customerAddress.BuildingNo : "",
                CityName = customerAddress.CityName != null ? customerAddress.CityName : "",
                CustomerAddressId = customerAddress.CustomerAddressId != null ? customerAddress.CustomerAddressId : 0,

            };


            return new JsonResult(customerVM);

        }

        public async Task<IActionResult> OnPostEditCustomerAddress(int OrderId)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
            }
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
                if (customerAddress != null)
                {
                    customerAddress.Address = customerVM.Address;
                    customerAddress.AreaName = customerVM.AreaName;
                    customerAddress.BuildingNo = customerVM.BuildingNo;
                    customerAddress.CityName = customerVM.CityName;
                    var UpdatedCustomerAddress = _context.customerAddresses.Attach(customerAddress);
                    UpdatedCustomerAddress.State = EntityState.Modified;
                }

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Customer Address Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
        }


        public async Task<IActionResult> OnPostEditCustomer(int OrderId)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
            }
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
                var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
                var model = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

                var userExists = await _userManager.FindByEmailAsync(customerVM.Email);
                if (userExists != null)
                {
                    _toastNotification.AddErrorToastMessage("Email is already exist");
                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);

                }

                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Customer Not Found");

                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                var countryob = _context.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault().CountryTlen;


                if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                {
                    if (order.CountryId == 2 && customerVM.Mobile.StartsWith("0"))
                    {
                        customerVM.Mobile = customerVM.Mobile.Substring(1);
                    }
                    if (customerVM.Mobile.StartsWith(countryCode))
                    {
                        // User entered phone number with country code, remove the duplicate country code
                        customerVM.Mobile = customerVM.Mobile.Substring(countryCode.Length);
                    }
                    customerVM.Mobile = countryCode + customerVM.Mobile;
                }
                if (user != null)
                {
                    user.PhoneNumber = customerVM.Mobile;
                    user.FullName = customerVM.CustomerName;
                    user.Email = customerVM.Email;
                    await _userManager.UpdateAsync(user);

                }

                model.Email = customerVM.Email;
                model.Phone = customerVM.Mobile;
                model.CustomerName = customerVM.CustomerName;

                var Updatedcoupon = _context.CustomerNs.Attach(model);

                Updatedcoupon.State = EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Customer Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
        }

        public async Task<IActionResult> OnPostEditorder(int OrderId)
        {
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");
                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
                }

                if (editOrder.DiscountAmount > model.OrderNet)
                {
                    ModelState.AddModelError("Order.Discount", "Discount cannot exceed net total.");
                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
                }

                string date = Request.Form["datepicker"];
                Console.WriteLine("Date from Form: " + date); // Debugging statement
                var datt = Convert.ToDateTime(date);


                model.OrderDate = datt;
                //if (DateTime.TryParseExact(date, "d MMM yyyy", CultureInfo.InvariantCulture,  out var parsedDate))
                //{
                //    model.OrderDate = parsedDate;

                //}



                // Assuming the 'Order' entity has a property named 'OrderDate' to store the date.
                // If it's a different property name, replace 'OrderDate' accordingly.

                model.DiscountAmount = editOrder.DiscountAmount;

                    // No need to attach the entity since you already retrieved it from the context.

                    _context.SaveChanges();
                    Recalculate(OrderId);
                    _toastNotification.AddSuccessToastMessage("Order Edited successfully");
                
               
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }

            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
        }
        private bool TryParseCustomDate(string input, out DateTime result)
        {
            // Define an array of possible month names
            string[] monthNames = {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

            // Split the input by space
            string[] parts = input.Split(' ');

            if (parts.Length == 3)
            {
                // Try to parse the month, year, and day parts
                int monthIndex = Array.IndexOf(monthNames, parts[0]);

                if (monthIndex != -1 && int.TryParse(parts[2], out int year) && int.TryParse(parts[1], out int day))
                {
                    // Successfully parsed the date
                    result = new DateTime(year, monthIndex + 1, day);
                    return true;
                }
            }

            // Parsing failed
            result = DateTime.MinValue;
            return false;
        }

        public IActionResult OnGetSingleItemForEdit(int OrderId, int ItemId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            orderItem = _context.OrderItem.Where(e => e.ItemId == ItemId && e.OrderId == order.OrderId).FirstOrDefault();


            //if (order != null)
            //{
            //    editOrder = new ViewModels.EditOrder()
            //    {
            //        OrderId = order.OrderId,

            //        DiscountAmount = order.DiscountAmount != null ? order.DiscountAmount.Value : 0, // Handle the case when DiscountAmount is null
            //        OrderDate = order.OrderDate
            //    };


            return new JsonResult(orderItem);
        }


        public async Task<IActionResult> OnPostEditItem(int OrderId, int ItemId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);

            }
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                var model = _context.OrderItem.Where(e => e.ItemId == ItemId && e.OrderId == order.OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
                }



                var item = Request.Form["item"];
                var itemId = int.Parse(item);
                model.ItemId = itemId;
                var itemprice = _context.itemPriceNs.Where(e => e.ItemId == itemId && e.CountryId == order.CountryId).FirstOrDefault().Price;

                model.ItemQuantity = orderItem.ItemQuantity;
                model.ItemPrice = itemprice.Value;
                model.Total = model.ItemPrice * model.ItemQuantity;
                var UpdatedorderItem = _context.OrderItem.Attach(model);
                UpdatedorderItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                
                _context.SaveChanges();
                order.OrderTotal = _context.OrderItem.Where(oi => oi.OrderId == order.OrderId).Sum(oi => oi.Total); ;
                var Updatedorder = _context.Order.Attach(order);
                Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
                Recalculate(order.OrderId);


                _toastNotification.AddSuccessToastMessage("Item Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);

        }

        public IActionResult OnGetSingleItemForDelete(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            orderItem = _context.OrderItem.Where(e => e.OrderId == order.OrderId).FirstOrDefault();
            return new JsonResult(orderItem);
        }

        public async Task<IActionResult> OnPostDeleteOrderItem(int OrderId)
        {
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                orderItem = _context.OrderItem.Where(e => e.OrderId == order.OrderId).FirstOrDefault();

                var orderItemsList = _context.OrderItem.Where(e => e.OrderId == order.OrderId).ToList();
                if (orderItemsList.Count == 0)
                {
                    _context.Order.Remove(order);

                    await _context.SaveChangesAsync();
                    return Redirect("/Admin/Configurations/ManageOrder/Orders");

                }

                if (orderItem != null)
                {


                    _context.OrderItem.Remove(orderItem);

                    await _context.SaveChangesAsync();
                    
                    order.OrderTotal = _context.OrderItem.Where(oi => oi.OrderId == order.OrderId).Sum(oi => oi.Total); ;
                    var Updatedorder = _context.Order.Attach(order);
                    Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    Recalculate(order.OrderId);

                    _toastNotification.AddSuccessToastMessage("order Item Deleted successfully");


                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
                }
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }

            return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + OrderId);
        }
        public void Recalculate(int OrderId)
        {
            var order = _context.Order.Where(e => e.OrderId == OrderId).Include(a => a.OrderItem).FirstOrDefault();
            if (order != null)
            {

                shippingMatrices = _context.ShippingsMatrix.Where(e => e.CountryId == order.CountryId).ToList();
                order.TotalWeight = 0;
                foreach (var orderItem in order.OrderItem)
                {
                    var itemWeight = _context.Item.Where(e => e.ItemId == orderItem.ItemId).FirstOrDefault().Weight;
                    if (itemWeight != null)
                    {
                        order.TotalWeight += (double)(itemWeight * orderItem.ItemQuantity);
                    }
                }
                foreach (var shipCost in shippingMatrices)
                {
                    if (order.TotalWeight == shipCost.ToWeight || order.TotalWeight == shipCost.FromWeight || (order.TotalWeight > shipCost.FromWeight && order.TotalWeight < shipCost.ToWeight) || (order.TotalWeight < shipCost.FromWeight && order.TotalWeight > shipCost.ToWeight))
                    {
                        DeliveryCost = shipCost.ActualPrice;
                    }

                }

                order.Deliverycost = DeliveryCost;
                if (order.OrderItem != null)
                {
                    var UpdatedorderItems = _context.Order.Attach(order);
                    UpdatedorderItems.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                }

                var subTotal = order.OrderTotal;
                var ShippingCost = order.Deliverycost;
                var tax = order.tax;
                var couponDiscount = order.OrderDiscount;
                var discountAmount = order.DiscountAmount;
                double Discount = 0;
                double TotalAmountAfterDiscount = 0;
                double sumItemTotal = subTotal;
                var percent = sumItemTotal / subTotal;

                if (order.CouponId == null)
                {
                    Discount = 0;
                    TotalAmountAfterDiscount = sumItemTotal;
                }
                else if (order.CouponTypeId == 2)
                {

                    Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - order.CouponAmount * percent), 2);

                    var AmountAfterDiscount = (double)(sumItemTotal - order.CouponAmount * percent);
                    TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
                }
                else
                {

                    var couponAmount = subTotal * (order.CouponAmount / 100);
                    Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - couponAmount * percent), 2);
                    var AmountAfterDiscount = (double)(sumItemTotal - couponAmount * percent);
                    TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
                }
                if (TotalAmountAfterDiscount < 0)
                {
                    TotalAmountAfterDiscount = 0;
                }

             var OrderTaxValue = (TotalAmountAfterDiscount * tax);
                //var TotalAfterDiscount = order.TotalAfterDiscount;
                var NetOrder = tax == 0 ? (TotalAmountAfterDiscount + ShippingCost) : ((TotalAmountAfterDiscount + ShippingCost + OrderTaxValue));
                var finalorderNet = NetOrder - discountAmount;
                order.OrderNet = Math.Round((double)finalorderNet, 2);
                order.TotalAfterDiscount = TotalAmountAfterDiscount;
                var Updatedorder = _context.Order.Attach(order);
                Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
        }
        private readonly Dictionary<string, string> CountryCodeMappings = new Dictionary<string, string>
        {
            { "BH", "+973" },
            { "KW", "+965" },
            { "OM", "+968" },
            { "QA", "+974" },
            { "SA", "+966" },
            { "AE", "+971" }
        };


    }
}
