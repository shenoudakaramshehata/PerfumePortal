using CRM.Data;
using CRM.Reports;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Configurations.ManageReport
{
    public class ItemReportModel : PageModel
    {
        [BindProperty]
        public FinancialFilterVm financialFilterVm { get; set; }
        public ItemRPT Report { get; set; }

        private readonly PerfumeContext _context;

        public ItemReportModel(PerfumeContext context)
        {
            _context = context;
            financialFilterVm = new FinancialFilterVm();
        }

        public async Task<IActionResult> OnGet()
        {
            Report = new ItemRPT();
            List<ItemReportVm> ds = _context.OrderItem.Include(e => e.Order).ThenInclude(e=>e.Country).Where(o => o.Order.OrderStatusId==2|| o.Order.OrderStatusId == 5|| o.Order.OrderStatusId == 6 || o.Order.OrderStatusId == 7).OrderByDescending(e=>e.Order.OrderDate.Date).Select(i => new ItemReportVm
            {
                ItemId = i.ItemId.Value,
                CountryId = _context.Order.Where(o => o.OrderId == i.OrderId).FirstOrDefault().CountryId.Value,
                CountryName = _context.Order.Where(o => o.OrderId == i.OrderId).FirstOrDefault().Country.CountryTlen,
                ItemName = _context.Item.Where(o => o.ItemId == i.ItemId).FirstOrDefault().ItemTitleEn,
                OrderDate = i.Order.OrderDate,
                ItemPrice = i.ItemPrice,
                TotalQuantity = i.ItemQuantity,
                TotalAmount = _context.Order.Where(o => o.OrderId == i.OrderId).FirstOrDefault().OrderNet,
            }).ToList();
            Report.DataSource = ds;
            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            //try
            //{
            List<ItemReportVm> ds = _context.OrderItem.Include(e=>e.Order).Where(o => o.Order.IsPaid).Select(i => new ItemReportVm
            {
                ItemId=i.ItemId.Value,
                CountryId = _context.Order.Where(o => o.OrderId == i.OrderId).FirstOrDefault().CountryId.Value,
                CountryName = _context.Order.Where(o => o.OrderId == i.OrderId).FirstOrDefault().Country.CountryTlen,
                ItemName = _context.Item.Where(o => o.ItemId == i.ItemId).FirstOrDefault().ItemTitleEn,
                OrderDate = i.Order.OrderDate,
                ItemPrice = i.ItemPrice,
                TotalQuantity=i.ItemQuantity,
                TotalAmount = _context.Order.Where(o => o.OrderId == i.OrderId).FirstOrDefault().OrderNet,
            }).ToList();

            if (financialFilterVm.From != null && financialFilterVm.To == null)
            {
                ds = null;
            }
            if (financialFilterVm.From == null && financialFilterVm.To != null)
            {
                ds = null;
            }
            if (financialFilterVm.From != null && financialFilterVm.To != null)
            {
                ds = ds.Where(i => i.OrderDate.Date <= financialFilterVm.To.Value.Date && i.OrderDate.Date >= financialFilterVm.From.Value.Date).ToList();
            }
            if (financialFilterVm.CountryId != null)
            {
                ds = ds.Where(i => i.CountryId == financialFilterVm.CountryId).ToList();
            }
            if (financialFilterVm.ItemId != null)
            {
                ds = ds.Where(i => i.ItemId == financialFilterVm.ItemId).ToList();
            }
            
            if (financialFilterVm.ItemId == null && financialFilterVm.CountryId == null && financialFilterVm.From == null && financialFilterVm.To == null)
            {
                ds = new List<ItemReportVm>();
            }


            Report = new ItemRPT();
            Report.DataSource = ds;
            return Page();
            //}
            //catch
            //{

            //}
        }
    }
}
