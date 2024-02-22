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

namespace CRM.Areas.Admin.Pages.Configurations.ManageCreateOrder
{
    public class CreateOrderModel : PageModel
    {
        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;


        //[BindProperty(SupportsGet = true)]
        //public string CouponSerial { get; set; }
        

        [BindProperty]
        public FastOrderVM FastOrderVM { get; set; }
       

        public CreateOrderModel( PerfumeContext perfumeContext, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;
            this.userManager = userManager;
            this._configuration = configuration;
            FastOrderVM = new FastOrderVM();

        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            try
            {

                var user = await userManager.GetUserAsync(User);


                var Country = Request.Form["Country"];
                var CountryId = int.Parse(Country);
                var customerObj = perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return NotFound();

                }


                var tax = perfumeContext.Country.Where(c => c.CountryId == CountryId).FirstOrDefault().tax;
                var shippingCost = perfumeContext.Country.Where(c => c.CountryId == CountryId).FirstOrDefault().ShippingCost;
                var countryob = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;


                if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                {
                    if (Country == "2" && FastOrderVM.PhoneNumber.StartsWith("0"))
                    {
                        FastOrderVM.PhoneNumber = FastOrderVM.PhoneNumber.Substring(1);
                    }
                    if (FastOrderVM.PhoneNumber.StartsWith(countryCode))
                    {
                        // User entered phone number with country code, remove the duplicate country code
                        FastOrderVM.PhoneNumber = FastOrderVM.PhoneNumber.Substring(countryCode.Length);
                    }
                    FastOrderVM.PhoneNumber = countryCode + FastOrderVM.PhoneNumber;
                }
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
                    CountryId = CountryId,
                    Mobile = FastOrderVM.PhoneNumber
                };
                perfumeContext.customerAddresses.Add(CustomerAddress);
                perfumeContext.SaveChanges();



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
                

                var orders =
                new Models.Order
                {
                    OrderDate = DateTime.Now,
                    OrderSerial = Convert.ToString(maxserial + 1),
                    CustomerId = customer.CustomerId,
                    CustomerAddressId = CustomerAddress.CustomerAddressId,

                    Deliverycost = shippingCost,
                    //OrderNet = TotalAmountAfterDiscount + shoppingCost + CountryObj.tax,

                    OrderStatusId = 1,
                    CountryId = CountryId,
                    tax = tax,
                    UniqeId = maxUniqe + 1,

                };


                perfumeContext.Order.Add(orders);
                perfumeContext.SaveChanges();
                return Redirect("/Admin/Configurations/ManageCreateOrder/AddOrderItems?Id=" + orders.OrderId);

            }
            catch (Exception e)
            {
                toastNotification.AddErrorToastMessage("Something went wrong");
                return RedirectToPage("/Configurations/ManageCreateOrder/CreateOrder");

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
