
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Configurations.ManageCountry
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        [BindProperty]
        public Country country { get; set; }


        public List<Country> countriesList = new List<Country>();
         
        //public List<Currency> currencyList { set; get; }

        //public CRM.Models.Country countryObj { get; set; }

        //[BindProperty]
        //public int option { get; set; }
        //public SelectList selectCurrencyList { set; get; }
        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            country = new Country();
            //countryObj = new CRM.Models.Country();
        }
        public void OnGet()
        {
            countriesList = _context.Country.ToList();
            //currencyList = _context.Currency.ToList();
            ViewData["selectCurrencyList"] = new SelectList(_context.Currency.ToList(), nameof(Currency.CurrencyId), nameof(Currency.CurrencyTlen));
          
            url = $"{this.Request.Scheme}://{this.Request.Host}";
        }


        public IActionResult OnGetSingleCountryForEdit(int CountryId)
        {
            var Result = _context.Country.Where(c => c.CountryId == CountryId).FirstOrDefault();
            return new JsonResult(Result);

        }


        public async Task<IActionResult> OnPostEditCountry(int CountryId, IFormFile EditCountryfile)
        {
           var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                    return Redirect("/Admin/Configurations/ManageCountry/Index");
                
            }
            try
            {
                var model = _context.Country.Where(c => c.CountryId == CountryId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Country Not Found");

                    return Redirect("/Admin/Configurations/ManageCountry/Index");
                }
                if (EditCountryfile != null)
                {


                    string folder = "Images/Country/";

                    model.Pic = UploadImage(folder, EditCountryfile);
                }
                else
                {
                    model.Pic = country.Pic;
                }
                model.CountryTlen = country.CountryTlen;
                model.CountryTlar = country.CountryTlar;
                model.ShippingCost = country.ShippingCost;
                model.tax=country.tax/100;

               

                var UpdatedCountry = _context.Country.Attach(model);
                UpdatedCountry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Country Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
               
            }
            return Redirect("/Admin/Configurations/ManageCountry/Index");
        }


        public IActionResult OnGetSingleCountryForView(int CountryId)
        {
            var Result = _context.Country.Where(c => c.CountryId == CountryId).Include(c => c.Currency).Select(i => new
            {
                CountryId = i.CountryId,
                //CountryTlen = i.CountryTlen,
                CountryTlen = i.CountryTlen,
                Pic = i.Pic,
                IsActive = i.IsActive,
                OrderIndex = i.OrderIndex,
                ShippingCost = i.ShippingCost,
                Currency = i.Currency.CurrencyTlen

            }).FirstOrDefault();

            return new JsonResult(Result);
        }


        //public IActionResult OnGetSingleCountryForDelete(int CountryId)
        //{
        //    country = _context.Country.Where(c => c.CountryId == CountryId).FirstOrDefault();
        //    return new JsonResult(country);
        //}


        //public async Task<IActionResult> OnPostDeleteCountry(int CountryId)
        //{
        //    try
        //    {
        //        Country CountryObj = _context.Country.Where(e => e.CountryId == CountryId).FirstOrDefault();


        //        if (CountryObj != null)
        //        {


        //            _context.Country.Remove(CountryObj);
        //            await _context.SaveChangesAsync();
        //            _toastNotification.AddSuccessToastMessage("Country Deleted successfully");
        //            var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "/" + country.Pic);
        //            if (System.IO.File.Exists(ImagePath))
        //            {
        //                System.IO.File.Delete(ImagePath);
        //            }
        //        }
        //        else
        //        {
        //            _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
        //        }
        //    }
        //    catch (Exception)

        //    {
        //        _toastNotification.AddErrorToastMessage("Something went wrong");
        //    }

        //    return Redirect("/Admin/Configurations/ManageCountry/Index");
        //}


        //public async Task<IActionResult> OnPostAddCountry(IFormFile file)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
        //        return Redirect("/Admin/Configurations/ManageCountry/Index");
        //    }
        //    try
        //    {
        //        if (file != null)
        //        {
        //            string folder = "Images/Country/";
        //            country.Pic = await UploadImage(folder, file);
        //        }

        //        _context.Country.Add(country);
        //        _context.SaveChanges();
        //        _toastNotification.AddSuccessToastMessage("Country Added Successfully");

        //    }
        //    catch (Exception)
        //    {

        //        _toastNotification.AddErrorToastMessage("Something went wrong");
        //    }
        //    return Redirect("/Admin/Configurations/ManageCountry/Index");
        //}


        //private async Task<string> UploadImage(string folderPath, IFormFile file)
        //{

        //    folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

        //    string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

        //    await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

        //    return folderPath;
        //}


        private string UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }

    }
}
