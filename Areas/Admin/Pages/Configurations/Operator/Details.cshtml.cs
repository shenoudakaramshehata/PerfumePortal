using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit;
using Newtonsoft.Json;
using NToastNotify;
using RestSharp;

namespace CRM.Areas.Admin.Pages.Configurations.Operator
{
    public class DetailsModel : PageModel
    {
        private readonly PerfumeContext _context;


        public IEnumerable<OrderItem> OrderList { get; set; }
        public CRM.Models.Order OrderReceipt { get; set; }
        [BindProperty]
        public CRM.Models.Order order { get; set; }
        public CustomerAddress customerAddress { get; set; }
        public CustomerN Customer { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodAR { get; set; }
        public double tax { get; set; }
        public HttpClient httpClient { get; set; }

        public string CurrencyEN { get; set; }
        public string CurrencyNameAr { get; set; }
        public double DiscountAmount { get; set; }
        public double DeliveryCost { get; set; }
        public double OrdernetByTax { get; set; }
        public double approximatedNumber { get; set; }
        public double taxpercentage { get; set; }
        public List<OrderTraking> OrderTraking { get; set; }

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public DetailsModel(PerfumeContext perfumeContext, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            httpClient = new HttpClient();
            _context = perfumeContext;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            order = new Models.Order();
        }

        public void OnGet([FromQuery] int Id)
        {
            OrderList = _context.OrderItem.Include(i => i.Item).Include(i => i.Order)
                                            .ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN)
                                        .Where(o => o.OrderId == Id).ToList();

            

            OrderReceipt = _context.Order.Where(i => i.OrderId == Id).Include(a => a.OrderStatus).FirstOrDefault();

            customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == OrderReceipt.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
            Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

            CurrencyEN = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen;
                CurrencyNameAr = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlar;
                tax = OrderReceipt.tax.Value != null ? OrderReceipt.tax.Value : 0;
            DeliveryCost = OrderReceipt.Deliverycost.Value != null ? OrderReceipt.Deliverycost.Value : 0;
            OrdernetByTax = OrderReceipt.OrderTotal * tax;
                double originalNumber = OrderReceipt.OrderNet.Value;
                int decimalPlaces = 2;
                approximatedNumber = Math.Round(originalNumber, decimalPlaces);
            DiscountAmount = (OrderReceipt.DiscountAmount.HasValue ? OrderReceipt.DiscountAmount.Value : 0) + (OrderReceipt.CouponAmount.HasValue ? (OrderReceipt.CouponTypeId == 1 ? (OrderReceipt.CouponAmount.Value / 100) * OrderReceipt.OrderTotal : OrderReceipt.CouponAmount.Value) : 0);
            //DiscountAmount = (OrderReceipt.DiscountAmount != null ? OrderReceipt.DiscountAmount.Value : 0) + (OrderReceipt.CouponAmount.Value == null ? 0 : OrderReceipt.CouponTypeId == 1 ? (OrderReceipt.CouponAmount.Value / 100) * OrderReceipt.OrderTotal : OrderReceipt.CouponAmount.Value);
            taxpercentage = tax * 100;

            OrderTraking = _context.OrderTrakings.Include(i => i.OrderStatus).Where(e => e.OrderId == Id).ToList();
            PaymentMethod= _context.paymentMehods.Where(e=>e.PaymentMethodId== OrderReceipt.PaymentMethodId).FirstOrDefault().PaymentMethodEN;
            PaymentMethodAR= _context.paymentMehods.Where(e => e.PaymentMethodId == OrderReceipt.PaymentMethodId).FirstOrDefault().PaymentMethodAR;
        }
        public IActionResult OnGetPrint(int OrderId)
        {
            var Result = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(Result);
        }
        public async Task<IActionResult> OnPost(IFormFile file, int Id)
        {
            try
            {
                var orderObj = _context.Order.Where(e => e.OrderId == Id).FirstOrDefault();
                if (orderObj != null)
                {


                    orderObj.OrderStatusId = 6;
                    if (file != null)
                    {
                        string folder = "images/ShippingNo/";

                        orderObj.ShippingLabel = UploadImage(folder, file);
                    }
                    orderObj.ShippingNo = order.ShippingNo;

                    OrderTraking orderTrakingObj = new OrderTraking()
                    {
                        OrderId = Id,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Is Processing",
                        OrderStatusId = 6
                    };
                    _context.OrderTrakings.Add(orderTrakingObj);


                    var UpdatedOrder = _context.Order.Attach(orderObj);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    

                    //var SMSMessage = new
                    //{
                    //    username = "mashaer",
                    //    password = "5!V[Ej4o",
                    //    type = 0,
                    //    dlr = 1,
                    //    destination = Customer.Phone.Split("+")[1],
                    //    message = "Your order on Mashaer perfumes is now with Shipping company."
                    //};
                    //var sendPaymentRequestJSON = JsonConvert.SerializeObject(SMSMessage);

                    //var client = new RestClient("http://api.rmlconnect.net/bulksms/bulksms");
                    //var request = new RestRequest();
                    //request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", sendPaymentRequestJSON, ParameterType.RequestBody);
                    //RestResponse response = await client.PostAsync(request);
                    //var stringrespone = response.Content.ToString();
                    _toastNotification.AddSuccessToastMessage("Order Updated Successfully");
                }
            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage(e.Message);
            }


            return RedirectToPage("/Configurations/Operator/Orders");
        }

        public async Task<IActionResult> OnPostEditShippingInfo(IFormFile Editfile, int Id)
        {
            try
            {
                var orderObj = _context.Order.Where(e => e.OrderId == Id).FirstOrDefault();
                if (orderObj != null)
                {
                    if (Editfile != null)
                    {
                        string folder = "images/ShippingNo/";

                        orderObj.ShippingLabel = UploadImage(folder, Editfile);
                    }
                    else
                    {
                        orderObj.ShippingLabel = orderObj.ShippingLabel;
                    }

                    if (order.ShippingNo != null)
                    {
                        orderObj.ShippingNo = order.ShippingNo;
                    }
                    else
                    {
                        orderObj.ShippingNo = orderObj.ShippingNo;
                    }

                    var UpdatedOrder = _context.Order.Attach(orderObj);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("ShippingInfo Updated Successfully");
                }
            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage(e.Message);
            }


            return RedirectToPage("/Configurations/Operator/Orders");
        }
        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
