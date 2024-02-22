using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Threading.Tasks;

namespace CRM.Pages
{
    public class ContactUsModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public ContactUs contact { get; set; }
        public ContactUsModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPostSendMessage([FromBody] ContactUs Messageobj)
        {
            try
            {
                if (Messageobj != null)
                {

                    ContactUs contact = new ContactUs
                    {
                        
                         FirstName= Messageobj.FirstName,
                        LastName = Messageobj.LastName,
                        Email = Messageobj.Email,
                        Mobile = Messageobj.Mobile,
                        Msg = Messageobj.Msg,
                        TransDate=DateTime.Now
                    };

                    _context.ContactUs.Add(contact);
                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Your message was delivered successfully");
                }

                   
               
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");

            }
            return new JsonResult(Messageobj);
        }
    }
}
