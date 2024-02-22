
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRM.ViewModels;
using Newtonsoft.Json;
using RestSharp;

namespace CRM.Areas.Admin.Pages.Order
{
    
    public class OrderManagerModel : PageModel
    {
        public Models.Order order { get; set; }
        private PerfumeContext _context;
        public List<OrderFiltterModel> OrderVmList { get; set; }
        public List<Country> countries { get; set; }
        public List<OrderStatus> statuses { get; set; }
        public int InitatedCount { get; set; }
        public int Notpaid { get; set; }
        public int Processing { get; set; }
        public int Packing { get; set; }
        public int Ondelivery { get; set; }

        public HttpClient httpClient { get; set; }

        public int PaidCount { get; set; }

        [BindProperty]
        public OrderFilterVM orderFilter { get; set; }
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
       
    
        public OrderManagerModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
            order = new Models.Order();
        }


        public void OnGet()
        {
            countries = _context.Country.ToList();
            statuses = _context.OrderStatuses.Where(e => e.OrderStatusId != 4).ToList();
            InitatedCount = _context.Order.Where(e => e.OrderStatusId == 1).Count();
            PaidCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();
            Notpaid = _context.Order.Where(e => e.OrderStatusId == 3).Count();
            Processing = _context.Order.Where(e => e.OrderStatusId == 6).Count();
            Packing = _context.Order.Where(e => e.OrderStatusId == 7).Count();
            Ondelivery = _context.Order.Where(e => e.OrderStatusId == 5).Count();
        }

        public async Task <IActionResult> OnGetSendSMSForOrder(int OrderId)
        {
            var order = _context.Order.Where(e => e.OrderId == OrderId).FirstOrDefault();
            var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
            var Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);
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
            

            catch (Exception e)
            {
                return new JsonResult("False");

            }

            return new JsonResult("true");
        }

        public IActionResult OnGetSingleorderstatusForEdit(int OrderId)
        {
            order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(order);

        }
        public async Task<IActionResult> OnPostEditOrderStatus(int OrderId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Order/OrderManager");

            }
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Order/OrderManager");
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

                return Redirect("/Admin/Order/OrderManager");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Order/OrderManager");

        }

        public IActionResult OnGetPrint(int orderId)
        {
            var Result = _context.Order.Where(c => c.OrderId == orderId).FirstOrDefault();
            return new JsonResult(Result);
        }
        public ActionResult OnPost()
        {

            try
            {
                countries = _context.Country.ToList();
                statuses = _context.OrderStatuses.ToList();
                InitatedCount = _context.Order.Where(e => e.OrderStatusId == 1).Count();
                PaidCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();
                Notpaid = _context.Order.Where(e => e.OrderStatusId == 3).Count();
                Processing = _context.Order.Where(e => e.OrderStatusId == 6).Count();
                Packing = _context.Order.Where(e => e.OrderStatusId == 7).Count();
                Ondelivery = _context.Order.Where(e => e.OrderStatusId == 5).Count();
                OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderStatusId == 2).Select(i => new OrderFiltterModel
                {
                    OrderDate = i.OrderDate,
                    Country = i.Country.CountryTlen,
                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                    CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                    NetOrder = i.OrderNet.Value,
                    SerialNo = i.OrderSerial,
                    Status = i.OrderStatus.Status,
                    statusId = i.OrderStatus.OrderStatusId,
                    CountryId = i.CountryId.Value,
                    OrderId = i.OrderId,

                }).ToList();

                DateTime startFromToDate = DateTime.Now;

                if (orderFilter.date != null)
                {


                    if (orderFilter.date.Contains("to"))
                    {

                        var date = orderFilter.date.Split("to");
                        var startFrom = date[0];
                        var toEnd = date[1];
                        startFromToDate = DateTime.Parse(startFrom).Date;
                        var toEndToDate = DateTime.Parse(toEnd).Date;
                        if (startFromToDate != null && toEndToDate != null)
                        {
                            OrderVmList = OrderVmList.Where(a => a.OrderDate >= startFromToDate && a.OrderDate.Date <= toEndToDate).ToList();
                        }
                    }
                    else
                    {
                        startFromToDate = DateTime.Parse(orderFilter.date).Date;
                        OrderVmList = OrderVmList.Where(a => a.OrderDate.Date == startFromToDate).ToList();

                    }
                }
                if (orderFilter.status != 0)
                {

                    OrderVmList = OrderVmList.Where(a => a.statusId == orderFilter.status).ToList();
                }
                if (orderFilter.country != 0)
                {

                    OrderVmList = OrderVmList.Where(a => a.CountryId == orderFilter.country).ToList();
                }


            }
            catch (Exception)
            {

            }
            return Page();
        }
       

        public IActionResult OnGetSingleOrderDetailsForView(int OrderId)
        {
            var OrderReceipt = _context.Order.Include(a => a.OrderStatus).Where(e => e.OrderId == OrderId).FirstOrDefault();
           var OrderList = _context.OrderItem.Include(i => i.Item).Include(i => i.Order)
                                           .ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN)
                                       .Where(o => o.OrderId == OrderId).Select(e=> new
                                       {
                                           ItemImage=  e.Item.ItemImage,
                                           ItemTitleAr= e.Item.ItemTitleAr,
                                           ItemTitleEn=   e.Item.ItemTitleEn,
                                          ItemPrice= e.ItemPrice,
                                           ItemQuantity=  e.ItemQuantity,
                                           Total= e.Total
                                       })
                                       
                                       
                                       .ToList();
           var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == OrderReceipt.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
           var Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);
            var OrderTraking = _context.OrderTrakings.Include(i => i.OrderStatus).Where(e => e.OrderId == OrderId)
                .Select(e => new
                {
                    Status = e.OrderStatus.Status,
                    ActionDate = e.ActionDate.ToShortDateString(),
                  
                })
                .ToList();
            var DiscountAmount = (OrderReceipt.DiscountAmount.HasValue ? OrderReceipt.DiscountAmount.Value : 0) + (OrderReceipt.CouponAmount.HasValue ? (OrderReceipt.CouponTypeId == 1 ? (OrderReceipt.CouponAmount.Value / 100) * OrderReceipt.OrderTotal : OrderReceipt.CouponAmount.Value) : 0);

            var currencyName = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen;
            var currencyNameAR = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlar;
            var Tax = OrderReceipt.tax.Value!=null? OrderReceipt.tax.Value:0 ;
            var ShippingCost = OrderReceipt.Deliverycost.Value!= null?OrderReceipt.Deliverycost.Value:0;
            var OrdernetByTax = (OrderReceipt.OrderTotal * Tax);
               double originalNumber = OrderReceipt.OrderNet.Value;
            int decimalPlaces = 2;
            var approximatedNumber = Math.Round(originalNumber, decimalPlaces);
            var taxpercentage = Tax * 100;
            var result = new { OrderReceipt, OrderList, customerAddress, Customer, OrderTraking, DiscountAmount, currencyName, currencyNameAR, Tax, ShippingCost, OrdernetByTax, approximatedNumber, taxpercentage };

            return new JsonResult(result);
        }


    }
}
