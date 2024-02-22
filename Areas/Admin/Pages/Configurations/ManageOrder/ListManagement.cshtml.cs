using CRM.Data;
using CRM.Models;
using CRM.ViewModels;
using CRM.wwwroot.DataTables;
using DevExpress.Xpo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Linq.Dynamic.Core;

namespace CRM.Areas.Admin.Pages.Configurations.ManageOrder
{
    public class ListManagementModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }

        public List<CRM.Models.Order> OrderList { get; set; }


        public ListManagementModel(PerfumeContext context, IWebHostEnvironment hostEnvironment,
                                           IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
        }
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

       
        public void OnGet()
        {
            //OrderVmList = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).OrderByDescending(i => i.OrderDate).Where(o => o.OrderItem.Any()).Select(i => new OrderFiltterModel
            //{
            //    OrderDate = i.OrderDate,
            //    Country = i.Country.CountryTlen,
            //    CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
            //    //CusName = _context.CustomerNs.Where(e => e.CustomerId == i..CustomerId).FirstOrDefault().CustomerName,
            //    CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
            //    NetOrder = i.OrderNet.Value,
            //    Status = i.OrderStatus.Status,
            //    statusId = i.OrderStatus.OrderStatusId,
            //    CountryId = i.CountryId.Value,
            //    SerialNo = i.OrderSerial,
            //    OrderId = i.OrderId,

            //}).ToList();
            countries = _context.Country.ToList();
            statuses = _context.OrderStatuses.Where(e => e.OrderStatusId != 4).ToList();
            InitatedCount = _context.Order.Where(e => e.OrderStatusId == 1).Count();
            PaidCount = _context.Order.Where(e => e.OrderStatusId == 2).Count();
            Notpaid = _context.Order.Where(e => e.OrderStatusId == 3).Count();
            Processing = _context.Order.Where(e => e.OrderStatusId == 6).Count();
            Packing = _context.Order.Where(e => e.OrderStatusId == 7).Count();
            Ondelivery = _context.Order.Where(e => e.OrderStatusId == 5).Count();

        }


        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }

        public async Task<JsonResult> OnPostAsync()
        {
            var recordsTotal = _context.Order.Count();

            var customersQuery = _context.Order.Include(a => a.CustomerAddress).Include(a => a.CustomerN).Include(e => e.OrderStatus).Include(e => e.Country).OrderByDescending(i => i.OrderDate).Where(o => o.OrderItem.Any()).Select(i => new OrderFiltterModel
            {
                OrderDate = i.OrderDate,
                Country = i.Country.CountryTlen,
                CusName = _context.CustomerNs.Where(e => e.CustomerId == i.CustomerAddress.CustomerId).FirstOrDefault().CustomerName,
                CusAddress = i.CustomerAddress.Country.CountryTlen + " ,  " + i.CustomerAddress.CityName + " , " + i.CustomerAddress.AreaName,
                NetOrder = i.OrderNet.Value,
                Status = i.OrderStatus.Status,
                SerialNo = i.OrderSerial,
                OrderId = i.OrderId,

            }).AsQueryable();

            var searchText = DataTablesRequest.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                customersQuery = customersQuery.Where(s =>
                    s.OrderId.ToString().Contains(searchText) ||
                    s.SerialNo.ToUpper().Contains(searchText) ||
                    s.OrderDate.ToString().ToUpper().Contains(searchText)
                );
            }

            var recordsFiltered = customersQuery.Count();

            var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
            var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

            // using System.Linq.Dynamic.Core
            customersQuery = customersQuery.OrderBy($"{sortColumnName} {sortDirection}");

            var skip = DataTablesRequest.Start;
            var take = DataTablesRequest.Length;
            var data =  customersQuery
                .Skip(skip)
                .Take(take)
                .ToList();

            return new JsonResult(new
            {
                draw = DataTablesRequest.Draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            });
        }
    }
}
