using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Areas.Admin.Pages.Configurations.ManageDashboard
{

    //public class IndexModel : PageModel
    //{
    //    private PerfumeContext _context;
    //    private readonly IWebHostEnvironment _hostEnvironment;
    //    private readonly IToastNotification _toastNotification;

     


    //    [BindProperty]
    //    public Order order { get; set; }


    //    public List<Order> orderlist = new List<Order>();
    //    public Order orderObject { get; set; }
    //    public int CountPaidOrders { get; set; }
    //    public int CustomersCount { get; set; }
    //    public int OrdersActiveCounts { get; set; }
    //    public double TotalAmountOfPaidOrders { get; set; }
    //    public double TotalAmountOfOrders { get; set; }
    //    public IEnumerable<Order> LatestOrders { get; set; }
    //    public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
    //    {
    //        _context = context;
    //        _hostEnvironment = hostEnvironment;
    //        _toastNotification = toastNotification;
    //    }
    //    public void OnGet()
    //    {
    //        orderlist = _context.Order.ToList();
    //        CountOrdersPaid();
    //        CustomerCount();
    //        CountOfActiveOrders();
    //        TotalAmountOfAllPaidOrders();
    //        TotalAmountOfAllOrders();
    //        latestOrders();
    //    }


    //    private void CountOrdersPaid()
    //    {
    //        CountPaidOrders = orderlist.Where(i=> i.IsPaid).Count();
    //    }

    //    private void CustomerCount()
    //    {
    //        CustomersCount = orderlist.DistinctBy(c=>c.CustomerId).Count();
    //    }

    //    private void CountOfActiveOrders()
    //    {
    //        OrdersActiveCounts = orderlist.Where(OrderPaid => OrderPaid.IsPaid 
    //                                    && OrderPaid.IsDeliverd!=true).Count();
    //    }

    //    private void TotalAmountOfAllPaidOrders()
    //    {
    //        TotalAmountOfPaidOrders = orderlist.Where(order=> order.IsPaid).Sum(order => order.OrderTotal);
    //    }

    //    private void TotalAmountOfAllOrders()
    //    {
    //        TotalAmountOfOrders = orderlist.Sum(order => order.OrderTotal);
    //    }

    //    private void latestOrders()
    //    {
    //        LatestOrders = _context.Order.Include(c => c.CustomerN).OrderByDescending(o=>o.OrderDate).ToList();
    //    }

    //}
}
