using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.Data;
using CRM.ViewModels;
using CRM.Models;
using Microsoft.EntityFrameworkCore;
using MailKit.Search;
using DevExpress.DataAccess.Native.Filtering;
using Microsoft.AspNetCore.Identity;

namespace CRM.Areas.Admin.Pages.Configurations.Operator
{
    public class OrdersModel : PageModel
    {
        private PerfumeContext _context;
        public List<OrderFiltterModel> OrderVmList { get; set; }
        public List<Country> countries { get; set; }
        public List<OrderStatus> statuses { get; set; }
        public int InitatedCount { get; set; }

        public int PaidCount { get; set; }

        [BindProperty]
        public OrderFilterVM orderFilter { get; set; }
       
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersModel(PerfumeContext context, UserManager<ApplicationUser> userManager,

            ApplicationDbContext db)
        {
            _context = context;
            _userManager = userManager;
            _db = db;
            
        }
        public void OnGet()
        {
            OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(i => i.OrderStatusId == 2|| i.OrderStatusId == 3||i.OrderStatusId == 7|| i.OrderStatusId == 2 || i.OrderStatusId == 5 || i.OrderStatusId==6).OrderByDescending(e=>e.OrderId).Select(i => new OrderFiltterModel
            {
                OrderDate = i.OrderDate,
                Country = i.Country.CountryTlen,
                CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
                CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                NetOrder = i.OrderNet.Value,
                Status = i.OrderStatus.Status,
                statusId = i.OrderStatus.OrderStatusId,
                CountryId = i.CountryId.Value,
                SerialNo= i.OrderSerial,
                OrderId=i.OrderId,
                Email= _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().Email,
                Phone= _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().Phone,
			}).ToList();
            countries = _context.Country.ToList();
            statuses = _context.OrderStatuses.Where(i => i.OrderStatusId == 2 || i.OrderStatusId == 3 || i.OrderStatusId == 7 || i.OrderStatusId == 2 || i.OrderStatusId == 5 || i.OrderStatusId == 6).ToList();
            InitatedCount = _context.Order.Where(e => e.OrderStatusId == 1).Count();
            PaidCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();

        }



        public PartialViewResult OnGetFilterList(string searchTerm)
        
        {
            bool CheckSearchItem = false;
            int SearchItem = 0;
            CheckSearchItem = int.TryParse(searchTerm, out SearchItem);
            var list = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(i => i.OrderStatusId == 2 || i.OrderStatusId == 3 || i.OrderStatusId == 7 || i.OrderStatusId == 2 || i.OrderStatusId == 5 || i.OrderStatusId == 6).ToList();
           
            OrderVmList = list.Where(x => x.OrderId == SearchItem || _context.CustomerNs.Where(e => e.CustomerId == x.CustomerId).FirstOrDefault().Email.Contains(searchTerm) || _context.CustomerNs.Where(e => e.CustomerId == x.CustomerId).FirstOrDefault().CustomerName.Contains(searchTerm)|| _context.CustomerNs.Where(e => e.CustomerId == x.CustomerId).FirstOrDefault().Phone.Contains(searchTerm) || _context.CustomerNs.Where(e => e.CustomerId == x.CustomerId).FirstOrDefault().Phone.Contains(CleanPhoneNumber(searchTerm))).Select(i => new OrderFiltterModel
            {
                OrderDate = i.OrderDate,
                Country = i.Country.CountryTlen,
                CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                NetOrder = i.OrderNet.Value,
                Status = i.OrderStatus.Status,
                statusId = i.OrderStatus.OrderStatusId,
                CountryId = i.CountryId.Value,
                SerialNo = i.OrderSerial,
                OrderId = i.OrderId,
                Email = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().Email,
                Phone = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().Phone,
            }).ToList();


            return Partial("_FilterTable", OrderVmList);
        }

        private string CleanPhoneNumber(string phoneNumber)
        {
            // Remove any non-digit characters from the phone number
            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }
        public IActionResult OnGetPrint(int OrderId)
        {
            var Result = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(Result);
        }
        public ActionResult OnPost()
        {

            try
            {
                InitatedCount = _context.Order.Where(e => e.OrderStatusId == 1).Count();
                PaidCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();
                countries = _context.Country.ToList();
                statuses = _context.OrderStatuses.Where(i => i.OrderStatusId == 2 || i.OrderStatusId == 3 || i.OrderStatusId == 7 || i.OrderStatusId == 2 || i.OrderStatusId == 5 || i.OrderStatusId == 6).ToList();
                OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(i => i.OrderStatusId == 2 || i.OrderStatusId == 3 || i.OrderStatusId == 7 || i.OrderStatusId == 2 || i.OrderStatusId == 5 || i.OrderStatusId == 6).Select(i => new OrderFiltterModel
                {
                    OrderDate = i.OrderDate,
                    Country = i.Country.CountryTlen,
                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                    CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                    NetOrder = i.OrderNet.Value,
                   SerialNo=i.OrderSerial,
                    Status = i.OrderStatus.Status,
                    statusId = i.OrderStatus.OrderStatusId,
                    CountryId = i.CountryId.Value,
                    OrderId = i.OrderId,

                }).ToList();
                DateTime startFromToDate = DateTime.Now;
                if (orderFilter.date != null)
                {


                    if (orderFilter.date.Contains("to"))
                    {

                        var date = orderFilter.date.Split("to");
                        var startFrom = date[0];
                        var toEnd = date[1];
                        startFromToDate = DateTime.Parse(startFrom).Date;
                        var toEndToDate = DateTime.Parse(toEnd).Date;
                        if (startFromToDate != null && toEndToDate != null)
                        {
                            OrderVmList = OrderVmList.Where(a => a.OrderDate >= startFromToDate &&  a.OrderDate.Date <= toEndToDate).ToList();
                        }
                    }
                    else
                    {
                        startFromToDate = DateTime.Parse(orderFilter.date).Date;
                        OrderVmList = OrderVmList.Where(a => a.OrderDate.Date == startFromToDate).ToList();

                    }
                }
                if (orderFilter.status != 0)
                {

                    OrderVmList = OrderVmList.Where(a => a.statusId == orderFilter.status).ToList();
                }
                if (orderFilter.country != 0)
                {

                    OrderVmList = OrderVmList.Where(a => a.CountryId == orderFilter.country).ToList();
                }


            }
            catch (Exception)
            {

            }
            return Page();
        }
    }
}
