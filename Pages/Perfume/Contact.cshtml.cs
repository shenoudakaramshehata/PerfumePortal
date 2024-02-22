using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Threading.Tasks;

namespace CRM.Pages.Perfume
{
    public class ContactModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public ContactUs contact { get; set; }
        public ContactModel(PerfumeContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            try
            {
                contact.TransDate = System.DateTime.Now;
                _context.ContactUs.Add(contact);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Your Message is Sending Successfully");
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");
                 
            }
            return Page();
        }
    }
}
