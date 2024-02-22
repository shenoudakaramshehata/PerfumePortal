
using CRM.Data;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageCoupon
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }


        [BindProperty]
        public Coupon coupon { get; set; }


        public List<Coupon> couponList = new List<Coupon>();
        
        public Coupon couponObj { get; set; }



        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            coupon = new CRM.Models.Coupon();
            couponObj = new CRM.Models.Coupon();
        }
        public void OnGet()
        {
            couponList = _context.Coupon.Include(c=>c.CouponType).ToList();

            url = $"{this.Request.Scheme}://{this.Request.Host}";

            ViewData["selectCouponTypeList"] = new SelectList(_context.CouponTypes.ToList(), nameof(CouponType.CouponTypeId), nameof(CouponType.CouponTypeEN));


        }

        public IActionResult OnGetSingleCouponForEdit(int CouponId)
        {
            coupon = _context.Coupon.Where(c => c.CouponId == CouponId).FirstOrDefault();
            return new JsonResult(coupon);

        }

        public async Task<IActionResult> OnPostEditCoupon(int CouponId)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageCoupon/Index");
            }
            try
            {
                var model = await _context.Coupon.Where(c => c.CouponId == CouponId).FirstOrDefaultAsync();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Coupon Not Found");

                    return Redirect("/Admin/Configurations/ManageCoupon/Index");
                }

                model.IssueDate = coupon.IssueDate;
                model.ExpirationDate = coupon.ExpirationDate;
                model.Serial = coupon.Serial;
                model.Amount = coupon.Amount;
                model.CouponTypeId = coupon.CouponTypeId;
                model.Used = coupon.Used;
                var Updatedcoupon = _context.Coupon.Attach(model);

                Updatedcoupon.State = EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Coupon Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
               
            }
            return Redirect("/Admin/Configurations/ManageCoupon/Index");
        }


        public IActionResult OnGetSingleCouponForView(int couponId)
        {
            var Result = _context.Coupon.Where(c => c.CouponId == couponId).Include(c => c.CouponType).Select(i => new
            {
                CouponId = i.CouponId,
                Serial = i.Serial,
                ExpirationDate = i.ExpirationDate,
                IssueDate = i.IssueDate,
                Amount = i.Amount,
                //CouponTypeId = i.CouponTypeId,
                Used = i.Used,
                CouponTypeEN = i.CouponType.CouponTypeEN

            }).FirstOrDefault();
            return new JsonResult(Result);
        }


        public IActionResult OnGetSingleCouponForDelete(int couponId)
        {
            coupon = _context.Coupon.Where(c => c.CouponId == couponId).FirstOrDefault();
            return new JsonResult(coupon);
        }

        public async Task<IActionResult> OnPostDeleteCoupon(int couponId)
        {
            try
            {
                CRM.Models.Coupon CatObj = _context.Coupon.Where(e => e.CouponId == couponId).FirstOrDefault();

                var Couponorder = _context.Order.Where(e => e.CouponId == CatObj.CouponId).ToList();
                foreach (var item in Couponorder)
                {
                    var CouponorderItems = _context.OrderItem.Where(e => e.OrderId == item.OrderId).ToList();
                    if(CouponorderItems.Count > 0)
                    {
                        _context.OrderItem.RemoveRange(CouponorderItems);
                        _context.SaveChanges();
                    }
                }

                if (Couponorder.Count > 0)
                {
                    _context.Order.RemoveRange(Couponorder);
                    _context.SaveChanges();
                }
                
                if (CatObj != null)
                {


                    _context.Coupon.Remove(CatObj);

                    await _context.SaveChangesAsync();

                    _toastNotification.AddSuccessToastMessage("Coupon Deleted successfully");

                  
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

            return Redirect("/Admin/Configurations/ManageCoupon/Index");
        }

        public IActionResult OnPostAddCoupon()
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageCoupon/Index");
            }
            try
            {
               

                _context.Coupon.Add(coupon);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Coupon Added Successfully");

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/Admin/Configurations/ManageCoupon/Index");
        }

        

    }
}
