using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace CRM.Pages
{
    public class ProductsModel : PageModel
    {

        private readonly PerfumeContext _context;

        private readonly IToastNotification _toastNotification;

        private readonly UserManager<ApplicationUser> _userManager;

        public string ProductUrl { get; set; }

        public List<Item> ItemsList { get; set; }


        public ProductsModel(PerfumeContext perfumeContext, IToastNotification toastNotification
                             , UserManager<ApplicationUser> userManager)
        {
            _context = perfumeContext;
            _toastNotification = toastNotification;
            _userManager = userManager;
        }


        public void OnGet()
        {
            ItemsList = _context.Item.Where(c => c.CategoryId == 1).ToList();
        }
    }
}
