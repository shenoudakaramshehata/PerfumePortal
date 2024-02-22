using CRM.Services.TabbyModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Pages
{
    public class TabbyTestModel : PageModel
    {
        public void OnGet()
        {

            TabbyCheckRequestMessage TabbyCheckoutRequest = new TabbyCheckRequestMessage()
            {
                lang = "en",
                merchant_code = "",
                create_token = true,
                token = "",
                merchant_urls = new MerchantUrls()
                {
                    cancel = "",
                    failure = "",
                    success = "",
                },
                payment = new Payment()
                {

                },
            };
        }
    }
}
