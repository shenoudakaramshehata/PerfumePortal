using CRM.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.ViewModels;
using NToastNotify;
using CRM.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using DevExpress.XtraRichEdit.Import.Html;
using NuGet.ContentModel;
using System.Security.Claims;
using MailKit.Search;
using CRM.Migrations;

namespace CRM.Pages
{
    public class TRACKINGORDERModel : PageModel
    {
        private readonly PerfumeContext _perfumeContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
       
        public List<OrderTrackingVm> orderTrackingVms = new List<OrderTrackingVm>();
        [BindProperty]
        public SearchOrder SearchOrderVM { get; set; }
        //[BindProperty]
        //public List<OrderTraking> orderTrakings  { get; set; }
        public TRACKINGORDERModel(PerfumeContext perfumeContext, IToastNotification toastNotification, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _perfumeContext = perfumeContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _toastNotification = toastNotification;

        }
       
        public async Task<IActionResult> OnGet()
        {
            

            //try
            //{
            //    var user = await _userManager.GetUserAsync(User);
            //    if (user == null)
            //    {
            //        //_toastNotification.AddErrorToastMessage("You Must Login First");
            //        return Redirect("~/login");
            //    }
            //    var customer = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email).FirstOrDefault();
            //    if (customer == null)
            //    {
            //        //_toastNotification.AddErrorToastMessage("You Must Login First");
            //        return Redirect("~/login");
            //    }

            //    var UserId = customer.CustomerId;
            //    var LatestOrder = _perfumeContext.Order.Where(e => e.CustomerId == UserId).Include(e => e.CustomerAddress).OrderByDescending(e => e.OrderId).FirstOrDefault();
            //    if (LatestOrder != null)
            //    {
            //        var countryOrder = _perfumeContext.Country.Where(e => e.CountryId == LatestOrder.CountryId).Include(e => e.Currency).FirstOrDefault();
            //        //orderTrackingVms = _perfumeContext.OrderTrakings.Include(e => e.Order).Include(e => e.OrderStatus).Where(c => c.OrderId == LatestOrder.OrderId)
            //        //    .Select(c => new OrderTrackingVm
            //        //    {
            //        //        OrderSerial = c.Order.OrderSerial,
            //        //        CustomerName = customer.CustomerName,
            //        //        Country = countryOrder.CountryTlen,
            //        //        Address = LatestOrder.CustomerAddress.Address,
            //        //        OrderTotal = c.Order.OrderNet + " " + countryOrder.Currency.CurrencyTlen,
            //        //        Status = c.OrderStatus.Status,
            //        //        Remarks = c.Remarks,
            //        //        ActionDate = c.ActionDate.ToShortDateString(),
            //        //        ActionTime = c.ActionDate.TimeOfDay.Hours + " : " + c.ActionDate.TimeOfDay.Minutes,
            //        //    }).ToList();
            //    }
            //    else
            //    {
            //        _toastNotification.AddInfoToastMessage("No Order To Track");
            //    }
                
               
            //}
            //catch (Exception)
            //{
            //    _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again");

            //}

            return Page();


        }


        public async Task<IActionResult> OnPostSearchOrder([FromBody] string searchtext)
        {
            bool CheckSearchItem = false;
            int SearchItem = 0;
            CheckSearchItem = int.TryParse(searchtext, out SearchItem);



            orderTrackingVms = _perfumeContext.OrderTrakings.Include(e=>e.Order)
                    .Where(x =>x.OrderId == SearchItem ).Select(c => new OrderTrackingVm
                    {
                        OrderSerial = c.Order.OrderSerial,
                        CustomerName = _perfumeContext.CustomerNs.Where(e => e.CustomerId == c.Order.CustomerId).FirstOrDefault().CustomerName,
                        //Country = countryOrder.CountryTlen,
                        Address = c.Order.CustomerAddress.Address,
                        OrderTotal = c.Order.OrderNet.Value ,
                        Status = c.OrderStatus.Status,
                        Remarks = c.Remarks,
                        ActionDate = c.ActionDate.ToShortDateString(),
                    }).ToList();

                if (orderTrackingVms == null && orderTrackingVms.Count==0)
                {
                    _toastNotification.AddErrorToastMessage("This Order Not Found");
                    return Page();
                }
                return new JsonResult(orderTrackingVms);
            
        }

        //public string GetUserIpAddress()
        //{
        //    string Ip = Response.HttpContext.Connection.RemoteIpAddress.ToString();

        //    if (Ip == "::1")
        //    {
        //        Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        //    }
        //    return Ip;
        //}


        //public static string GetUserCountryByIp(string IpAddress)
        //{
        //    IpInfo ipInfo = new IpInfo();
        //    try
        //    {
        //        string info = new WebClient().DownloadString("http://ipinfo.io/" + IpAddress);
        //        ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
        //        RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
        //        ipInfo.Country = myRI1.EnglishName;
        //    }
        //    catch
        //    {
        //        ipInfo.Country = null;
        //    }

        //    return ipInfo.Country;
        //}
    }
}
