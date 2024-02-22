using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using CRM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using CRM.Data;
using CRM.ViewModels;
using NToastNotify;

namespace CRM.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly PerfumeContext perfumeContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly PerfumeContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IToastNotification _toastNotification;
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            PerfumeContext perfumeContext,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            PerfumeContext context,
            RoleManager<IdentityRole> roleManager
             , IToastNotification toastNotification,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            this.perfumeContext = perfumeContext;
            _context = context;
            _roleManager = roleManager;
            _toastNotification = toastNotification;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required (ErrorMessage = "Enter your Email")]
            [EmailAddress (ErrorMessage = "Not a vaild Email address")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required (ErrorMessage = "Enter your Full name")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Is Required"), RegularExpression("^[0-9]+$", ErrorMessage = " Accept Number Only")]
            [Display(Name = "Phone")]
            public string? Phone { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Enter your password")]
            [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            //[Required(ErrorMessage = "Confirm Password is required")]
            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            //public string ConfirmPassword { get; set; }

        }


        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(Input.Email);
                if (userExists != null)
                {
                    _toastNotification.AddErrorToastMessage("Email is already exist");
                    return Page();
                }
                var Country = HttpContext.Session.GetString("country");
                var countryob = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;


                if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                {
                    if (Country == "2" && Input.Phone.StartsWith("0"))
                    {
                        Input.Phone = Input.Phone.Substring(1);
                    }
                    if (Input.Phone.StartsWith(countryCode))
                    {
                        // User entered phone number with country code, remove the duplicate country code
                        Input.Phone = Input.Phone.Substring(countryCode.Length);
                    }
                    Input.Phone = countryCode + Input.Phone;
                }
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, FullName = Input.FullName, PhoneNumber = Input.Phone };
                var result = await _userManager.CreateAsync(user, Input.Password);
               
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var customer = new CustomerN()
                    {
                        Email = Input.Email,
                        RegisterDate = DateTime.Now,
                        CustomerName = Input.FullName,
                        Phone = Input.Phone,

                    };
                    _context.CustomerNs.Add(customer);
                    _context.SaveChanges();
                    await _userManager.AddToRoleAsync(user, "Customer");
                    
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            // If we got this far, something failed, redisplay form
            return Redirect("/");
        }

        //public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~/");
        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        //    if (ModelState.IsValid)
        //    {
        //        var user = CreateUser();

        //        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        //        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        //        var result = await _userManager.CreateAsync(user, Input.Password);

        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User created a new account with password.");
        //            var customer = new CustomerN()
        //            {
        //                Email = Input.Email,
        //                RegisterDate = DateTime.Now,
        //                CustomerName = Input.FullName,
        //                Phone = Input.Phone,

        //            };
        //            _context.CustomerNs.Add(customer);
        //            _context.SaveChanges();
        //            await _userManager.AddToRoleAsync(user, "Customer");

        //        }
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return Page();
        //}

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
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
