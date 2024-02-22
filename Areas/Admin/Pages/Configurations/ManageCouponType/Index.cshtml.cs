
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageCouponType
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }


        [BindProperty]
        public CouponType couponType { get; set; }


        public List<CouponType> couponTypeList = new List<CouponType>();
        
        public CouponType couponTypeObj { get; set; }

        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            couponType = new CRM.Models.CouponType();
            couponTypeObj = new CRM.Models.CouponType();
        }
        public void OnGet()
        {
            couponTypeList = _context.CouponTypes.ToList();
            url = $"{this.Request.Scheme}://{this.Request.Host}";

        }

        public IActionResult OnGetSingleCouponTypeForEdit(int CouponTypeId)
        {
            couponType = _context.CouponTypes.Where(c => c.CouponTypeId == CouponTypeId).FirstOrDefault();
            return new JsonResult(couponType);

        }

        public async Task<IActionResult> OnPostEditCouponType(int CouponTypeId)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageCouponType/Index");
            }
            try
            {
                var model = await _context.CouponTypes.Where(c => c.CouponTypeId == CouponTypeId).FirstOrDefaultAsync();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Coupon type Not Found");

                    return Redirect("/Admin/Configurations/ManageCouponType/Index");
                }

                model.CouponTypeId = couponType.CouponTypeId;
                model.CouponTypeAR = couponType.CouponTypeAR;
                model.CouponTypeEN = couponType.CouponTypeEN;
                var Updatedcoupon = _context.CouponTypes.Attach(model);

                Updatedcoupon.State = EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Coupon type edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageCouponType/Index");
        }


        public IActionResult OnGetSingleCouponTypeForView(int CouponTypeId)
        {
                var Result = _context.CouponTypes.Where(c => c.CouponTypeId == CouponTypeId).FirstOrDefault();

            return new JsonResult(Result);
        }


        public IActionResult OnGetSingleCouponTypeForDelete(int CouponTypeId)
        {
            couponType = _context.CouponTypes.Where(c => c.CouponTypeId == CouponTypeId).FirstOrDefault();
            return new JsonResult(couponType);
        }

        public async Task<IActionResult> OnPostDeleteCouponType(int CouponTypeId)
        {
            try
            {
                CRM.Models.CouponType CoTypeObj = _context.CouponTypes.Where(e => e.CouponTypeId == CouponTypeId).FirstOrDefault();


                if (CoTypeObj != null)
                {


                    _context.CouponTypes.Remove(CoTypeObj);

                    await _context.SaveChangesAsync();

                    _toastNotification.AddSuccessToastMessage("Coupon type deleted successfully");


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

            return Redirect("/Admin/Configurations/ManageCouponType/Index");
        }

        public IActionResult OnPostAddCouponType()
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageCouponType/Index");
            }
            try
            {


                _context.CouponTypes.Add(couponType);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Coupon types Added Successfully");

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/Admin/Configurations/ManageCouponType/Index");
        }



    }
}
