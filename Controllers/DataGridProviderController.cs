using CRM.Data;
using CRM.Models;
using CRM.Services;
using CRM.ViewModels;
using DevExpress.DataAccess.Native.Sql;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataGridProviderController : Controller
    {
        private readonly PerfumeContext _perfumeContext;
        private readonly OrderService _orderService;
        public DataGridProviderController(PerfumeContext perfumeContext, OrderService orderService)
        {
            _perfumeContext = perfumeContext;
            _orderService = orderService;

        }

        [HttpGet]
        //[Route("GetrOrders")]
        public object GetrOrders(DataSourceLoadOptions loadOptions)
        {
            var data = _perfumeContext.Order.ToList();
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public async Task<IActionResult> GetrAllOrder(DataSourceLoadOptions loadOptions)
        {
            var orders = _perfumeContext.Order.Include(c=>c.CustomerN).Include(c=>c.Country).Select(c=> new OrderView
            {
                OrderId = c.OrderId,
                OrderDate = c.OrderDate,
                OrderSerial = c.OrderSerial,
                CustomerId = c.CustomerId,
                CustomerName= _perfumeContext.customerAddresses.Include(e=>e.CustomerN)
    .Where(x => x.CustomerAddressId == c.CustomerAddressId)
    .Select(e => e.CustomerN.CustomerName)
    .FirstOrDefault(),
            CustomerEmail = _perfumeContext.customerAddresses.Include(e => e.CustomerN)
    .Where(x => x.CustomerAddressId == c.CustomerAddressId)
    .Select(e => e.CustomerN.Email)
    .FirstOrDefault(),
                CustomerPhone = _perfumeContext.customerAddresses.Include(e => e.CustomerN)
    .Where(x => x.CustomerAddressId == c.CustomerAddressId)
    .Select(e => e.CustomerN.Phone)
    .FirstOrDefault(),
                OrderStatusId = c.OrderStatusId,
                OrderStatus=c.OrderStatus.Status,
                IsCanceled = c.IsCanceled,
                IsDeliverd = c.IsDeliverd,
                Notes = c.Notes,
                PaymentMethodId = c.PaymentMethodId,
                PaymentMethodTitleEn = _perfumeContext.paymentMehods.Where(x=>x.PaymentMethodId==c.PaymentMethodId).FirstOrDefault().PaymentMethodEN,
                PaymentMethodTitleAr = _perfumeContext.paymentMehods.Where(x=>x.PaymentMethodId==c.PaymentMethodId).FirstOrDefault().PaymentMethodAR,
                CustomerAddressId = c.CustomerAddressId,
                CountryId = c.CountryId,
                CountryTitleEn= _perfumeContext.Country.Where(x => x.CountryId == c.CountryId).FirstOrDefault().CountryTlen,
                CountryTitleAr = _perfumeContext.Country.Where(x => x.CountryId== c.CountryId).FirstOrDefault().CountryTlar,
                OrderTotal = c.OrderTotal,
                OrderDiscount = c.OrderDiscount,
                CouponId = c.CouponId,
                CouponTypeId = c.CouponTypeId,
                CouponAmount = c.CouponAmount,
                Deliverycost = c.Deliverycost,
                OrderNet = c.OrderNet,
                IsPaid = c.IsPaid,
                UniqeId = c.UniqeId,
                PaymentID = c.PaymentId,
                PostDate = c.PostDate,
                CustomerNCustomerId = c.CustomerAddressId,
                CustomerAddressCity=c.CustomerAddress.CityName,
                CustomerAddressArea = c.CustomerAddress.AreaName,
                CustomerAddressAddress = c.CustomerAddress.Address,
                CustomerAddressBuildingNo = c.CustomerAddress.BuildingNo,
                CustomerAddressMobile = c.CustomerAddress.Mobile,
                ShippingLabel = c.ShippingLabel,
                ShippingNo = c.ShippingNo,
                tax = c.tax,
                TotalAfterDiscount = c.TotalAfterDiscount,
                DiscountAmount = c.DiscountAmount,

            });

            return Json(await DataSourceLoader.LoadAsync(orders, loadOptions));
        }



        [HttpGet]
        public async Task<IActionResult> GetAllOrders(DataSourceLoadOptions loadOptions)
        {
            var orders = _perfumeContext.Order.Include(e => e.CustomerAddress).Include(e => e.OrderStatus).Include(c => c.CustomerN).Include(c => c.Country).Select(c => new OrdersDataGrid
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

                delieveryCost = c.Deliverycost.Value!=null? c.Deliverycost.Value:0,
                orderNet = c.OrderNet.Value!=null? c.OrderNet.Value:0,

                city = _perfumeContext.customerAddresses.Where(e=>e.CustomerAddressId==c.CustomerAddressId).FirstOrDefault().CityName,
                area = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == c.CustomerAddressId).FirstOrDefault().AreaName,
                Address = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == c.CustomerAddressId).FirstOrDefault().Address,

                SHIPMENTNUMBER = c.ShippingNo,
                tax = c.tax.Value!=null? c.tax.Value:0,
                status = _perfumeContext.OrderStatuses.Where(e => e.OrderStatusId == c.OrderStatusId).FirstOrDefault().Status,

                //orderItems = GetItems(c.OrderItem)

                Perfume1 = GetNames(c.OrderItem, _perfumeContext).Count>=1? GetNames(c.OrderItem, _perfumeContext)[0]:"",
                quantity1 = GetQunatites(c.OrderItem, _perfumeContext).Count >= 1 ? GetQunatites(c.OrderItem, _perfumeContext)[0] : 0,

                //Perfume2 = c.OrderItem.Count >= 2 ? c.OrderItem[1].Item.ItemTitleEn : "",
                //quantity2 = c.OrderItem.Count >= 2 ? c.OrderItem[1].ItemQuantity : 0,

                //Perfume3 = c.OrderItem.Count >= 3 ? c.OrderItem[2].Item.ItemTitleEn : "",
                //quantity3 = c.OrderItem.Count >= 3 ? c.OrderItem[2].ItemQuantity : 0
                //Perfume1 = _orderService.GetPerfumeTitle(c.OrderItem.FirstOrDefault()),
                //quantity1 = c.OrderItem.FirstOrDefault() != null ? c.OrderItem.First().ItemQuantity : 0,

                //Perfume2 = _orderService.GetPerfumeTitle(c.OrderItem.Skip(1).FirstOrDefault()),
                //quantity2 = c.OrderItem.Skip(1).FirstOrDefault() != null ? c.OrderItem.Skip(1).First().ItemQuantity : 0,

                //Perfume3 = _orderService.GetPerfumeTitle(c.OrderItem.Skip(2).FirstOrDefault()),
                //quantity3 = c.OrderItem.Skip(2).FirstOrDefault() != null ? c.OrderItem.Skip(2).First().ItemQuantity : 0

            }) ;

            return Json(await DataSourceLoader.LoadAsync(orders, loadOptions));
        }
        [HttpGet]
        public async Task<IActionResult> OrderItems(int OrderId, DataSourceLoadOptions loadOptions)
        {
            var ordersItem = _perfumeContext.OrderItem.Include(c => c.Item).Where(e=>e.OrderId==OrderId).Select(c => new ItemView
            {
                OrderId = c.OrderId,
                ItemPrice = c.ItemPrice,
                ItemQuantity= c.ItemQuantity,
                ItemId= c.ItemId,
                Total= c.Total,
                ItemTitle= c.Item.ItemTitleEn,



            });

            return Json(await DataSourceLoader.LoadAsync(ordersItem, loadOptions));
        }
        
        private static List<string> GetNames(ICollection<OrderItem> orderItems, PerfumeContext perfumeContext)
        {
            List<string> perfumeNames = new List<string>();

            foreach (var item in orderItems)
            {
                var itemobj =perfumeContext.Item.Find(item.ItemId);
                perfumeNames.Add(itemobj.ItemTitleEn);
                //Quantites.Add(item.ItemQuantity);
            }
            return perfumeNames;
        }

        private static List<int> GetQunatites(ICollection<OrderItem> orderItems, PerfumeContext perfumeContext)
        {
            List<int> Quantites = new List<int>();

            foreach (var item in orderItems)
            {
                var itemobj = perfumeContext.Item.Find(item.ItemId);
                //perfumeNames.Add(itemobj.ItemTitleEn);
                Quantites.Add(item.ItemQuantity);
            }
            return Quantites;
        }
        [HttpGet]
        public  IActionResult GetrAllOrderJson()
        {
            var orders = _perfumeContext.Order.Select(c => new
            {
                OrderId = c.OrderId,
                OrderDate = c.OrderDate,
                OrderSerial = c.OrderSerial,
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerN.CustomerName,
                CustomerEmail = c.CustomerN.Email,
                CustomerPhone = c.CustomerN.Phone,
                OrderStatusId = c.OrderStatusId,
                OrderStatusTitleAr = c.OrderStatus.Status,
                IsCanceled = c.IsCanceled,
                IsDeliverd = c.IsDeliverd,
                Notes = c.Notes,
                PaymentMethodId = c.PaymentMethodId,
                CustomerAddressId = c.CustomerAddressId,
                CountryId = c.CountryId,
                CountryTitleEn = c.Country.CountryTlen,
                CountryTitleAr = c.Country.CountryTlar,
                OrderTotal = c.OrderTotal,
                OrderDiscount = c.OrderDiscount,
                CouponId = c.CouponId,
                CouponTypeId = c.CouponTypeId,
                CouponAmount = c.CouponAmount,
                Deliverycost = c.Deliverycost,
                OrderNet = c.OrderNet,
                IsPaid = c.IsPaid,
                UniqeId = c.UniqeId,
                PaymentID = c.PaymentId,
                PostDate = c.PostDate,
                CustomerNCustomerId = c.CustomerAddressId,
                CustomerAddressCity = c.CustomerAddress.CityName,
                CustomerAddressArea = c.CustomerAddress.AreaName,
                CustomerAddressAddress = c.CustomerAddress.Address,
                CustomerAddressBuildingNo = c.CustomerAddress.BuildingNo,
                CustomerAddressMobile = c.CustomerAddress.Mobile,
                ShippingLabel = c.ShippingLabel,
                ShippingNo = c.ShippingNo,
                tax = c.tax,
                TotalAfterDiscount = c.TotalAfterDiscount,
                DiscountAmount = c.DiscountAmount,

            });

            return  Json(orders.Take(1));
        }



        [HttpGet]
        public IActionResult GetrOrders2(DataSourceLoadOptions loadOptions)
        {
            var orders = _perfumeContext.Order.ToList();

            return Json(DataSourceLoader.Load(orders, loadOptions));
        }


    }
}