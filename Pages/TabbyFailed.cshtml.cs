using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using CRM.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using CRM.Services;
using CRM.ViewModels;
using NToastNotify;
using RestSharp;
using Newtonsoft.Json.Linq;
using CRM.Services.TabbyModels;
using System.Text;
using System.Drawing;


namespace CRM.Pages
{
    public class TabbyFailedModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        public Order order { get; set; }
        public InvoiceVm invoiceVm { get; set; }
        public string PaymentStatus { get; set; }

        public ApplicationUser user { set; get; }
        private readonly IRazorPartialToStringRenderer _renderer;

        private readonly IConfiguration _configuration;
        public HttpClient httpClient { get; set; }
        public string Reason { get; set; }
        public Root ResStatus { get; set; }
        FattorhResult FattoraResStatus { set; get; }
        //public static bool expired = false;
        string res { set; get; }
        public TabbyFailedModel(IRazorPartialToStringRenderer renderer, PerfumeContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _env = env;
            _renderer = renderer;
            httpClient = new HttpClient();


        }
        public FattorahPaymentResult fattorahPaymentResult { get; set; }
        static string token = "sk_74eb86f5-f780-4b08-b73d-1de94a8270ff";
        //static string testURL = "https://apitest.myfatoorah.com/v2/GetPaymentStatus";
        //static string liveURL = "https://api.myfatoorah.com/v2/GetPaymentStatus";
        public async Task<IActionResult> OnGet(string payment_id)
        {
            if (payment_id == null)
            {
                //return RedirectToPage("SomethingwentError");
            }
            string url = "https://api.tabby.ai/api/v1/payments/" + payment_id;


            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //request.Headers.Add("Authorization", "Bearer sk_test_bf04ca3f-b6e5-451b-8013-8aefeaeffd60");
            //request.Headers.Add("Cookie", "_cfuvid=R6P0.2ESKgiMKGYmO3NQ4qEZQuDr.W5EPlICzx3a2iQ-1691234705097-0-604800000");
            var response = await client.SendAsync(request);
            var res = await response.Content.ReadAsStringAsync();
            if (res != null)
            {
                ResStatus = JsonConvert.DeserializeObject<Root>(res);
                if (ResStatus.status == "REJECTED")
                {

                    int orderId = 0;
                    bool checkRes = int.TryParse(ResStatus.meta.order_id, out orderId);

                    try
                    {
                        if (ResStatus.meta.order_id != null)
                        {

                            if (checkRes)
                            {

                                order = _context.Order.Where(e => e.OrderId == orderId).FirstOrDefault();
                                order.IsPaid = false;

                                order.OrderStatusId = 3;
                                var TrakingOrderObj = new OrderTraking()
                                {
                                    OrderId = order.OrderId,
                                    OrderStatusId = 3,
                                    ActionDate = DateTime.Now,
                                    Remarks = "There Is Something Error During Transaction"
                                };
                                _context.OrderTrakings.Add(TrakingOrderObj);
                                var UpdatedOrder = _context.Order.Attach(order);
                                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _context.SaveChanges();
                                //expired = true;
                                Reason = ResStatus.meta.ToString();
                                return Page();
                            }
                        }
                        return RedirectToPage("SomethingwentError", new { Message = "SomeThing Went Error,try again" });

                    }
                    catch (Exception ex)
                    {
                        return RedirectToPage("SomethingwentError", new { Message = ex.Message });
                    }





                }

            }
            return Page();

        }
    }
}