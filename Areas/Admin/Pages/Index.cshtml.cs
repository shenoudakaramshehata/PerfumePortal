using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.Data;
using CRM.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        public int CustomerCount { get; set; }
        public int PaidOrderCount { get; set; }
        public int PaidOrderAmount { get; set; }
        public int TotalOrderAmount { get; set; }
        public int CanceledOrderCount { get; set; }
        public int TotalOrderCount { get; set; }
        public int OrderActiveCount { get; set; }
        public List<OrderVM> OrderVmList { get; set; }
        
        public IndexModel(PerfumeContext context)
        {
            _context = context;
            OrderVmList = new List<OrderVM>();
        }
        public void OnGet()
        {
            CustomerCount = _context.CustomerNs.Count();
            PaidOrderCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();
            CanceledOrderCount = _context.Order.Where(e => e.OrderStatusId == 5).Count();
            PaidOrderCount = _context.Order.Where(e => e.IsPaid == true).Count();
            OrderActiveCount = _context.Order.Where(e => e.OrderStatusId == 2 && e.IsDeliverd == false).Count();
            TotalOrderCount = _context.Order.Count();
            PaidOrderAmount =(int) _context.Order.Where(e => e.OrderStatusId == 2).Sum(e => e.OrderNet).Value;
            TotalOrderAmount =(int) _context.Order.Sum(e => e.OrderNet);
            OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(o => o.OrderItem.Any()).Include(a => a.OrderItem).Select(i => new OrderVM
            {
                OrderDate = i.OrderDate,
                Country = i.Country.CountryTlen,
                CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                NetOrder = i.OrderNet.HasValue ? i.OrderNet.Value : 0, 
                Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                Status = i.OrderStatus.Status


            }).OrderByDescending(d => d.OrderDate).ToList();

        }
        public PartialViewResult OnGetSortList(string company)
        {
            int restult = 0;
            bool checkValue = int.TryParse(company, out restult);
            if (checkValue)
            {
                if (restult == 0)
                {
                    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Select(i => new OrderVM
                    {
                        OrderDate = i.OrderDate,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                        Status = i.OrderStatus.Status
                    }).OrderByDescending(d=>d.OrderDate).ToList();

                }
                if (restult == 1)
                {
                    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e=>e.OrderDate.Date==DateTime.Now.Date).Select(i => new OrderVM
                    {
                        OrderDate = i.OrderDate,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                        Status = i.OrderStatus.Status


                    }).OrderByDescending(d => d.OrderDate).ToList();

                }
                if (restult == 2)
                {
                    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderDate.Date >= DateTime.Now.AddDays(-30)).Select(i => new OrderVM
                    {
                        OrderDate = i.OrderDate,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                        Status = i.OrderStatus.Status


                    }).OrderByDescending(d => d.OrderDate).ToList();

                }
                if (restult == 3)
                {
                    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderStatusId==2).Select(i => new OrderVM
                    {
                        OrderDate = i.OrderDate,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                        Status = i.OrderStatus.Status


                    }).OrderByDescending(d => d.OrderDate).ToList();

                }
                //if (restult == 4)
                //{
                //    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderStatusId == 4).Select(i => new OrderVM
                //    {
                //        OrderDate = i.OrderDate.ToShortDateString().ToString(),
                //        Country = i.Country.CountryTlen,
                //        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                //        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                //        NetOrder = i.OrderNet.Value,
                //        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                //        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                //        Status = i.OrderStatus.Status


                //    }).ToList();

                //}
                if (restult == 5)
                {
                    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderStatusId == 5).Select(i => new OrderVM
                    {
                        OrderDate = i.OrderDate,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                        Status = i.OrderStatus.Status


                    }).OrderByDescending(d => d.OrderDate).ToList();

                }
                if (restult == 6)
                {
                    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.OrderStatusId == 5&&e.IsPaid==true).Select(i => new OrderVM
                    {
                        OrderDate = i.OrderDate,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                        Status = i.OrderStatus.Status


                    }).OrderByDescending(d => d.OrderDate).ToList();

                }
                if (restult == 7)
                {
                    OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).Where(e => e.IsPaid == true&&e.OrderDate.Date==DateTime.Now.Date).Select(i => new OrderVM
                    {
                        OrderDate = i.OrderDate,
                        Country = i.Country.CountryTlen,
                        CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                        CusAddress = i.CustomerAddress.Country.CountryTlen + " , " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                        NetOrder = i.OrderNet.Value,
                        Deliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                        Cancelled = i.IsCanceled == true ? "Cancelled" : "Not Cancelled",
                        Status = i.OrderStatus.Status


                    }).OrderByDescending(d => d.OrderDate).ToList();

                }
            }

            return Partial("_FilteredTable", OrderVmList);
        }
    }
}
