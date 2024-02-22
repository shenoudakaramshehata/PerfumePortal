using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit;
using NToastNotify;
using RestSharp;

namespace CRM.Areas.Admin.Pages.Configurations.ManageStoredOrder
{
    public class DetailsModel : PageModel
    {
        private readonly PerfumeContext _context;


        public IEnumerable<OrderItem> OrderList { get; set; }
        public CRM.Models.Order OrderReceipt { get; set; }
        [BindProperty]
        public CRM.Models.Order order { get; set; }
        public double DiscountAmount { get; set; }

        public CustomerAddress customerAddress { get; set; }
        public CustomerN Customer { get; set; }
      
        public string CurrencyEN { get; set; }
        public double OrdernetByTax { get; set; }
        public double approximatedNumber { get; set; }
        public double tax { get; set; }
        public string CurrencyNameAr { get; set; }
        public double DeliveryCost { get; set; }
        public double taxpercentage { get; set; }
        public List<OrderTraking> OrderTraking { get; set; }

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public DetailsModel(PerfumeContext perfumeContext, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = perfumeContext;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            order = new Models.Order();
        }

        public void OnGet([FromQuery] int Id)
        {
            OrderList = _context.OrderItem.Include(i => i.Item).Include(i => i.Order)
                                            .ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN)
                                        .Where(o =>o.OrderId == Id).ToList();

            

            OrderReceipt = _context.Order.Where(i => i.OrderId == Id).Include(a => a.OrderStatus).FirstOrDefault();

            customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == OrderReceipt.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
            Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);
           

                CurrencyEN = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen;
                CurrencyNameAr = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlar;
                tax = OrderReceipt.tax.Value != null ? OrderReceipt.tax.Value : 0;
            DeliveryCost = OrderReceipt.Deliverycost.Value != null ? OrderReceipt.Deliverycost.Value : 0;
            OrdernetByTax = (OrderReceipt.OrderTotal * tax);
                double originalNumber = OrderReceipt.OrderNet.Value;
                int decimalPlaces = 2;
                approximatedNumber = Math.Round(originalNumber, decimalPlaces);

                DeliveryCost = _context.Country.Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().ShippingCost.Value;
            DiscountAmount = (OrderReceipt.DiscountAmount.HasValue ? OrderReceipt.DiscountAmount.Value : 0) + (OrderReceipt.CouponAmount.HasValue ? (OrderReceipt.CouponTypeId == 1 ? (OrderReceipt.CouponAmount.Value / 100) * OrderReceipt.OrderTotal : OrderReceipt.CouponAmount.Value) : 0);

            OrderTraking = _context.OrderTrakings.Include(i => i.OrderStatus).Where(e => e.OrderId == Id).ToList();
            taxpercentage = tax * 100;

        }
        public IActionResult OnGetPrint(int OrderId)
        {
            var Result = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(Result);
        }
        public async Task<IActionResult> OnPost(int Id)
        {
            try
            {
                var orderObj = _context.Order.Where(e => e.OrderId == Id).FirstOrDefault();
                if (orderObj != null)
                {
                    if (orderObj.OrderStatusId == 6)
                    {
                        orderObj.OrderStatusId = 7;

                        OrderTraking orderTrakingObj = new OrderTraking()
                        {
                            OrderId = Id,
                            ActionDate = DateTime.Now,
                            Remarks = "Order Is Packing",
                            OrderStatusId = 7
                        };
                        _context.OrderTrakings.Add(orderTrakingObj);


                        var UpdatedOrder = _context.Order.Attach(orderObj);
                        UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Order Updated Successfully");
                    }
                    else if (orderObj.OrderStatusId == 7)
                    {
                        orderObj.OrderStatusId = 5;

                        OrderTraking orderTrakingObj = new OrderTraking()
                        {
                            OrderId = Id,
                            ActionDate = DateTime.Now,
                            Remarks = "Order Is OnDelivery",
                            OrderStatusId = 5
                        };
                        _context.OrderTrakings.Add(orderTrakingObj);


                        var UpdatedOrder = _context.Order.Attach(orderObj);
                        UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();
                        var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == orderObj.CustomerAddressId).FirstOrDefault();
                        var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
                        var Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

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
           .AddParameter("message", "Your order on Mashaer perfumes is now with Shipping company.");
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
                            _toastNotification.AddErrorToastMessage("SMS doesnot Send");

                            return RedirectToPage("/Configurations/ManageStoredOrder/Orders");

                        }
                        _toastNotification.AddSuccessToastMessage("Order Updated Successfully");
                    }

                }
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
            }


            return RedirectToPage("/Configurations/ManageStoredOrder/Orders");
        }

    }
}
