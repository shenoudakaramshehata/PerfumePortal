using CRM.Data;
using CRM.Reports;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRM.Areas.Admin.Pages.Configurations.ManageReport
{
    public class FinancialReportModel : PageModel
    {
        [BindProperty]
        public FinancialFilterVm financialFilterVm { get; set; }
        public Financialrpt Report { get; set; }

        private readonly PerfumeContext _context;

        public FinancialReportModel(PerfumeContext context)
        {
            _context = context;
            financialFilterVm=new FinancialFilterVm();
        }

        public async Task<IActionResult> OnGet()
        {
            Report = new Financialrpt();
            List<FinancialVm> ds = _context.Order.Include(c => c.CustomerN).Include(c => c.Country).Where(o => o.OrderStatusId == 2 || o.OrderStatusId == 5 || o.OrderStatusId == 6 || o.OrderStatusId == 7).OrderByDescending(e => e.OrderDate.Date).Select(i => new FinancialVm
            {
                CountryId = i.CountryId,
                CountryName = i.Country.CountryTlen,
                CustomerName = _context.CustomerNs.Where(c => c.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                OrderDate = i.OrderDate,
                CustomerTele = _context.CustomerNs.Where(c => c.CustomerId == i.CustomerId).FirstOrDefault().Phone,
                OrderSerial = i.OrderSerial,
                OrderTotal = i.OrderNet.HasValue? i.OrderNet.Value:0,
            }).ToList();
            Report.DataSource = ds;
            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            //try
            //{
                List<FinancialVm> ds = _context.Order.Include(c => c.CustomerN).Include(c => c.Country).Where(o => o.IsPaid).Select(i => new FinancialVm
                {
                    CountryId = i.CountryId,
                    CountryName = i.Country.CountryTlen,
                    CustomerName = _context.CustomerNs.Where(c => c.CustomerId == i.CustomerId).FirstOrDefault().CustomerName,
                    OrderDate = i.OrderDate,
                    CustomerTele = _context.CustomerNs.Where(c => c.CustomerId == i.CustomerId).FirstOrDefault().Phone,
                    OrderSerial = i.OrderSerial,
                    OrderTotal = i.OrderNet.HasValue ? i.OrderNet.Value : 0,
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
                if (financialFilterVm.CustomerName != null)
                {
                    ds = ds.Where(ds => ds.CustomerName.Contains(financialFilterVm.CustomerName)).ToList();
                }
                if (financialFilterVm.CustomerTele != null)
                {
                    ds = ds.Where(ds => ds.CustomerTele.Contains(financialFilterVm.CustomerTele)).ToList();
                }
                if (financialFilterVm.OrderSerial != null)
                {
                    ds = ds.Where(ds => ds.OrderSerial.Contains(financialFilterVm.OrderSerial)).ToList();
                }
                if (financialFilterVm.CustomerName == null && financialFilterVm.OrderSerial == null && financialFilterVm.From == null && financialFilterVm.To == null && financialFilterVm.CustomerTele == null)
                {
                    ds = new List<FinancialVm>();
                }


                Report = new Financialrpt();
                Report.DataSource = ds;
                return Page();
            //}
            //catch
            //{
                
            //}
        }
    }
}
