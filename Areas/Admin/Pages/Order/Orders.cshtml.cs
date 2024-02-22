using CRM.Data;
using CRM.Models;
using CRM.Services;
using CRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CRM.Areas.Admin.Pages.Order
{
    public class OrdersModel : PageModel
    {
        private readonly PerfumeContext _perfumeContext;
        public List<OrdersDataGrid> orders { get; set; }
        public OrdersModel(PerfumeContext perfumeContext)
        {
            _perfumeContext = perfumeContext;
            orders = new List<OrdersDataGrid>();

        }
        public void OnGet()
        {
             orders = _perfumeContext.Order.Include(e => e.CustomerAddress).Include(e => e.OrderStatus).Include(e=>e.OrderItem).Include(c => c.CustomerN).Include(c => c.Country).Select(c => new OrdersDataGrid
            {
                orderId = c.OrderId,
                ORDERDATE = c.OrderDate,


                customerName = _perfumeContext.customerAddresses.Include(e => e.CustomerN)
.Where(x => x.CustomerAddressId == c.CustomerAddressId)
.Select(e => e.CustomerN.CustomerName)
.FirstOrDefault(),
                email = _perfumeContext.customerAddresses.Include(e => e.CustomerN)
.Where(x => x.CustomerAddressId == c.CustomerAddressId)
.Select(e => e.CustomerN.Email)
.FirstOrDefault(),
                customerPhone = _perfumeContext.customerAddresses.Include(e => e.CustomerN)
.Where(x => x.CustomerAddressId == c.CustomerAddressId)
.Select(e => e.CustomerN.Phone)
.FirstOrDefault(),

                notes = c.Notes,

                paymentMethod = _perfumeContext.paymentMehods.Where(x => x.PaymentMethodId == c.PaymentMethodId).FirstOrDefault().PaymentMethodEN,


                country = _perfumeContext.Country.Where(x => x.CountryId == c.CountryId).FirstOrDefault().CountryTlen,

                orderTotal = c.OrderTotal,
                discount = c.OrderDiscount,

                delieveryCost = c.Deliverycost.Value != null ? c.Deliverycost.Value : 0,
                orderNet = c.OrderNet.Value != null ? c.OrderNet.Value : 0,

                city = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == c.CustomerAddressId).FirstOrDefault().CityName,
                area = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == c.CustomerAddressId).FirstOrDefault().AreaName,
                Address = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == c.CustomerAddressId).FirstOrDefault().Address,

                SHIPMENTNUMBER = c.ShippingNo,
                tax = c.tax.Value != null ? c.tax.Value : 0,
                status = _perfumeContext.OrderStatuses.Where(e => e.OrderStatusId == c.OrderStatusId).FirstOrDefault().Status,

                 //orderItems = GetItems(c.OrderItem)

                 //Perfume1 = GetNames(c.OrderItem).Count >= 1 ? GetNames(c.OrderItem)[0] : "",
                 //quantity1 = GetQunatites(c.OrderItem).Count >= 1 ? GetQunatites(c.OrderItem)[0] : 0,

                 //Perfume2 = c.OrderItem.Count >= 2 ? c.OrderItem[1].Item.ItemTitleEn : "",
                 //quantity2 = c.OrderItem.Count >= 2 ? c.OrderItem[1].ItemQuantity : 0,

                 //Perfume3 = c.OrderItem.Count >= 3 ? c.OrderItem[2].Item.ItemTitleEn : "",
                 //quantity3 = c.OrderItem.Count >= 3 ? c.OrderItem[2].ItemQuantity : 0
                 Perfume1 = c.OrderItem.FirstOrDefault() != null ? c.OrderItem.First().Item.ItemTitleEn : "",
                 quantity1 = c.OrderItem.FirstOrDefault() != null ? c.OrderItem.First().ItemQuantity : 0,

                 Perfume2 = c.OrderItem.Skip(1).FirstOrDefault() != null ? c.OrderItem.Skip(1).First().Item.ItemTitleEn : "",
                 quantity2 = c.OrderItem.Skip(1).FirstOrDefault() != null ? c.OrderItem.Skip(1).First().ItemQuantity : 0,

                 Perfume3 = c.OrderItem.Skip(2).FirstOrDefault() != null ? c.OrderItem.Skip(2).First().Item.ItemTitleEn : "",
                 quantity3 = c.OrderItem.Skip(2).FirstOrDefault() != null ? c.OrderItem.Skip(2).First().ItemQuantity : 0,
                 Perfume4 = c.OrderItem.Skip(3).FirstOrDefault() != null ? c.OrderItem.Skip(3).First().Item.ItemTitleEn : "",
                 quantity4 = c.OrderItem.Skip(3).FirstOrDefault() != null ? c.OrderItem.Skip(3).First().ItemQuantity : 0,
                  Perfume5 = c.OrderItem.Skip(4).FirstOrDefault() != null ? c.OrderItem.Skip(4).First().Item.ItemTitleEn : "",
                 quantity5 = c.OrderItem.Skip(4).FirstOrDefault() != null ? c.OrderItem.Skip(4).First().ItemQuantity : 0,

                 Perfume6 = c.OrderItem.Skip(5).FirstOrDefault() != null ? c.OrderItem.Skip(5).First().Item.ItemTitleEn : "",
                 quantity6 = c.OrderItem.Skip(5).FirstOrDefault() != null ? c.OrderItem.Skip(5).First().ItemQuantity : 0,
                 
                 
                 Perfume7 = c.OrderItem.Skip(6).FirstOrDefault() != null ? c.OrderItem.Skip(6).First().Item.ItemTitleEn : "",
                 quantity7 = c.OrderItem.Skip(6).FirstOrDefault() != null ? c.OrderItem.Skip(6).First().ItemQuantity : 0,
                 
                 Perfume8 = c.OrderItem.Skip(7).FirstOrDefault() != null ? c.OrderItem.Skip(7).First().Item.ItemTitleEn : "",
                 quantity8 = c.OrderItem.Skip(7).FirstOrDefault() != null ? c.OrderItem.Skip(7).First().ItemQuantity : 0,
                 
                 Perfume9 = c.OrderItem.Skip(8).FirstOrDefault() != null ? c.OrderItem.Skip(8).First().Item.ItemTitleEn : "",
                 quantity9 = c.OrderItem.Skip(8).FirstOrDefault() != null ? c.OrderItem.Skip(8).First().ItemQuantity : 0,
                 Perfume10 = c.OrderItem.Skip(9).FirstOrDefault() != null ? c.OrderItem.Skip(9).First().Item.ItemTitleEn : "",
                 quantity10 = c.OrderItem.Skip(9).FirstOrDefault() != null ? c.OrderItem.Skip(9).First().ItemQuantity : 0,
                 
             }).ToList();

        }
        private  List<string> GetNames(ICollection<OrderItem> orderItems)
        {
            List<string> perfumeNames = new List<string>();

            foreach (var item in orderItems)
            {
                var itemobj = _perfumeContext.Item.Find(item.ItemId);
                perfumeNames.Add(itemobj.ItemTitleEn);
                //Quantites.Add(item.ItemQuantity);
            }
            return perfumeNames;
        }

        private  List<int> GetQunatites(ICollection<OrderItem> orderItems)
        {
            List<int> Quantites = new List<int>();

            foreach (var item in orderItems)
            {
                var itemobj = _perfumeContext.Item.Find(item.ItemId);
                //perfumeNames.Add(itemobj.ItemTitleEn);
                Quantites.Add(item.ItemQuantity);
            }
            return Quantites;
        }
    }
}
