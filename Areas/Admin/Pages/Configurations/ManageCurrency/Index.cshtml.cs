
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageCurrency
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        [BindProperty]
        public Currency currency { get; set; }
        public List<Currency> countriesList = new List<Currency>();
        //public CRM.Models.Currency currencyObj { get; set; }
        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            currency = new CRM.Models.Currency();
            //currencyObj = new CRM.Models.Currency();
        }
        public void OnGet()
        {
            countriesList = _context.Currency.ToList();
            url = $"{this.Request.Scheme}://{this.Request.Host}";

        }


        public IActionResult OnGetSingleCurrencyForEdit(int CurrencyId)
        {
            currency = _context.Currency.Where(c => c.CurrencyId == CurrencyId).FirstOrDefault();
            return new JsonResult(currency);

        }


        public async Task<IActionResult> OnPostEditCurrency(int CurrencyId, IFormFile Editfile)
        {
			if (!ModelState.IsValid)
			{
				_toastNotification.AddErrorToastMessage("Please Enter All Required Data");
				return Redirect("/Admin/Configurations/ManageCurrency/Index");
			}
			try
            {
                var model = _context.Currency.Where(c => c.CurrencyId == CurrencyId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Currency Not Found");

                    return Redirect("/Admin/Configurations/ManageCurrency/Index");
                }


                if (Editfile != null)
                {
                   

                    string folder = "Images/Currency/";
                    model.CurrencyPic = UploadImage(folder, Editfile);
                }
                else
                {
                    model.CurrencyPic = currency.CurrencyPic;
                }
                model.IsActive = currency.IsActive;
                model.CurrencyTlar = currency.CurrencyTlar;
                model.CurrencyTlen = currency.CurrencyTlen;

                var UpdatedCurrency = _context.Currency.Attach(model);

                UpdatedCurrency.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Currency Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
               
            }
            return Redirect("/Admin/Configurations/ManageCurrency/Index");
        }


        public IActionResult OnGetSingleCurrencyForView(int CurrencyId)
        {
            var Result = _context.Currency.Where(c => c.CurrencyId == CurrencyId).FirstOrDefault();
            return new JsonResult(Result);
        }


        public IActionResult OnGetSingleCurrencyForDelete(int CurrencyId)
        {
            currency = _context.Currency.Where(c => c.CurrencyId == CurrencyId).FirstOrDefault();
            return new JsonResult(currency);
        }


        public async Task<IActionResult> OnPostDeleteCurrency(int CurrencyId)
        {
            try
            {
                Currency CurrencyObj = _context.Currency.Where(e => e.CurrencyId == CurrencyId).FirstOrDefault();


                if (CurrencyObj != null)
                {


                    _context.Currency.Remove(CurrencyObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Currency Deleted successfully");
                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "/" + currency.CurrencyPic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
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

            return Redirect("/Admin/Configurations/ManageCurrency/Index");
        }


        public async Task<IActionResult> OnPostAddCurrency(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageCurrency/Index");
            }
            try
            {
                if (file != null)
                {
                    string folder = "Images/Currency/";
                    currency.CurrencyPic =  UploadImage(folder, file);
                }

                _context.Currency.Add(currency);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Currency Added Successfully");

                return Redirect("/Admin/Configurations/ManageCurrency/Index");
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/Admin/Configurations/ManageCurrency/Index");
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
