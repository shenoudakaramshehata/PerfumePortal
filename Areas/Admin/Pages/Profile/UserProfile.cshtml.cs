using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Identity;

namespace CRM.Areas.Admin.Pages.Profile
{
    public class UserProfileModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public UserProfileVM userProfileVM { get; set; }
        public UserProfileModel(PerfumeContext context, ApplicationDbContext db, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            userProfileVM = new UserProfileVM();
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> OnGet()
        {
          
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Login");
            }
            userProfileVM = new UserProfileVM()
            {
                FullName = user.FullName,
                Mobile = user.PhoneNumber,
                Pic = user.UserPic,
                Email=user.Email,
                UserId=user.Id
            };
            return Page();
        }
        public async Task<IActionResult> OnGetSingleUserForEdit()
        {
            var user = await _userManager.GetUserAsync(User);
            return new JsonResult(user);
        }
        public async Task<IActionResult> OnPostEditUserProfile(IFormFile Editfile)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/Identity/Account/Login");
                }


                if (Editfile != null)
                {
                    string folder = "Images/Users/";
                    user.UserPic = UploadImage(folder, Editfile);
                }
                else
                {
                    user.UserPic = userProfileVM.Pic;
                }
                user.PhoneNumber = userProfileVM.Mobile;
                user.FullName = userProfileVM.FullName;
                var UpdatedUser = _db.Users.Attach(user);
                UpdatedUser.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _db.SaveChanges();
                _toastNotification.AddSuccessToastMessage("User Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Profile/UserProfile");
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
