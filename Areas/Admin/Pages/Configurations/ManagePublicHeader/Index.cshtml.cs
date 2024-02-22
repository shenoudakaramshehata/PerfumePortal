using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManagePublicHeader
{
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }


        [BindProperty]
        public PublicHeader publicHeader { get; set; }


       

        


        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            publicHeader = new CRM.Models.PublicHeader();
            
        }
        public void OnGet()
        {
            publicHeader = _context.PublicHeader.FirstOrDefault();

            url = $"{this.Request.Scheme}://{this.Request.Host}";

        }

        public IActionResult OnGetSinglePublicHeaderForEdit(int PublicHeaderId)
        {
            publicHeader = _context.PublicHeader.Where(c => c.PublicHeaderId == PublicHeaderId).FirstOrDefault();
            return new JsonResult(publicHeader);

        }


        public async Task<IActionResult> OnPostEditBanner(int PublicHeaderId, IFormFile Editfile)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/ManagePublicHeader/Index");

            }
            try
            {
                var model = _context.PublicHeader.Where(c => c.PublicHeaderId == PublicHeaderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Banner Not Found");

                    return Redirect("/Admin/Configurations/ManagePublicHeader/Index");
                }
                if (Editfile != null)
                {


                    string folder = "Images/PublicHeader/";

                    model.pic = UploadImage(folder, Editfile);
                }
                else
                {
                    model.pic = publicHeader.pic;
                }
               


                var UpdatedCountry = _context.PublicHeader.Attach(model);
                UpdatedCountry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Banner Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManagePublicHeader/Index");
        }
        private string UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
        //public async Task<IActionResult> OnPostEditCoupon(int CouponId,, IFormFile EditCountryfile)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
        //        return Redirect("/Admin/Configurations/ManageCoupon/Index");
        //    }
        //    try
        //    {
        //        var model = await _context.Coupon.Where(c => c.CouponId == CouponId).FirstOrDefaultAsync();
        //        if (model == null)
        //        {
        //            _toastNotification.AddErrorToastMessage("Coupon Not Found");

        //            return Redirect("/Admin/Configurations/ManageCoupon/Index");
        //        }

        //        model.IssueDate = coupon.IssueDate;
        //        model.ExpirationDate = coupon.ExpirationDate;
        //        model.Serial = coupon.Serial;
        //        model.Amount = coupon.Amount;
        //        model.CouponTypeId = coupon.CouponTypeId;
        //        model.Used = coupon.Used;
        //        var Updatedcoupon = _context.Coupon.Attach(model);

        //        Updatedcoupon.State = EntityState.Modified;

        //        _context.SaveChanges();

        //        _toastNotification.AddSuccessToastMessage("Coupon Edited successfully");


        //    }
        //    catch (Exception)
        //    {
        //        _toastNotification.AddErrorToastMessage("Something went Error");

        //    }
        //    return Redirect("/Admin/Configurations/ManageCoupon/Index");
        //}



    }
}
