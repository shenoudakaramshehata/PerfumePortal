using CRM.Data;
using CRM.Data.Migrations;
using CRM.Migrations;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using NuGet.Packaging;
using System.Reflection.Metadata.Ecma335;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace CRM.Areas.Admin.Pages.Configurations.Operator
{
    public class CustomersModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<ApplicationUser> _userManager;
        public string url { get; set; }
        [BindProperty]
      public CustomerVM customerVM { get; set; }
        private readonly ApplicationDbContext _db;
        public List<CustomerN>customerNs { get; set; }
        public CustomersModel(PerfumeContext context
                        , IWebHostEnvironment hostEnvironment
                        , IToastNotification toastNotification
            , UserManager<ApplicationUser> userManager,

            ApplicationDbContext db
            )

        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _db = db;
            _toastNotification = toastNotification;
            customerNs = new List<CustomerN>();
            customerVM = new CustomerVM();
        }
        public async Task<IActionResult> OnGet()
        {

            url = $"{this.Request.Scheme}://{this.Request.Host}";
            customerNs = _context.CustomerNs.ToList();
           
            return Page();
        }

        public IActionResult OnGetSingleCustomerForEdit(int CustomerId)
        {
            var customer = _context.CustomerNs.Where(c => c.CustomerId == CustomerId).FirstOrDefault();
            var customerAddress= _context.customerAddresses.Where(e=>e.CustomerId== CustomerId).FirstOrDefault();
            if (customerAddress == null)
            {
               
                customerVM = new CustomerVM()
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    Mobile = customer.Phone,
                    Email = customer.Email,
                    Address =  "",
                    CountryId =  0,
                    AreaName =  "",
                    BuildingNo =  "",
                    CityName =  "",
                    
                };

            }
            else
            {
                customerVM = new CustomerVM()
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    Mobile = customer.Phone,
                    Email = customer.Email,
                    Address = customerAddress.Address != null ? customerAddress.Address : "",
                    CountryId = customerAddress.CountryId.Value != null ? customerAddress.CountryId.Value : 0,
                    AreaName = customerAddress.AreaName != null ? customerAddress.AreaName : "",
                    BuildingNo = customerAddress.BuildingNo != null ? customerAddress.BuildingNo : "",
                    CityName = customerAddress.CityName != null ? customerAddress.CityName : "",
                    CustomerAddressId = customerAddress.CustomerAddressId != null ? customerAddress.CustomerAddressId : 0,

                };
            }

            

            return new JsonResult(customerVM);

        }


        public async Task<IActionResult> OnPostEditCustomer(int CustomerId)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/Operator/Customers");
            }
            try
            {

                var model = await _context.CustomerNs.Where(c => c.CustomerId == CustomerId).FirstOrDefaultAsync();
                var customerAddress = _context.customerAddresses.Where(e => e.CustomerId == CustomerId).FirstOrDefault();

                var userExists = await _userManager.FindByEmailAsync(customerVM.Email);
                if (userExists != null)
                {
                    _toastNotification.AddErrorToastMessage("Email is already exist");
                    return Page();
                }
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Customer Not Found");

                    return Redirect("/Admin/Configurations/Operator/Customers");
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                var Country = HttpContext.Session.GetString("country");
                var countryob = _context.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;


                if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                {
                    if (Country == "2" && customerVM.Mobile.StartsWith("0"))
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

                if (customerAddress == null)
                {
                    var Address = new CustomerAddress()
                    {
                        Address = customerVM.Address,
                        CountryId = customerVM.CountryId,
                        AreaName = customerVM.AreaName,
                        BuildingNo = customerVM.BuildingNo,
                        CityName = customerVM.CityName,
                        CustomerId = model.CustomerId,
                        Mobile = customerVM.Mobile,
                    };
                    _context.customerAddresses.Add(Address);



                }
                else
                {
                    customerAddress.Address = customerVM.Address;
                    customerAddress.CountryId = customerVM.CountryId;
                    customerAddress.AreaName = customerVM.AreaName;
                    customerAddress.BuildingNo = customerVM.BuildingNo;
                    customerAddress.CityName = customerVM.CityName;
                    var UpdatedCustomerAddress = _context.customerAddresses.Attach(customerAddress);

                    Updatedcoupon.State = EntityState.Modified;
                }
                ;

               
                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Customer Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/Customers");
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
