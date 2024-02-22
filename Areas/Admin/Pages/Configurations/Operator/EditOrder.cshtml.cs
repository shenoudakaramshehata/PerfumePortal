using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit;
using NToastNotify;
using StackExchange.Redis;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace CRM.Areas.Admin.Pages.Configurations.Operator
{
    public class EditOrderModel : PageModel
    {
        private readonly PerfumeContext _context;
        [BindProperty]
        public ViewModels.EditOrder editOrder { get; set; }
        [BindProperty]
        public OrderItem orderItem { get; set; }
        public List<OrderItem> OrderList { get; set; }
        public CRM.Models.Order OrderReceipt { get; set; }
        [BindProperty]
        public CRM.Models.Order order { get; set; }
        public CustomerAddress customerAddress { get; set; }
        public double taxpercentage { get; set; }
        public CustomerN Customer { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodAR { get; set; }
        public double tax { get; set; }
        public string CurrencyEN { get; set; }
        public string CurrencyNameAr { get; set; }
        public double DeliveryCost { get; set; }
        public double OrdernetByTax { get; set; }
        public double approximatedNumber { get; set; }
        [BindProperty]
        public CustomerVM customerVM { get; set; }
        public double DiscountAmount { get; set; }
        public List<ShippingMatrix> shippingMatrices { get; set; }

        public double TotalWeight { get; set; }
        public double DeliverCost { get; set; }
        public List<OrderTraking> OrderTraking { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public EditOrderModel(PerfumeContext perfumeContext, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = perfumeContext;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;
            order = new Models.Order();
            editOrder = new ViewModels.EditOrder();
            orderItem = new OrderItem();
            customerVM = new CustomerVM();
        }

        public IActionResult OnGet([FromQuery] int Id)
        {
            try
            {
                OrderList = _context.OrderItem.Include(i => i.Item).Include(i => i.Order)
                                           .ThenInclude(a => a.CustomerAddress).ThenInclude(a => a.CustomerN)
                                       .Where(o => o.OrderId == Id).ToList();


                order = _context.Order.Where(o => o.OrderId == Id).FirstOrDefault();
                OrderReceipt = _context.Order.Where(i => i.OrderId == Id).Include(a => a.OrderStatus).FirstOrDefault();

                customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == OrderReceipt.CustomerAddressId).FirstOrDefault();
                var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
                Customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

                CurrencyEN = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlen;
                CurrencyNameAr = _context.Country.Include(e => e.Currency).Where(e => e.CountryId == _context.Country.Where(e => e.CountryId == OrderReceipt.CountryId).FirstOrDefault().CountryId).FirstOrDefault().Currency.CurrencyTlar;
                tax = OrderReceipt.tax.HasValue ? OrderReceipt.tax.Value : 0;
                DeliveryCost = OrderReceipt.Deliverycost.HasValue ? OrderReceipt.Deliverycost.Value : 0;
                OrdernetByTax = (OrderReceipt.OrderTotal * tax);
                double originalNumber;
                if (OrderReceipt.OrderNet != null)
                {
                    originalNumber = OrderReceipt.OrderNet.Value;

                }
                else
                {
                    originalNumber = 0;
                }
                int decimalPlaces = 2;
                approximatedNumber = Math.Round(originalNumber, decimalPlaces);
                DiscountAmount = (OrderReceipt.DiscountAmount.HasValue ? OrderReceipt.DiscountAmount.Value : 0) + (OrderReceipt.CouponAmount.HasValue ? (OrderReceipt.CouponTypeId == 1 ? (OrderReceipt.CouponAmount.Value / 100) * OrderReceipt.OrderTotal : OrderReceipt.CouponAmount.Value) : 0);
                taxpercentage = tax * 100;


                OrderTraking = _context.OrderTrakings.Include(i => i.OrderStatus).Where(e => e.OrderId == Id).ToList();
                if (OrderReceipt.PaymentMethodId != null)
                {
                    PaymentMethod = _context.paymentMehods.Where(e => e.PaymentMethodId == OrderReceipt.PaymentMethodId).FirstOrDefault().PaymentMethodEN;
                    PaymentMethodAR = _context.paymentMehods.Where(e => e.PaymentMethodId == OrderReceipt.PaymentMethodId).FirstOrDefault().PaymentMethodAR;
                }
            }
            catch (Exception e)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Page();
        }


        public async Task<IActionResult> OnPost(int Id)
        {
            try
            {
                var orderObj = _context.Order.Where(e => e.OrderId == Id).FirstOrDefault();
                if (orderObj != null)
                {
                    orderObj.ShippingNo = order.ShippingNo;

                    var UpdatedOrder = _context.Order.Attach(orderObj);
                    UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("ShippingNo Updated Successfully");
                }


            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
                return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + Id);

            }


            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + Id);
        }

        public IActionResult OnGetSingleorderForEdit(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            if (order != null)
            {
                editOrder = new ViewModels.EditOrder()
                {
                    OrderId = order.OrderId,

                    DiscountAmount = order.DiscountAmount!=null ? order.DiscountAmount.Value : 0, // Handle the case when DiscountAmount is null
                    OrderDate = order.OrderDate
                };


                return new JsonResult(editOrder);
            }
            else
            {
                return new JsonResult("not found");
            }
        }

        public IActionResult OnGetSingleorderstatusForEdit(int OrderId)
        {
            order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
             return new JsonResult(order);

        }
        public IActionResult OnGetSingleorderNotesForEdit(int OrderId)
        {
            order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(order);

        }
        

        public IActionResult OnGetSinglepaymentMethodForEdit(int OrderId)
        {
            order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            return new JsonResult(order);

        }


        public async Task<IActionResult> OnPostEditpaymentmethod(int OrderId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

            }
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
                }
                var PaymentMethod = Request.Form["PaymentMethod"];
                var PaymentMethodId = int.Parse(PaymentMethod);

                model.PaymentMethodId = PaymentMethodId;

            
                var UpdatedOrder = _context.Order.Attach(model);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Payment Method Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

        }
        
                    public async Task<IActionResult> OnPostEditOrderNotes(int OrderId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

            }
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
                }
                var ordernote = Request.Form["orderNotes"];

                model.Notes = ordernote;



                var UpdatedOrder = _context.Order.Attach(model);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Order Notes Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

        }

        public async Task<IActionResult> OnPostEditOrderStatus(int OrderId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

            }
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
                }
                var orderstatus = Request.Form["OrderStatus"];
                var orderStatusId = int.Parse(orderstatus);

                model.OrderStatusId = orderStatusId;

                OrderTraking orderTrakingObj = new OrderTraking()
                {
                    OrderId = OrderId,
                    ActionDate = DateTime.Now,
                    Remarks = "",
                    OrderStatusId = orderStatusId
                };
                _context.OrderTrakings.Add(orderTrakingObj);


                var UpdatedOrder = _context.Order.Attach(model);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
               
                _toastNotification.AddSuccessToastMessage("Order Status Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

        }
        public IActionResult OnGetSingleCustomerForEdit(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
            var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
          var customer = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);
            
                customerVM = new CustomerVM()
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    Mobile = customer.Phone,
                    Email = customer.Email,
                   OrderId= OrderId

                };

            return new JsonResult(customerVM);

        }
        public IActionResult OnGetSingleCustomerAddressForEdit(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
           
             customerVM = new CustomerVM()
                {
                   
                    Address = customerAddress.Address != null ? customerAddress.Address : "",
                    CountryId = customerAddress.CountryId.Value != null ? customerAddress.CountryId.Value : 0,
                    AreaName = customerAddress.AreaName != null ? customerAddress.AreaName : "",
                    BuildingNo = customerAddress.BuildingNo != null ? customerAddress.BuildingNo : "",
                    CityName = customerAddress.CityName != null ? customerAddress.CityName : "",
                    CustomerAddressId = customerAddress.CustomerAddressId != null ? customerAddress.CustomerAddressId : 0,

                };
            

            return new JsonResult(customerVM);

        }

        public async Task<IActionResult> OnPostEditCustomerAddress(int OrderId)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
            }
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
                if (customerAddress != null)
                {
                    customerAddress.Address = customerVM.Address;
                    customerAddress.AreaName = customerVM.AreaName;
                    customerAddress.BuildingNo = customerVM.BuildingNo;
                    customerAddress.CityName = customerVM.CityName;
                    var UpdatedCustomerAddress = _context.customerAddresses.Attach(customerAddress);
                    UpdatedCustomerAddress.State = EntityState.Modified;
                }
                
                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Customer Address Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
        }


        public async Task<IActionResult> OnPostEditCustomer(int OrderId)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
            }
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                var customerAddress = _context.customerAddresses.Where(c => c.CustomerAddressId == order.CustomerAddressId).FirstOrDefault();
                var customerId = _context.CustomerNs.Where(o => o.CustomerId == customerAddress.CustomerId).FirstOrDefault().CustomerId;
                var model = _context.CustomerNs.FirstOrDefault(c => c.CustomerId == customerId);

                var userExists = await _userManager.FindByEmailAsync(customerVM.Email);
                if (userExists != null)
                {
                    _toastNotification.AddErrorToastMessage("Email is already exist");
                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

                }

                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Customer Not Found");

                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                var countryob = _context.Country.Where(e => e.CountryId == order.CountryId).FirstOrDefault().CountryTlen;


                if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                {
                    if (order.CountryId == 2 && customerVM.Mobile.StartsWith("0"))
                    {
                        customerVM.Mobile = customerVM.Mobile.Substring(1);
                    }
                    if (customerVM.Mobile.StartsWith(countryCode))
                    {
                        // User entered phone number with country code, remove the duplicate country code
                        customerVM.Mobile = customerVM.Mobile.Substring(countryCode.Length);
                    }
                    customerVM.Mobile = countryCode + customerVM.Mobile;
                }
                if (user != null)
                {
                    user.PhoneNumber = customerVM.Mobile;
                    user.FullName = customerVM.CustomerName;
                    user.Email = customerVM.Email;
                    await _userManager.UpdateAsync(user);

                }

                model.Email = customerVM.Email;
                model.Phone = customerVM.Mobile;
                model.CustomerName = customerVM.CustomerName;

                var Updatedcoupon = _context.CustomerNs.Attach(model);

                Updatedcoupon.State = EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Customer Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
        }

        public async Task<IActionResult> OnPostEditorder(int OrderId)
        {
            //var error = ModelState.Values.SelectMany(v => v.Errors);
            //if (!ModelState.IsValid)
            //{
            //    _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

            //    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

            //}
            try
            {
                var model = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
                }
                if (editOrder.DiscountAmount > model.OrderNet)
                {
                    ModelState.AddModelError("Order.Discount", "Discount cannot exceed net total.");
                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

                }


                string date = Request.Form["datepicker"];
                Console.WriteLine("Date from Form: " + date); // Debugging statement
                var datt = Convert.ToDateTime(date);


                model.OrderDate = datt;
                model.DiscountAmount = editOrder.DiscountAmount;
               
                var Updatedorder = _context.Order.Attach(model);
                Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                Recalculate(OrderId);
                _toastNotification.AddSuccessToastMessage("Order Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

        }


        public IActionResult OnGetSingleItemForEdit(int OrderId,int ItemId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            orderItem = _context.OrderItem.Where(e=>e.ItemId== ItemId && e.OrderId== order.OrderId).FirstOrDefault();
          
            
            //if (order != null)
            //{
            //    editOrder = new ViewModels.EditOrder()
            //    {
            //        OrderId = order.OrderId,

            //        DiscountAmount = order.DiscountAmount != null ? order.DiscountAmount.Value : 0, // Handle the case when DiscountAmount is null
            //        OrderDate = order.OrderDate
            //    };


                return new JsonResult(orderItem);
            }


        public async Task<IActionResult> OnPostEditItem(int OrderId, int ItemId)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

            }
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                var model = _context.OrderItem.Where(e => e.ItemId == ItemId && e.OrderId == order.OrderId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
                }
               

                

                var item = Request.Form["item"];
                var itemId = int.Parse(item);
                model.ItemId = itemId;
                var itemprice = _context.itemPriceNs.Where(e => e.ItemId == itemId && e.CountryId == order.CountryId).FirstOrDefault().Price;
                model.ItemQuantity = orderItem.ItemQuantity;
                model.ItemPrice = itemprice.Value;
                model.Total= model.ItemPrice * model.ItemQuantity;
                var UpdatedorderItem = _context.OrderItem.Attach(model);
                UpdatedorderItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                
                _context.SaveChanges();
                order.OrderTotal = _context.OrderItem.Where(oi => oi.OrderId == order.OrderId).Sum(oi => oi.Total); ;
                var Updatedorder = _context.Order.Attach(order);
                Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
                Recalculate(order.OrderId);


                _toastNotification.AddSuccessToastMessage("Item Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);

        }

        public IActionResult OnGetSingleItemForDelete(int OrderId)
        {
            var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
            orderItem = _context.OrderItem.Where(e =>e.OrderId == order.OrderId).FirstOrDefault();
            return new JsonResult(orderItem);
        }

        public async Task<IActionResult> OnPostDeleteOrderItem(int OrderId)
        {
            try
            {
                var order = _context.Order.Where(c => c.OrderId == OrderId).FirstOrDefault();
                orderItem = _context.OrderItem.Where(e => e.OrderId == order.OrderId).FirstOrDefault();

                var orderItemsList = _context.OrderItem.Where(e => e.OrderId == order.OrderId).ToList();
                if (orderItemsList.Count == 0)
                {
                    _context.Order.Remove(order);

                    await _context.SaveChangesAsync();
                    return Redirect("/Admin/Configurations/Operator/Orders");

                }

                if (orderItem != null)
                {


                    _context.OrderItem.Remove(orderItem);

                    await _context.SaveChangesAsync();
                    order.OrderTotal = _context.OrderItem.Where(oi => oi.OrderId == order.OrderId).Sum(oi => oi.Total); ;
                    var Updatedorder = _context.Order.Attach(order);
                    Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    Recalculate(order.OrderId);

                    _toastNotification.AddSuccessToastMessage("order Item Deleted successfully");


                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
                }
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }

            return Redirect("/Admin/Configurations/Operator/EditOrder?Id=" + OrderId);
        }
        public void Recalculate(int OrderId)
        {
            var order = _context.Order.Where(e => e.OrderId == OrderId).Include(a => a.OrderItem).FirstOrDefault();

            if (order != null)
            {
                shippingMatrices = _context.ShippingsMatrix.Where(e => e.CountryId == order.CountryId).ToList();
                order.TotalWeight = 0;
                foreach (var orderItem in order.OrderItem)
                {
                    var itemWeight = _context.Item.Where(e => e.ItemId == orderItem.ItemId).FirstOrDefault().Weight;
                    if (itemWeight != null)
                    {
                        order.TotalWeight += (double)(itemWeight * orderItem.ItemQuantity);
                    }
                }
                foreach (var shipCost in shippingMatrices)
                {
                    if (order.TotalWeight == shipCost.ToWeight || order.TotalWeight == shipCost.FromWeight || (order.TotalWeight > shipCost.FromWeight && order.TotalWeight < shipCost.ToWeight) || (order.TotalWeight < shipCost.FromWeight && order.TotalWeight > shipCost.ToWeight))
                    {
                        DeliveryCost = shipCost.ActualPrice;
                    }

                }

                order.Deliverycost = DeliveryCost;

                if (order.OrderItem != null)
                {
                    var UpdatedorderItems = _context.Order.Attach(order);
                    UpdatedorderItems.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                }

                var subTotal = order.OrderTotal;
                var ShippingCost = order.Deliverycost;
                var tax = order.tax;
                var couponDiscount = order.OrderDiscount;
                var discountAmount = order.DiscountAmount;
                double Discount = 0;
                double TotalAmountAfterDiscount = 0;
                double sumItemTotal = subTotal;
                var percent = sumItemTotal / subTotal;
                
                if (order.CouponId == null)
                {
                    Discount = 0;
                    TotalAmountAfterDiscount = sumItemTotal;
                }
                else if (order.CouponTypeId == 2)
                {
                   
                    Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - order.CouponAmount * percent), 2);

                    var AmountAfterDiscount = (double)(sumItemTotal - order.CouponAmount * percent);
                    TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
                }
                else
                {
                   
                    var couponAmount = subTotal * (order.CouponAmount / 100);
                    Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - couponAmount * percent), 2);
                    var AmountAfterDiscount = (double)(sumItemTotal - couponAmount * percent);
                    TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
                }
                if (TotalAmountAfterDiscount < 0)
                {
                    TotalAmountAfterDiscount = 0;
                }

                var OrderTaxValue = (TotalAmountAfterDiscount * tax);
                //var TotalAfterDiscount = order.TotalAfterDiscount;
                var NetOrder = tax == 0 ? (TotalAmountAfterDiscount + ShippingCost) : TotalAmountAfterDiscount+ OrderTaxValue + ShippingCost;
                var finalorderNet = NetOrder - discountAmount;
                order.OrderNet = Math.Round((double)finalorderNet, 2);
                order.TotalAfterDiscount = TotalAmountAfterDiscount;
                var Updatedorder = _context.Order.Attach(order);
                Updatedorder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
        }
        private readonly Dictionary<string, string> CountryCodeMappings = new Dictionary<string, string>
        {
            { "BH", "+973" },
            { "KW", "+965" },
            { "OM", "+968" },
            { "QA", "+974" },
            { "SA", "+966" },
            { "AE", "+971" }
        };

    }
}

