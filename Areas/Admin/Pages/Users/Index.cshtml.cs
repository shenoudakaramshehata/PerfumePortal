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

namespace CRM.Areas.Admin.Pages.Users
{
    public class IndexModel : PageModel
    {
		private PerfumeContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IToastNotification _toastNotification;
		public string url { get; set; }
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public List<IdentityRole> Roles { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
		public IList<ApplicationUser> Users { get; set; }
        private readonly ApplicationDbContext _db;


        [BindProperty]
        public ViewModels.Users UserVM { get; set; }
        [BindProperty]
        public UsersVM userProfileVM { get; set; }
        [BindProperty]
        public ApplicationUser User { get; set; }
        public IndexModel(PerfumeContext context
						, IWebHostEnvironment hostEnvironment,
            RoleManager<IdentityRole> roleManager
                        , IToastNotification toastNotification,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db
            , UserManager<ApplicationUser> userManager
			)
			
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _toastNotification = toastNotification;
			User = new ApplicationUser();
			UserVM = new ViewModels.Users();
            _signInManager = signInManager;
            userProfileVM = new UsersVM();
            Users = new List<ApplicationUser>();
        }
		public async Task<IActionResult> OnGet()
		{

			url = $"{this.Request.Scheme}://{this.Request.Host}";

			var rolesToSearch = new string[] { "Operator", "Store" };

			var operatorUsers = await _userManager.GetUsersInRoleAsync("Operator");
			var storeUsers = await _userManager.GetUsersInRoleAsync("Store");

			Users = operatorUsers.Concat(storeUsers).ToList();


            var operatorRole = await _roleManager.FindByNameAsync("Operator");
            var storeRole = await _roleManager.FindByNameAsync("Store");

            Roles = new List<IdentityRole>();

            if (operatorRole != null)
            {
                Roles.Add(operatorRole);
            }

            if (storeRole != null)
            {
                Roles.Add(storeRole);
            }
            //Users = new List<IdentityUser>();

            //foreach (var roleName in rolesToSearch)
            //{
            //	var role = await _userManager.FindByNameAsync(roleName);

            //	if (role != null)
            //	{
            //		var usersInRole = await _userManager.GetUsersInRoleAsync(role.Email);
            //		Users.AddRange(usersInRole);
            //	}
            //}
            return Page();
		}
        public async Task<IActionResult> OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
            //    return Redirect("/Admin/Users/Index");
            //}
            try
            {
                var roleName = Request.Form["RoleName"];
                var userExists = await _userManager.FindByEmailAsync(UserVM.Email);
                if (userExists != null)
                {
                    _toastNotification.AddErrorToastMessage("Email is already exist");
                    return Page();
                }
                var user = new ApplicationUser { UserName = UserVM.Email, Email = UserVM.Email, FullName = UserVM.FullName, PhoneNumber = UserVM.PhoneNumber };
                var result = await _userManager.CreateAsync(user, UserVM.Password);

                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, roleName);
                    var customer = new CustomerN()
                    {
                        Email = UserVM.Email,
                        RegisterDate = DateTime.Now,
                        CustomerName = UserVM.FullName,
                        Phone = UserVM.PhoneNumber,

                    };
                    _context.CustomerNs.Add(customer);
                    _context.SaveChanges();
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Something went Error");
                    return Redirect("/Admin/Users/Index");
                }

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("User Added successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Users/Index");

        }

        public async Task<IActionResult> OnGetSingleUsersForEdit(string Id)
        {
            var user  = _userManager.Users.Where(e => e.Id == Id).FirstOrDefault();
            var userRoles = await _userManager.GetRolesAsync(user);
            userProfileVM = new UsersVM()
            {
                FullName = user.FullName,
                Mobile = user.PhoneNumber,
               Id= user.Id,
                CurrentEmail = user.Email,
                IsActive = user.IsActive,
                
            };
            var result = new { userProfileVM, userRoles };

            return new JsonResult(result);
        }


        public async Task<IActionResult> OnPostEditUser(string Id)
        {

            var user = _userManager.Users.Where(e => e.Id == userProfileVM.Id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            var customer = _context.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
            
            if (userProfileVM.CurrentEmail != null)
            {
                //var userExists = await _userManager.FindByEmailAsync(userProfileVM.CurrentEmail);
                //if (userExists != null)
                //{
                //    _toastNotification.AddErrorToastMessage("Email is already exist");
                //    return Page();
                //}
                // Update the email if it's provided
                user.Email = userProfileVM.CurrentEmail;
                user.UserName = userProfileVM.CurrentEmail;
                var setEmailResult = await _userManager.SetEmailAsync(user, user.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Redirect("/Admin/Users/Index");
                }
            }
            else
            {
                user.Email = userProfileVM.CurrentEmail;
            }
            user.IsActive = userProfileVM.IsActive;
            user.FullName = userProfileVM.FullName;
            user.PhoneNumber = userProfileVM.Mobile;
            // Update the password
            if (!string.IsNullOrEmpty(userProfileVM.CurrentPassword) && !string.IsNullOrEmpty(userProfileVM.NewPassword))
            {
                // Update the password
                var changePasswordResult = await _userManager.ChangePasswordAsync(
                    user,
                    userProfileVM.CurrentPassword,
                    userProfileVM.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Redirect("/Admin/Users/Index");
                }
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }
            if (customer != null)
            {
                customer.Email = userProfileVM.CurrentEmail;
                customer.CustomerName = userProfileVM.FullName;
                customer.Phone = userProfileVM.Mobile;
                var Updatedcustomer = _context.CustomerNs.Attach(customer);
                Updatedcustomer.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            }
            var roleName = Request.Form["selectedRoles"];
            await _userManager.AddToRoleAsync(user, roleName);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);
            await _signInManager.RefreshSignInAsync(user);
            _context.SaveChanges();
            return Redirect("/Admin/Users/Index");

        }

        public IActionResult OnGetUsersForDelete(string Id)
        {

           

            User = _db.Users.Where(e => e.Id == Id).FirstOrDefault(); 
            return new JsonResult(User);
        }

        public async Task<IActionResult> OnPostDeleteUser(string Id)
        {
            try
            {
                User = _db.Users.Where(e => e.Id == User.Id).FirstOrDefault();
                var customer= _context.CustomerNs.Where(e=>e.Email== User.Email).FirstOrDefault();
                var customerAddress = _context.customerAddresses.Where(e => e.CustomerId == customer.CustomerId).FirstOrDefault();
                if (User != null)
                {
                    _db.Users.Remove(User);
                    _db.SaveChanges();
                   
                }
                if (customerAddress != null)
                {
                    _context.customerAddresses.Remove(customerAddress);

                    await _context.SaveChangesAsync();
                }
                if (customer != null)
                {
                    _context.CustomerNs.Remove(customer);

                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("User Deleted successfully");
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

            return Redirect("/Admin/Users/Index");
        }
    }
}
