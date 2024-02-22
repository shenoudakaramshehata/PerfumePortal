using CRM.Areas.Identity.Pages.Account;
using CRM.Data;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace CRM.Pages
{
    public class ViewProfileModel : PageModel
    {
        public CustomerN customer { get; set; }
        
        public List<Order> orders { get; set; }

        [BindProperty]
        public CustomerAddress? customerAddr { get; set; }
        [BindProperty]
        public CustomerAddressVM newCustomerAddressVM { get; set; }

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        private readonly PerfumeContext _context;

        [BindProperty]
        public resetPasswordVM resetPasswordVM { get; set; }
        
        public ViewProfileModel(PerfumeContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _context = Context;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _toastNotification = toastNotification;
            resetPasswordVM = new resetPasswordVM();
            customer = new CustomerN();
        }
        public async Task<IActionResult> OnGet()
       {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {

                return Redirect("~/login");
            }

            customer = _context.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault();
            
            var customerId = _context.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;

            customerAddr = _context.customerAddresses.Where(i => i.CustomerId == customerId).FirstOrDefault();

            orders =  _context.Order.Where(i => i.CustomerId == customerId).Include(i => i.OrderStatus).ToList();


            return Page();
        }


        public async Task<IActionResult> OnPostAddCustomerAddress()
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Page();
                }
                var Country = HttpContext.Session.GetString("country");

                var customerObj = _context.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return NotFound();

                }

                var AddressCustomer = _context.customerAddresses.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();
                if (AddressCustomer == null)
                {
                    var countryob = _context.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;


                    if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                    {
                        if (Country == "2" && customerAddr.Mobile.StartsWith("0"))
                        {
                            customerAddr.Mobile = customerAddr.Mobile.Substring(1);
                        }
                        if (customerAddr.Mobile.StartsWith(countryCode))
                        {
                            // User entered phone number with country code, remove the duplicate country code
                            customerAddr.Mobile = customerAddr.Mobile.Substring(countryCode.Length);
                        }
                        customerAddr.Mobile = countryCode + customerAddr.Mobile;
                    }
                    var customerAddress = new CustomerAddress()
                    {

                        CustomerId = customerObj.CustomerId,
                        Address = customerAddr.Address,
                        CityName = customerAddr.CityName,
                        AreaName = customerAddr.AreaName,
                        BuildingNo = customerAddr.BuildingNo,
                        Mobile = customerAddr.Mobile,
                        ZIPCode = customerAddr.ZIPCode,

                    };


                    _context.customerAddresses.Add(customerAddress);
                    _context.SaveChanges();

                    _toastNotification.AddSuccessToastMessage("Address Added Successfully");

                }
                else
                {
                    AddressCustomer.Address = customerAddr.Address;
                    AddressCustomer.AreaName = customerAddr.AreaName;
                    AddressCustomer.CityName = customerAddr.CityName;
                    AddressCustomer.BuildingNo = customerAddr.BuildingNo;
                    AddressCustomer.Mobile = customerAddr.Mobile;
                    AddressCustomer.ZIPCode = customerAddr.ZIPCode;


                    _context.Attach(AddressCustomer).State = EntityState.Modified;
                    _context.SaveChanges();

                    _toastNotification.AddSuccessToastMessage("Address Updated Successfully");

                }


                var existedAddressForCustomer = _context.customerAddresses.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();


                return RedirectToPage("/ViewProfile");
            }
            catch 
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
                return RedirectToPage("/ViewProfile");
            }
        }

        public async Task<IActionResult> OnPostEditPassword()
        {
            try
            {
                //if (!ModelState.IsValid)
                    //return Page();
                if (resetPasswordVM.ConfirmPassword != resetPasswordVM.NewPassword)
                {
                    _toastNotification.AddErrorToastMessage("Password not matched");
                    return RedirectToPage("/ViewProfile");
                }
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userid);
                var Result = await _userManager.ChangePasswordAsync(user, resetPasswordVM.CurrentPassword, resetPasswordVM.NewPassword);
                if (!Result.Succeeded)
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.TryAddModelError("", error.Description);
                        _toastNotification.AddErrorToastMessage(error.Description);
                    }
                   
                    return RedirectToPage("/ViewProfile");

                }

                _toastNotification.AddSuccessToastMessage("Password Changed");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
            }
            return RedirectToPage("/ViewProfile");
        }

        public async Task<IActionResult> ChangeImage()
        {
            return Page();
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




