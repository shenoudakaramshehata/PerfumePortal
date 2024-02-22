using CRM.Data;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.Runtime;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using DevExpress.XtraRichEdit.Import.Html;
using CRM.Migrations;
using MimeKit;
using System.Net.Http.Headers;
using System.Text;
using CRM.Services;
using RestSharp;
using Newtonsoft.Json.Linq;
using CRM.Services.TabbyModels;

namespace CRM.Pages
{
    public class CheckOutFastOrderModel : PageModel
    {
        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailSender _emailSender;
        public InvoiceVm invoiceVm { get; set; }
        private readonly IRazorPartialToStringRenderer _renderer;

        //[BindProperty(SupportsGet = true)]
        //public string CouponSerial { get; set; }
        public string url { get; set; }
        public double Subtotal { get; set; }
        public double OrdernetByTax { get; set; }
        public Order orders { get; set; }

        public double OrdernetByInstallments { get; set; }

        [BindProperty]
        public List<ShoppingCart> shoppingCarts { get; set; }
        [BindProperty]
        public List<PaymentMehod> paymentMehods { get; set; }
        [BindProperty]
        public CustomerAddress? customerAddr { get; set; }
        //[BindProperty]
        //public CustomerAddressVM newCustomerAddressVM { get; set; }
        [BindProperty]
        public int Payment { get; set; }
        public Coupon? coupon { get; set; }
        public double TotalAmount { get; set; }
        //public int couponID { get; set; }
        //[BindProperty]
        //public int countryId { get; set; } 
        //public bool hasAddress { get; set; }
        public string newserial { get; set; }
        [BindProperty]
        public int PaymentId { get; set; }
        public double taxvalue { get; set; }
        //[BindProperty]
        public int FattorahPaymentId { get; set; }

        public int CahshPaymentId { get; set; }
        public int TabbyPaymentId { get; set; }

        public HttpClient httpClient { get; set; }

        public double Discount { get; set; }
        public double? ShippingCost { get; set; }

        public bool IsDiscounted { get; set; } = false;
        public double TotalAmountAfterDiscount { get; set; }

        public string CurrencyNameAr { get; set; }
        [BindProperty]
        public FastOrderVM FastOrderVM { get; set; }
        public string CurrencyNameEN { get; set; }
        public string CountryENName { get; set; }
        public string CountryARName { get; set; }
        public double DeliveryCost { get; set; }
        public ApplicationUser user { get; set; }
        public string CountryToShow { get; set; }
        public int CountryId { get; set; }
        public double tax { get; set; }
        public List<ShippingCartObjectVM> shippingCartObjectVMs { get; set; }
        public static string rejectionReason { get; set; }
        public List<ShippingMatrix> shippingMatrices { get; set; }
        public double TotalWeight { get; set; }
        public double DeliverCost { get; set; }
        public CheckOutFastOrderModel(IRazorPartialToStringRenderer renderer, PerfumeContext perfumeContext, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IConfiguration configuration, IWebHostEnvironment hostEnvironment, IEmailSender emailSender)
        {
            _renderer = renderer;
            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;
            this.userManager = userManager;
            this._configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            httpClient = new HttpClient();
            shippingCartObjectVMs = new List<ShippingCartObjectVM>();
        }

        public async Task<IActionResult> OnGet(string Serial)
        {
         

            user = await userManager.GetUserAsync(User);
            var Country = HttpContext.Session.GetString("country");
            if (Country != null)
            {
                CountryId = int.Parse(Country);
                tax = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value * 100;
               taxvalue= perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value ;
                CountryENName = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;
                CurrencyNameEN = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlen;
                CurrencyNameAr = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlar;
                DeliveryCost = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().ShippingCost.Value;
                shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();


            }

            if (user != null)
            {
                var customerId = perfumeContext.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;
                 shoppingCarts = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item).Include(i => i.CustomerN).ToList();
                TotalAmount = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);
                customerAddr = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerId).FirstOrDefault();
                foreach (var item in shoppingCarts)
                {
                    if (item != null)
                    {
                        if (item.ItemQty >= 1)
                        {
                            TotalWeight += (item.ItemQty * (item.Item.Weight ?? 0.0));
                        }


                    }

                }
                foreach (var shipCost in shippingMatrices)
                {
                    if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                    {
                        DeliveryCost = shipCost.ActualPrice;
                    }

                }
                //TotalAmount += DeliveryCost+tax;
                Subtotal = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);

                OrdernetByTax = (TotalAmount * taxvalue) ;
                TotalAmount = Subtotal + DeliveryCost + OrdernetByTax;
                OrdernetByInstallments = TotalAmount / 4;
            }
            else
            {
                var data = HttpContext.Session.GetString("parsecartItems");
                if (data != null)
                {

                    //List<ShippingCartObjectVM> shippingCartObjectVMs = JsonSerializer.Deserialize<List<ShippingCartObjectVM>>(data);
                    shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(data);
                    if (shippingCartObjectVMs != null)
                    {
                        foreach (var item in shippingCartObjectVMs)
                        {
                            if (item != null)
                            {

                                if (item.Qunatity >= 1)
                                {
                                    var productWeight = perfumeContext.Item.Where(a => a.ItemId == item.ItemId).FirstOrDefault().Weight;
                                    if (productWeight != null)
                                    {
                                        TotalWeight += (item.Qunatity * (productWeight ?? 0.0));

                                    }
                                }


                            }

                        }
                    }
                        foreach (var shipCost in shippingMatrices)
                        {
                        if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                        {
                            DeliveryCost = shipCost.ActualPrice;
                        }

                    }
                    foreach (var item in shippingCartObjectVMs)
                    {
                        Subtotal = TotalAmount + (item.priceCart * item.Qunatity);

                        TotalAmount = TotalAmount + (item.priceCart * item.Qunatity);
                        

                    }
                    OrdernetByTax = (TotalAmount * taxvalue) ;
                    TotalAmount = Subtotal + DeliveryCost + OrdernetByTax;
                    OrdernetByInstallments = TotalAmount / 4;
                    //TotalAmount += DeliveryCost + tax ;


                }
            };
            var CouponSerialdata = HttpContext.Session.GetString("newCouponSerial");
            newserial = CouponSerialdata;
            
          
           
            PaymentId = perfumeContext.paymentMehods.FirstOrDefault().PaymentMethodId;
            FattorahPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 1).FirstOrDefault().PaymentMethodId;
            CahshPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 2).FirstOrDefault().PaymentMethodId;
            //TabbyPaymentId = perfumeContext.paymentMehods.Where(e => e.PaymentMethodId == 3).FirstOrDefault().PaymentMethodId;           

            paymentMehods = perfumeContext.paymentMehods.ToList();

            coupon = perfumeContext.Coupon.Where(i => i.Serial == newserial).FirstOrDefault();


            GetDiscountFromCoupon(TotalAmount, coupon);


            return Page();
        }



        public async Task<IActionResult> OnPostAddCustomerAddress(string Serial, string FullName, string Email, string PhoneNumber, string Address, string CityName, string AreaName, string BuildingNo, string Notes, string ZIP, bool IsCreateAccountChecked, string Password)

        {
            try
            {
                bool outofStock = false;
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    if (IsCreateAccountChecked)
                    {
                        var userExists = await userManager.FindByEmailAsync(Email);
                        if (userExists != null)
                        {
                            toastNotification.AddErrorToastMessage("Email is already exist");
                            return Page();
                        }
                        user = new ApplicationUser { UserName = Email, Email = Email, FullName = FullName, PhoneNumber = PhoneNumber };
                        var result = await userManager.CreateAsync(user, Password);

                        if (result.Succeeded)
                        {

                            await userManager.AddToRoleAsync(user, "Customer");

                        }
                        else
                        {
                            toastNotification.AddErrorToastMessage("Something went Error");
                            return Page();
                        }
                    }

                    var data = HttpContext.Session.GetString("parsecartItems");
                    List<ShoppingCart> EmptyShippingCartList = new List<ShoppingCart>();
                    shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(data);
                    if (shippingCartObjectVMs.Count != 0)
                    {

                        List<string> outOfStockItemNames = new List<string>();
                        foreach (var orderItem in shippingCartObjectVMs)
                        {
                            var item = perfumeContext.Item.Find(orderItem.ItemId);
                            if (item.Stock <= 0 || item.Stock == null || item.OutOfStock == true)
                            {
                                outOfStockItemNames.Add(item.ItemTitleEn);


                            }
                            if (item.Stock < orderItem.Qunatity)
                            {
                                //toastNotification.AddErrorToastMessage($"{item.ItemTitleEn} Quantity less than Stock Quantity");
                                //return Redirect("/CheckOutFastOrder");

                            }


                        }
                        if (outOfStockItemNames.Count > 0)
                        {
                            var result = new { outofStock = outofStock, outOfStockItemNames = outOfStockItemNames };
                            return new JsonResult(result);
                        }
                    }
                    var Country = HttpContext.Session.GetString("country");
                    shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();
                    var countryob = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;


                    if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                    {
                        if (Country == "2" && PhoneNumber.StartsWith("0"))
                        {
                            PhoneNumber = PhoneNumber.Substring(1);
                        }
                        if (FastOrderVM.PhoneNumber.StartsWith(countryCode))
                        {
                            // User entered phone number with country code, remove the duplicate country code
                            PhoneNumber = PhoneNumber.Substring(countryCode.Length);
                        }
                        PhoneNumber = countryCode + PhoneNumber;
                    }

                    var customer = new CustomerN()
                    {
                        CustomerName = FullName,
                        Email = Email,
                        Phone = PhoneNumber,
                        RegisterDate = DateTime.Now
                    };
                    perfumeContext.CustomerNs.Add(customer);
                    perfumeContext.SaveChanges();
                    var CustomerAddress = new CustomerAddress()
                    {
                        Address = Address,
                        AreaName = AreaName,
                        BuildingNo = BuildingNo,
                        CityName = CityName,
                        CustomerId = customer.CustomerId,
                        CountryId = int.Parse(Country),
                        Mobile = PhoneNumber,
                        ZIPCode = ZIP,
                    };
                    perfumeContext.customerAddresses.Add(CustomerAddress);
                    perfumeContext.SaveChanges();
                    var shippingCartObj = new ShoppingCart();
                    if (shippingCartObjectVMs != null)
                    {
                        if (shippingCartObjectVMs.Count != 0)
                        {
                            foreach (var item in shippingCartObjectVMs)
                            {

                                shippingCartObj = new ShoppingCart()
                                {
                                    CustomerId = customer.CustomerId,
                                    ItemId = item.ItemId,
                                    ItemPrice = item.priceCart,
                                    ItemQty = item.Qunatity,
                                    ItemTotal = item.priceCart * item.Qunatity
                                };
                                EmptyShippingCartList.Add(shippingCartObj);
                                perfumeContext.ShoppingCart.Add(shippingCartObj);
                                perfumeContext.SaveChanges();
                            }

                        }
                        var CountryObj = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault();
                        string currencyISO = perfumeContext.Currency.Where(i => i.CurrencyId == CountryObj.CurrencyId).FirstOrDefault().CurrencyTlen;
                        var customerShoppingCartList = perfumeContext.
                    ShoppingCart.Include(s => s.CustomerN)
                    .Include(s => s.Item)
                    .Where(c => c.CustomerId == customer.CustomerId);
                        double shoppingCost = 0.0;

                        shoppingCost = CountryObj.ShippingCost.Value;
                        taxvalue = CountryObj.tax.Value;
                        var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);
                        var totalorder = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);
                        coupon = perfumeContext.Coupon.Where(i => i.Serial == Serial).FirstOrDefault();

                        double discount = 0.0;
                        var OrdernetByTax = totalOfAll * taxvalue;
                        if (shippingCartObjectVMs != null)
                        {
                            foreach (var item in shippingCartObjectVMs)
                            {
                                if (item != null)
                                {

                                    if (item.Qunatity >= 1)
                                    {
                                        var productWeight = perfumeContext.Item.Where(a => a.ItemId == item.ItemId).FirstOrDefault().Weight;
                                        if (productWeight != null)
                                        {
                                            TotalWeight += (item.Qunatity * (productWeight ?? 0.0));

                                        }
                                    }


                                }

                            }
                        }
                        foreach (var shipCost in shippingMatrices)
                        {
                            if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                            {
                                DeliveryCost = shipCost.ActualPrice;
                            }

                        }
                        totalOfAll = totalOfAll + DeliveryCost + OrdernetByTax;

                        GetDiscountFromCoupon(totalOfAll, coupon);


                        int maxUniqe = 1;
                        var newList = perfumeContext.Order.ToList();
                        var maxserial = Convert.ToInt64(1);
                        perfumeContext.Order.ToList().Max(e => Convert.ToInt64(e.OrderSerial));
                        if (newList != null)
                        {
                            if (newList.Count > 0)
                            {
                                maxUniqe = newList.Max(e => e.UniqeId).Value;
                                maxserial = newList.Max(e => Convert.ToInt64(e.OrderSerial));
                            }
                        }
                        var TaxRateValue = CountryObj.tax;
                        var taxpercentage = CountryObj.tax * 100;
                        var orders =
                    new Order
                    {
                        OrderDate = DateTime.Now,
                        OrderSerial = Convert.ToString(maxserial + 1),
                        CustomerId = customer.CustomerId,
                        CustomerAddressId = CustomerAddress.CustomerAddressId,
                        OrderTotal = totalorder,
                        CouponId = coupon != null ? coupon.CouponId : null,
                        CouponTypeId = coupon != null ? coupon.CouponTypeId : null,
                        CouponAmount = coupon != null ? (float?)coupon.Amount : null,
                        Deliverycost = DeliveryCost,
                        OrderNet = TaxRateValue == 0 ? Math.Round(TotalAmountAfterDiscount, 2) : TotalAmountAfterDiscount /*Math.Round((double)(TotalAmountAfterDiscount), 2)*/,
                        TotalWeight = TotalWeight,
                        //OrderNet = (TotalAmountAfterDiscount + shoppingCost)* TaxRateValue,
                        //PaymentMethodId = PaymentId,
                        OrderDiscount = Discount,
                        IsCanceled = false,
                        TotalAfterDiscount = TotalAmountAfterDiscount,
                        OrderStatusId = 1,
                        CountryId = int.Parse(Country),
                        UniqeId = maxUniqe + 1,
                        IsDeliverd = false,
                        tax = TaxRateValue,
                        Notes = Notes
                    };


                        perfumeContext.Order.Add(orders);
                        perfumeContext.SaveChanges();


                        List<OrderItem> orderItems = new List<OrderItem>();

                        foreach (var itemObj in customerShoppingCartList)
                        {

                            OrderItem orderItem = new OrderItem
                            {
                                ItemId = itemObj.ItemId,
                                ItemPrice = itemObj.ItemPrice,
                                Total = itemObj.ItemTotal,
                                ItemQuantity = itemObj.ItemQty,
                                OrderId = orders.OrderId
                            };

                            perfumeContext.OrderItem.Add(orderItem);

                        }

                        var TrakingOrderObj = new OrderTraking()
                        {
                            OrderId = orders.OrderId,
                            OrderStatusId = 1,
                            ActionDate = DateTime.Now,
                            Remarks = "Order Initiated"
                        };

                        perfumeContext.OrderTrakings.Add(TrakingOrderObj);

                        perfumeContext.SaveChanges();
                        outofStock = true;
                        var result = new { outofStock = outofStock, order = orders.OrderId };
                        return new JsonResult(result);
                        //string serial = Serial != null ? "&Serial=" + Serial.ToString() : "";
                        //return Redirect("/CheckOutPayment?orderId=" + orders.OrderId + serial);

                        //return RedirectToPage("/CheckOutFastOrder");
                        //return Redirect("/CheckOutPayment?Serial=" + serialParameter + "&orderId=" + orders.OrderId);

                    }
                }


            }





            catch (Exception ex)
            {
                toastNotification.AddErrorToastMessage("Something went wrong");
                return RedirectToPage("/CheckOutFastOrder");
            }
            string serialParameter = Serial != null ? "&Serial=" + Serial.ToString() : "";
            return Redirect("/CheckOutPayment?orderId=" + orders.OrderId + serialParameter);


        }


        public async Task<IActionResult> OnPostAddCustomerAddressDatabase(string Serial, string PhoneNumber, string Address, string CityName, string AreaName, string ZIP, string BuildingNo, string Notes)
        {
            try
            {
                bool outofStock = false;
                var user = await userManager.GetUserAsync(User);
                if (user != null) /////////////////Database
                {
                    var Country = HttpContext.Session.GetString("country");
                    var customerObj = perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                    if (customerObj == null)
                    {
                        return NotFound();

                    }


                    var country = perfumeContext.Country.Where(c => c.CountryId == int.Parse(Country)).FirstOrDefault();
                    var countryId = country.CountryId;
                    var tax = country.tax;

                    var AddressCustomer = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();
                    if (AddressCustomer == null)
                    {
                        var countryob = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;


                        if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                        {
                            if (Country == "2" && PhoneNumber.StartsWith("0"))
                            {
                                PhoneNumber = PhoneNumber.Substring(1);
                            }
                            if (PhoneNumber.StartsWith(countryCode))
                            {
                                // User entered phone number with country code, remove the duplicate country code
                                PhoneNumber = PhoneNumber.Substring(countryCode.Length);
                            }
                            PhoneNumber = countryCode + PhoneNumber;
                        }
                        var customerAddress = new CustomerAddress()
                        {

                            CustomerId = customerObj.CustomerId,
                            Address = Address,
                            CountryId = countryId,
                            CityName = CityName,
                            AreaName = AreaName,
                            BuildingNo = BuildingNo,
                            Mobile = PhoneNumber,
                            ZIPCode = ZIP,

                        };


                        perfumeContext.customerAddresses.Add(customerAddress);
                        perfumeContext.SaveChanges();

                        toastNotification.AddSuccessToastMessage("Address Added Successfully");

                    }
                    else
                    {
                        var countryob = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().CountryTlen;
                        if (CountryCodeMappings.TryGetValue(countryob, out string countryCode))
                        {
                            if (Country == "2" && PhoneNumber.StartsWith("0"))
                            {
                                PhoneNumber = PhoneNumber.Substring(1);
                            }
                            if (PhoneNumber.StartsWith(countryCode))
                            {
                                // User entered phone number with country code, remove the duplicate country code
                                PhoneNumber = PhoneNumber.Substring(countryCode.Length);
                            }
                            PhoneNumber = countryCode + PhoneNumber;
                        }
                        AddressCustomer.Address = Address;
                        AddressCustomer.AreaName = AreaName;
                        AddressCustomer.CityName = CityName;
                        AddressCustomer.BuildingNo = BuildingNo;
                        AddressCustomer.CountryId = countryId;
                        AddressCustomer.Mobile = PhoneNumber;

                        AddressCustomer.ZIPCode = ZIP;
                        perfumeContext.Attach(AddressCustomer).State = EntityState.Modified;
                        perfumeContext.SaveChanges();
                    }


                    var existedAddressForCustomer = perfumeContext.customerAddresses.Where(i => i.CustomerId == customerObj.CustomerId).FirstOrDefault();

                    var CountryObj = perfumeContext.Country.Where(i => i.CountryId == countryId).FirstOrDefault();
                    string currencyISO = perfumeContext.Currency.Where(i => i.CurrencyId == CountryObj.CurrencyId).FirstOrDefault().CurrencyTlen;
                    shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();

                    var customerShoppingCartList = perfumeContext.
                     ShoppingCart.Include(s => s.CustomerN)
                     .Include(s => s.Item)
                     .Where(c => c.CustomerId == customerObj.CustomerId).ToList();
                    if (customerShoppingCartList.Count != 0)
                    {

                        List<string> outOfStockItemNames = new List<string>();
                        foreach (var orderItem in customerShoppingCartList)
                        {
                            var item = perfumeContext.Item.Find(orderItem.ItemId);
                            if (item.Stock <= 0 || item.Stock == null || item.OutOfStock == true)
                            {
                                outOfStockItemNames.Add(item.ItemTitleEn);


                            }
                            if (item.Stock < orderItem.ItemQty)
                            {
                                //toastNotification.AddErrorToastMessage($"{item.ItemTitleEn} Quantity less than Stock Quantity");
                                //return Redirect("/CheckOutFastOrder");

                            }


                        }
                        if (outOfStockItemNames.Count > 0)
                        {
                            var resulttock = new { outofStock = outofStock, outOfStockItemNames = outOfStockItemNames };
                            return new JsonResult(resulttock);
                        }


                    }

                    double shoppingCost = 0.0;

                    shoppingCost = CountryObj.ShippingCost.Value;

                    var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);
                    var ordertotal = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);
                    var OrdernetByTax = totalOfAll * tax;
                    foreach (var item in customerShoppingCartList)
                    {
                        if (item != null)
                        {
                            if (item.ItemQty >= 1)
                            {
                                TotalWeight += (item.ItemQty * (item.Item.Weight ?? 0.0));
                            }


                        }

                    }
                    foreach (var shipCost in shippingMatrices)
                    {
                        if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                        {
                            DeliveryCost = shipCost.ActualPrice;
                        }

                    }
                    totalOfAll = totalOfAll + DeliveryCost + OrdernetByTax.Value;
                    coupon = perfumeContext.Coupon.Where(i => i.Serial == Serial).FirstOrDefault();

                    double discount = 0.0;

                    GetDiscountFromCoupon(totalOfAll, coupon);


                    int maxUniqe = 1;
                    var newList = perfumeContext.Order.ToList();
                    var maxserial = Convert.ToInt64(1);
                    perfumeContext.Order.ToList().Max(e => Convert.ToInt64(e.OrderSerial));
                    if (newList != null)
                    {
                        if (newList.Count > 0)
                        {
                            maxUniqe = newList.Max(e => e.UniqeId).Value;
                            maxserial = newList.Max(e => Convert.ToInt64(e.OrderSerial));
                        }
                    }
                    orders =
               new Order
               {
                   OrderDate = DateTime.Now,
                   OrderSerial = Convert.ToString(maxserial + 1),
                   CustomerId = customerObj.CustomerId,
                   CustomerAddressId = existedAddressForCustomer.CustomerAddressId,
                   OrderTotal = ordertotal,
                   CouponId = coupon != null ? coupon.CouponId : null,
                   CouponTypeId = coupon != null ? coupon.CouponTypeId : null,
                   CouponAmount = coupon != null ? (float?)coupon.Amount : null,
                   Deliverycost = DeliveryCost,
                   TotalWeight = TotalWeight,
                   OrderNet = tax == 0 ? TotalAmountAfterDiscount : TotalAmountAfterDiscount,
                   //OrderNet = TotalAmountAfterDiscount + shoppingCost + CountryObj.tax,
                   PaymentMethodId = PaymentId,
                   OrderDiscount = Discount,
                   IsCanceled = false,
                   OrderStatusId = 1,
                   TotalAfterDiscount = TotalAmountAfterDiscount,
                   CountryId = countryId,
                   tax = tax,
                   UniqeId = maxUniqe + 1,
                   IsDeliverd = false,
                   Notes = Notes
               };


                    perfumeContext.Order.Add(orders);
                    perfumeContext.SaveChanges();


                    List<OrderItem> orderItems = new List<OrderItem>();

                    foreach (var itemObj in customerShoppingCartList)
                    {

                        OrderItem orderItem = new OrderItem
                        {
                            ItemId = itemObj.ItemId,
                            ItemPrice = itemObj.ItemPrice,
                            Total = itemObj.ItemTotal,
                            ItemQuantity = itemObj.ItemQty,
                            OrderId = orders.OrderId
                        };

                        perfumeContext.OrderItem.Add(orderItem);

                    }

                    var TrakingOrderObj = new OrderTraking()
                    {
                        OrderId = orders.OrderId,
                        OrderStatusId = 1,
                        ActionDate = DateTime.Now,
                        Remarks = "Order Initiated"
                    };

                    perfumeContext.OrderTrakings.Add(TrakingOrderObj);

                    perfumeContext.SaveChanges();
                    outofStock = true;
                    var result = new { outofStock = outofStock, order = orders.OrderId };
                    return new JsonResult(result);
                }


            }
            catch (Exception ex)
            {
                toastNotification.AddErrorToastMessage("Something went wrong");
                return RedirectToPage("/CheckOutFastOrder");
            }
            string serialParameter = Serial != null ? "&Serial=" + Serial.ToString() : "";
            return Redirect("/CheckOutPayment?orderId=" + orders.OrderId + serialParameter);
        }

        public void GetDiscountFromCoupon(double totalOfAll, Coupon coupon)
        {
            Discount = 0;
            double sumItemTotal = totalOfAll;
            var percent = sumItemTotal / totalOfAll;

            if (coupon == null)
            {
                Discount = 0;
                TotalAmountAfterDiscount = sumItemTotal;
            }
            else if (coupon.CouponTypeId == 2)
            {
                IsDiscounted = true;
                Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - coupon.Amount * percent), 2);

                var AmountAfterDiscount = (double)(sumItemTotal - coupon.Amount * percent);
                TotalAmountAfterDiscount = /*AmountAfterDiscount*/ AmountAfterDiscount;
            }
            else
            {
                IsDiscounted = true;
                var couponAmount = totalOfAll * (coupon.Amount / 100);
                Discount = Math.Round(sumItemTotal - (double)(sumItemTotal - couponAmount * percent), 2);
                var AmountAfterDiscount = (double)(sumItemTotal - couponAmount * percent);
                TotalAmountAfterDiscount = AmountAfterDiscount;
            }
            if (TotalAmountAfterDiscount < 0)
            {
                TotalAmountAfterDiscount = 0;
            }


        }

        public string GetUserIpAddress()
        {
            string Ip = Response.HttpContext.Connection.RemoteIpAddress.ToString();

            if (Ip == "::1")
            {
                Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            }
            return Ip;
        }


        public string GetUserCountryByIp(string IpAddress)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + IpAddress);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
                CurrencyNameEN = myRI1.ISOCurrencySymbol;
                CurrencyNameAr = myRI1.CurrencySymbol;


            }
            catch
            {
                CurrencyNameEN = "";
                CurrencyNameAr = "";
                ipInfo.Country = null;
            }

            return ipInfo.Country;
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
