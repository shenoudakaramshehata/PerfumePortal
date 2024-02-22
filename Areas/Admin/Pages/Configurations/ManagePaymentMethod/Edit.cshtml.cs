using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManagePaymentMethod
{
    public class EditModel : PageModel
    {

        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _hostEnvironment;


        [BindProperty]
        public PaymentMehod PaymentMethod { get; set; }


        public EditModel(PerfumeContext context, IToastNotification toastNotification, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _hostEnvironment = hostEnvironment;
        }


        public ActionResult OnGet(int id)
        {
            PaymentMethod = _context.paymentMehods.FirstOrDefault(a => a.PaymentMethodId == id);
            return Page();
        }


        public async Task<ActionResult> OnPost(int id, IFormFile Editfile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _toastNotification.AddErrorToastMessage("Please Enter Required Image");

                    return Page();
                }

                var paymentMethod = _context.paymentMehods.FirstOrDefault(a => a.PaymentMethodId == id);

                if (paymentMethod is null)
                {
                    _toastNotification.AddErrorToastMessage("Payment method Not Found");

                    return NotFound();
                }

                if (Editfile != null)
                {
                  

                    string folder = "images/PaymentMethod/";
                    paymentMethod.PaymentMethodPic = UploadImage(folder, Editfile);
                }
                else
                {
                    paymentMethod.PaymentMethodPic = paymentMethod.PaymentMethodPic;
                }

                var UpdatedPaymenyMethod = _context.paymentMehods.Attach(paymentMethod);

                UpdatedPaymenyMethod.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Image updated successfully");
                
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
            }

           return Redirect("./");
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

