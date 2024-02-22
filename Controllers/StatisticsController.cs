using CRM.Data;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CRM.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StatisticsController : Controller
    {
        private PerfumeContext _context;

        public StatisticsController(PerfumeContext context)
        {
            _context = context;
        }
        [HttpGet]
        public object GetCountryOrders()
        {
            var data = _context.Order
                .GroupBy(c => c.CountryId).

                Select(g => new
                {

                    Lable =_context.Country.Where(e=>e.CountryId==g.Key).FirstOrDefault().CountryTlen,

                    Count = g.Count(),
                    Sum = g.Sum(e => e.OrderNet)

                }).OrderByDescending(r => r.Lable).ToList();

            return data;


        }
        [HttpGet]
        public List<object> GetDountChart()
        {
            List<object> dataDount = new List<object>();
            List<string> labels = new List<string>();
            List<double> Revenue = new List<double>();
            var countryList=_context.Country.Where(e => e.IsActive == true).ToList();
            foreach (var item in countryList)
            {
                labels.Add(item.CountryTlen);
                double countryRev = _context.Order.Where(e => e.IsPaid == true && e.CountryId == item.CountryId).Sum(e => e.OrderNet).Value;
                Revenue.Add(countryRev);
            }
            dataDount.Add(labels);
            dataDount.Add(Revenue);
            return dataDount;
        }


        [HttpGet]
        public List<object> GetNewOrderChart()
        {
            List<object> All = new List<object>();
            List<int> labels = new List<int>();
            List<int> CountOrder = new List<int>();
            List<double> Revenue = new List<double>();
            var data = _context.Order
                .GroupBy(c => c.OrderDate.Date.Month).

                Select(g => new
                {

                    labels = g.Key,
                    Count = g.Count(),
                    Sum = g.Sum(e=>e.OrderNet)

                }).ToList();

            
            foreach (var item in data)
            {
                labels.Add(item.labels);
                CountOrder.Add(item.Count);
                Revenue.Add(item.Count);
                
            }
            All.Add(labels);
            All.Add(CountOrder);
            All.Add(Revenue);
            
            return All;
        }


        [HttpGet]
        public object GetorderDetails()
        {
            var data = _context.Order
                .GroupBy(c => c.OrderDate.Date.Month).

                Select(g => new
                {

                    Lable = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),

                    Count = g.Count(),
                    Sum = g.Sum(e=>e.OrderNet)

                }).OrderByDescending(r => r.Count).ToList();

            return data;


        }

    }
}
