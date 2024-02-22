using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.Data;
using CRM.ViewModels;
using CRM.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Configurations.ManageOrder
{
    public class OrdersModel : PageModel
    {
        private PerfumeContext _context;
        public List<OrderFiltterModel> OrderVmList { get; set; }
        public List<Country> countries { get; set; }
        public List<OrderStatus> statuses { get; set; }
        public int InitatedCount { get; set; }
        public int Notpaid { get; set; }
        public int Processing { get; set; }
        public int Packing { get; set; }
        public int Ondelivery { get; set; }


        public int PaidCount { get; set; }

        [BindProperty]
        public OrderFilterVM orderFilter { get; set; }

        public OrdersModel(PerfumeContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(o => o.OrderItem.Any()).OrderByDescending(i => i.OrderDate).Select(i => new OrderFiltterModel
            {
                OrderDate = i.OrderDate,
                Country = i.Country.CountryTlen,
                CusName= _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
                //CusName = _context.CustomerNs.Where(e => e.CustomerId == i..CustomerId).FirstOrDefault().CustomerName,
                CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                NetOrder = Math.Round(i.OrderNet.Value, 2),
                Status = i.OrderStatus.Status,
                statusId = i.OrderStatus.OrderStatusId,
                CountryId = i.CountryId.Value,
                SerialNo= i.OrderSerial,
                OrderId=i.OrderId,

            }).ToList();
            countries = _context.Country.ToList();
            statuses = _context.OrderStatuses.Where(e=>e.OrderStatusId!= 4).ToList();
            InitatedCount = _context.Order.Where(e => e.OrderStatusId == 1).Count();
            PaidCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();
            Notpaid = _context.Order.Where(e => e.OrderStatusId == 3).Count();
            Processing = _context.Order.Where(e => e.OrderStatusId == 6).Count();
            Packing = _context.Order.Where(e => e.OrderStatusId == 7).Count();
            Ondelivery = _context.Order.Where(e => e.OrderStatusId == 5).Count();
            
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
                countries = _context.Country.ToList();
                statuses = _context.OrderStatuses.ToList();
                InitatedCount = _context.Order.Where(e => e.OrderStatusId == 1).Count();
                PaidCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();
                Notpaid = _context.Order.Where(e => e.OrderStatusId == 3).Count();
                Processing = _context.Order.Where(e => e.OrderStatusId == 6).Count();
                Packing = _context.Order.Where(e => e.OrderStatusId == 7).Count();
                Ondelivery = _context.Order.Where(e => e.OrderStatusId == 5).Count();
                OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e=>e.OrderStatusId==2).Select(i => new OrderFiltterModel
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
