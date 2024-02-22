using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.Data;
using CRM.ViewModels;
using CRM.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Order
{
    public class OrderReportModel : PageModel
    {
        private PerfumeContext _context;
        public List<OrderFiltterModel> OrderVmList { get; set; }
        public List<Country> countries { get; set; }
        public List<OrderStatus> statuses { get; set; }
        [BindProperty]
        public OrderFilterVM orderFilter { get; set; }

        public OrderReportModel(PerfumeContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Select(i => new OrderFiltterModel
            {
                OrderDate = i.OrderDate,
                Country = i.Country.CountryTlen,
                CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                NetOrder = i.OrderNet.Value,
               
                Status = i.OrderStatus.Status,
                statusId = i.OrderStatus.OrderStatusId,
                CountryId = i.CountryId.Value,


            }).ToList();
            countries = _context.Country.ToList();
            statuses = _context.OrderStatuses.ToList();
        }
        public ActionResult OnPost()
        {

            try
            {
                countries = _context.Country.ToList();
                statuses = _context.OrderStatuses.ToList();
                OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Select(i => new OrderFiltterModel
                {
                    OrderDate = i.OrderDate,
                    Country = i.Country.CountryTlen,
                    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                    CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                    NetOrder = i.OrderNet.Value,
                    
                    Status = i.OrderStatus.Status,
                    statusId = i.OrderStatus.OrderStatusId,
                    CountryId = i.CountryId.Value,


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
                            OrderVmList = OrderVmList.Where(a => a.OrderDate >= startFromToDate && a.OrderDate.Date <= toEndToDate).ToList();
                        }
                    }
                    else
                    {
                        startFromToDate = DateTime.Parse(orderFilter.date).Date;
                        OrderVmList = OrderVmList.Where(a => a.OrderDate == startFromToDate).ToList();

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
