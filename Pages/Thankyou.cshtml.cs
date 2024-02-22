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

namespace CRM.Pages
{
    public class ThankyouModel : PageModel
    {

        public int OrderNo { get; set; }
        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;

        //[BindProperty(SupportsGet = true)]
        //public string CouponSerial { get; set; }

       


       

        public ThankyouModel( PerfumeContext perfumeContext, IToastNotification toastNotification, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
           
            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;
           
            this._configuration = configuration;
            _hostEnvironment = hostEnvironment;
           

        }

        public async Task<IActionResult> OnGet(int orderId)
        {
            OrderNo = orderId;
            var order = perfumeContext.Order.Include(e=>e.OrderItem).Where(e => e.OrderId == orderId).FirstOrDefault();

            order.IsPaid = true;


            order.OrderStatusId = 2;
            var UpdatedOrder = perfumeContext.Order.Attach(order);
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
            var Customer = perfumeContext.CustomerNs.Where(e => e.CustomerId == order.CustomerId).FirstOrDefault();

            try
            {
                if (Customer != null)
                {
                    var options = new RestClientOptions("https://api.rmlconnect.net")
                    {
                        MaxTimeout = -1,
                    };
                    var clientOption = new RestClient(options);
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




                    RestResponse response = await clientOption.ExecuteAsync(request);
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
            }
            catch (Exception e)
            {
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnGetPayment(int orderId)
        {
            var order = perfumeContext.Order.Include(e => e.OrderItem).Where(e => e.OrderId == orderId).FirstOrDefault();
            var country = perfumeContext.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault();
            var currency = perfumeContext.Currency.Where(e => e.CurrencyId == country.CurrencyId).FirstOrDefault().CurrencyTlar;
            var result = new { currency = currency, total = order.OrderNet };
            return new JsonResult(result);
        }


        }
    }

