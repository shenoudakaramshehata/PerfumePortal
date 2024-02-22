
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageShippingMatrix
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        [BindProperty]
        public ShippingMatrix ShippingMatrix { get; set; }
        public List<Country> countriesList = new List<Country>();
        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            ShippingMatrix = new CRM.Models.ShippingMatrix();
        }
        public void OnGet()
        {
            countriesList = _context.Country.ToList();
            url = $"{this.Request.Scheme}://{this.Request.Host}";

        }


        public IActionResult OnGetSingleShippingMatrixForView(int ShippingMatrixId)
        {
            var ShippingsMatrix = _context.ShippingsMatrix.Include(a => a.Country).Where(c => c.ShippingMatrixId == ShippingMatrixId)
            .Select(i => new
             {
                 ShippingMatrixId = i.ShippingMatrixId,
                 ActualPrice = i.ActualPrice,
                 AramexPrice = i.AramexPrice,
                 FromWeight = i.FromWeight,
                 ToWeight = i.ToWeight,
                 Country = i.Country.CountryTlar,

             }).FirstOrDefault();


            return new JsonResult(ShippingsMatrix);

        }


        public async Task<IActionResult> OnPostEditShippingMatrix(int ShippingMatrixId)
        {
			if (!ModelState.IsValid)
			{
				_toastNotification.AddErrorToastMessage("Please Enter All Required Data");
				return Redirect("/Admin/Configurations/ManageShippingMatrix/Index");
			}
			try
            {
                var model = _context.ShippingsMatrix.Where(c => c.ShippingMatrixId == ShippingMatrixId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("ShippingMatrix Not Found");

                    return Redirect("/Admin/Configurations/ManageShippingMatrix/Index");
                }


               
                model.ActualPrice = ShippingMatrix.ActualPrice;
                model.AramexPrice = ShippingMatrix.AramexPrice;
                model.FromWeight = ShippingMatrix.FromWeight;
                model.ToWeight = ShippingMatrix.ToWeight;
                model.CountryId = ShippingMatrix.CountryId;

                var UpdatedShippingMatrix = _context.ShippingsMatrix.Attach(model);

                UpdatedShippingMatrix.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Shipping Matrix Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
               
            }
            return Redirect("/Admin/Configurations/ManageShippingMatrix/Index");
        }


        public IActionResult OnGetSingleShippingMatrixForEdit(int ShippingMatrixId)
        {
            var Result = _context.ShippingsMatrix.Where(c => c.ShippingMatrixId == ShippingMatrixId).FirstOrDefault();
            return new JsonResult(Result);
        }


        public IActionResult OnGetSingleShippingMatrixForDelete(int ShippingMatrixId)
        {
            ShippingMatrix = _context.ShippingsMatrix.Where(c => c.ShippingMatrixId == ShippingMatrixId).FirstOrDefault();
            return new JsonResult(ShippingMatrix);
        }


        public async Task<IActionResult> OnPostDeleteShippingMatrix(int ShippingMatrixId)
        {
            try
            {
                ShippingMatrix ShippingMatrixObj = _context.ShippingsMatrix.Where(e => e.ShippingMatrixId == ShippingMatrixId).FirstOrDefault();


                if (ShippingMatrixObj != null)
                {


                    _context.ShippingsMatrix.Remove(ShippingMatrixObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Shipping Matrix Deleted successfully");
                   
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

            return Redirect("/Admin/Configurations/ManageShippingMatrix/Index");
        }


        public async Task<IActionResult> OnPostAddShippingMatrix()
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageShippingMatrix/Index");
            }
            try
            {
                

                _context.ShippingsMatrix.Add(ShippingMatrix);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Shipping Matrix Added Successfully");

                return Redirect("/Admin/Configurations/ManageShippingMatrix/Index");
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/Admin/Configurations/ManageShippingMatrix/Index");
        }



    }
}
