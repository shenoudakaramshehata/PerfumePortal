
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.UserMessages
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }


        [BindProperty]
        public ContactUs contactUs { get; set; }


       


        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            contactUs = new CRM.Models.ContactUs();
  
        }
        public void OnGet()
        {
           
            url = $"{this.Request.Scheme}://{this.Request.Host}";

        }

       
        

        public IActionResult OnGetSingleMessageForView(int ContactId)
        {
            var Result = _context.ContactUs.Where(c => c.ContactId == ContactId).FirstOrDefault();
            
            return new JsonResult(Result);
        }


        public IActionResult OnGetSingleMessageForDelete(int ContactId)
        {
            contactUs = _context.ContactUs.Where(c => c.ContactId == ContactId).FirstOrDefault();
            return new JsonResult(contactUs);
        }

        public async Task<IActionResult> OnPostDeleteMessage(int ContactId)
        {
            try
            {
                ContactUs MessageObj = _context.ContactUs.Where(e => e.ContactId == ContactId).FirstOrDefault();


                if (MessageObj != null)
                {


                    _context.ContactUs.Remove(MessageObj);

                    await _context.SaveChangesAsync();

                    _toastNotification.AddSuccessToastMessage("Message Deleted successfully");

                  
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

            return Redirect("/Admin/UserMessages/Index");
        }

       
        

    }
}
