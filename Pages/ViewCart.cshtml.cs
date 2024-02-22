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
using DevExpress.DataAccess.Native.Json;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using CRM.Migrations;
using CRM.Services.TabbyModels;
using System.Net.Http.Headers;
using System.Text;

namespace CRM.Pages
{
    public class ViewCartModel : PageModel
    {
        private readonly PerfumeContext perfumeContext;
        private readonly IToastNotification toastNotification;
        private readonly UserManager<ApplicationUser> userManager;
        public double tax { get; set; }
        public double taxvalue { get; set; }
        public string url { get; set; }

        [BindProperty]
        public List<ShoppingCart> shoppingCarts { get; set; }

        [BindProperty]
        public  string? CouponSerial { get; set; }

        public  string? newCouponSerial { get; set; }
        public double TotalAmount { get; set; }
        public double DeliveryCost { get; set; }

        public double TotalAmountAfterDiscount { get; set; }
        public CustomerN customer { get; set; }
        public double Discount { get; set; }
        bool IsInstallment = false;
        public HttpClient httpClient { get; set; }

        public string data { get; set; }
        public bool IsDiscounted { get; set; } = false;
        public List<ShippingCartObjectVM> shippingCartObjectVMs { get; set; }
        public ShippingCartObjectVM shippingCartObject { get; set; }
        public double totalitems { get; set; }
        public double SubTotal { get; set; }
        public string CurrencyEN { get; set; }
        public string CurrencyNameAr { get; set; }
        public double tabbyorderNet { get; set; }
        public double OrdernetByTax { get; set; }
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        public List<ShippingMatrix> shippingMatrices { get; set; }
        public double TotalWeight { get; set; }
        public ViewCartModel(PerfumeContext perfumeContext, IHttpContextAccessor HttpContextAccessor, IToastNotification toastNotification, UserManager<ApplicationUser> userManager)
        {
            this.perfumeContext = perfumeContext;
            this.toastNotification = toastNotification;
            this.userManager = userManager;
            this.HttpContextAccessor = HttpContextAccessor;
            shippingCartObjectVMs = new List<ShippingCartObjectVM>();
            shoppingCarts = new List<ShoppingCart>();
            httpClient = new HttpClient();
        }

        public async Task<IActionResult> OnGet(string CouponSerial)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                 data = HttpContext.Session.GetString("parsecartItems");
                if (data != null)
                {
                    var Country = HttpContext.Session.GetString("country");
                    if (Country != null)
                    {
                        tax = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value * 100;
                        taxvalue = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value;
                        CurrencyEN = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlen;
                        CurrencyNameAr = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlar;
                        DeliveryCost = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().ShippingCost.Value;
                        shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();
                    }
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
                        foreach (var shipCost in shippingMatrices)
                        {
                            if (TotalWeight == shipCost.ToWeight || TotalWeight == shipCost.FromWeight || (TotalWeight > shipCost.FromWeight && TotalWeight < shipCost.ToWeight) || (TotalWeight < shipCost.FromWeight && TotalWeight > shipCost.ToWeight))
                            {
                                DeliveryCost = shipCost.ActualPrice;
                            }
                            //else if(TotalWeight >= shipCost.FromWeight && TotalWeight <= shipCost.ToWeight)
                            //{
                            //    DeliveryCost = shipCost.ActualPrice;
                                
                            //}
                            //else
                            //{

                            //}

                        }

                        foreach (var item in shippingCartObjectVMs)
                        {
                            totalitems = totalitems + (item.priceCart * item.Qunatity);
                            TotalAmount = TotalAmount + (item.priceCart * item.Qunatity);

                        }
                        OrdernetByTax = (TotalAmount * taxvalue);
                        TotalAmount = totalitems + DeliveryCost + OrdernetByTax;
                        tabbyorderNet = TotalAmount;
                    }



                }
            }
           if(user!=null)
            {
                var customerId = perfumeContext.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;

                shoppingCarts = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item)
                                .Include(i => i.CustomerN).ToList();

                totalitems = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);
                var Country = HttpContext.Session.GetString("country");
                if (Country != null)
                {
                    tax = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value * 100;
                    taxvalue = perfumeContext.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().tax.Value;
                    shippingMatrices = perfumeContext.ShippingsMatrix.Where(e => e.CountryId == int.Parse(Country)).ToList();
                    DeliveryCost = perfumeContext.Country.Where(e=>e.CountryId ==int.Parse( Country)).FirstOrDefault().ShippingCost.Value;
                    foreach(var item in shoppingCarts)
                    {
                        if(item != null)
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
                    //TotalAmount = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);
                    //TotalAmount = TotalAmount + DeliveryCost+ tax;
                    SubTotal = perfumeContext.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);
                    CurrencyEN = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlen;
                    CurrencyNameAr = perfumeContext.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency.CurrencyTlar;
                    OrdernetByTax = (SubTotal * taxvalue) ;
                    TotalAmount = totalitems + DeliveryCost + OrdernetByTax;
                    tabbyorderNet = TotalAmount;
                }

            }


            newCouponSerial = CouponSerial;


            if (CouponSerial != null)
            {
                HttpContext.Session.SetString("newCouponSerial", newCouponSerial);
                HttpContext.Session.GetString("newCouponSerial");
            }





            //if (user == null)
            //{

            //    return Redirect("~/login");
            //}


            var coupon = await perfumeContext.Coupon.FirstOrDefaultAsync(c => c.Serial == CouponSerial);

            GetDiscountFromCoupon(TotalAmount, coupon);
            return Page();
        }
        public IActionResult OnPostRemoveFromsession(int ItemId)
        {
            var data = HttpContext.Session.GetString("parsecartItems");
            if (data != null)
            {

                //List<ShippingCartObjectVM> shippingCartObjectVMs = JsonSerializer.Deserialize<List<ShippingCartObjectVM>>(data);
                shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(data);
                var itemToRemove = shippingCartObjectVMs.FirstOrDefault(item => item.ItemId == ItemId);
                if (itemToRemove != null)
                {
                    shippingCartObjectVMs.Remove(itemToRemove);
                    var newCartString = JsonConvert.SerializeObject(shippingCartObjectVMs);

                   HttpContext.Session.SetString("parsecartItems", newCartString);
                    var session = newCartString;
                }
            }
               
           

            return new JsonResult("Item removed from cart in session.");
            //HttpContext.Session.Remove("parsecartItems");
            return new JsonResult(ItemId);
        }
        public IActionResult OnPostUpdateCart(List<ShoppingCart> shoppingCarts)
        {
            var _shoppingCarts = perfumeContext.ShoppingCart.Include(i => i.Item).Where(i => i.CustomerId == 5).ToList();
            for (int i = 1; i <= _shoppingCarts.Count(); i++)
            {
                _shoppingCarts[i].ItemQty = shoppingCarts[i].ItemQty;
            }
            perfumeContext.Attach(_shoppingCarts).State = EntityState.Modified;
            perfumeContext.SaveChanges();

            return Page();
        }
        public async Task<IActionResult> OnPostDeleteItemFromShoppingCart(int Id)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    toastNotification.AddErrorToastMessage("you have to login First");
                    return Redirect("~/login");
                }

                var Item = perfumeContext.ShoppingCart.Where(e => e.ItemId == Id).FirstOrDefault();

                if (Item == null)
                {
                    toastNotification.AddErrorToastMessage("Item Not Found");
                    return Page();

                }

                var UserId = perfumeContext.CustomerNs.Where(a => a.Email == user.Email)
                                                    .FirstOrDefault()
                                                    .CustomerId;

                var cartItem = perfumeContext.ShoppingCart
                                    .Where(e => e.ItemId == Id && e.CustomerId == UserId)
                                    .FirstOrDefault();

                if (cartItem == null)
                {
                    toastNotification.AddErrorToastMessage("Item Not Found in shopping cart");
                    return Page();
                }

                perfumeContext.ShoppingCart.Remove(cartItem);
                perfumeContext.SaveChanges();
                toastNotification.AddSuccessToastMessage("Item Removed Successfully From shopping cart");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                toastNotification.AddErrorToastMessage("Somthing Went Wrong");
                return Page();
            }
        }


        public async Task<IActionResult> OnPostValidateCopoun(string CouponSerial)
        {
            try
            {
                var coupon = await perfumeContext.Coupon.FirstOrDefaultAsync(c => c.Serial == CouponSerial);


                if (coupon == null)
                {
                    toastNotification.AddErrorToastMessage("Coupon Not Exist");
                    return RedirectToPage();
                }
                if (
                    (DateTime.Now.Date >= coupon.IssueDate.Date) &&
                    (DateTime.Now.Date <= coupon.ExpirationDate.Date) &&
                    (coupon.Used != true))
                {
                    toastNotification.AddSuccessToastMessage("Coupon applied");

                    return RedirectToPage("", new { CouponSerial });
                }
                else
                {
                    toastNotification.AddErrorToastMessage("Coupon is no valid");
                    return RedirectToPage("/ViewCart");
                }
            }
            catch (Exception ex)
            {
                toastNotification.AddErrorToastMessage(ex.Message);
                return RedirectToPage("/ViewCart");
            }


        }
        public async Task<IActionResult> OnPostEditItemQuantity([FromBody] List<ItemQuantityVm> ItemQuantityVmList)
        {
            var status = false;
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var UserId = perfumeContext.CustomerNs.Where(a => a.Email == user.Email)
                                                   .FirstOrDefault()
                                                   .CustomerId;
                if (ItemQuantityVmList != null)
                {
                    foreach (var item in ItemQuantityVmList)
                    {
                        var itemObj = perfumeContext.ShoppingCart
                                    .Where(e => e.ItemId == item.ItemId && e.CustomerId == UserId)
                                    .FirstOrDefault();
                        itemObj.ItemQty = item.ItemQty;

                        itemObj.ItemTotal = item.ItemQty * itemObj.ItemPrice;
                        perfumeContext.Attach(itemObj).State = EntityState.Modified;
                        perfumeContext.SaveChanges();
                        status = true;
                    }

                    return new JsonResult(status);
                }

            }




            return new JsonResult(status);
        }

       
        public void GetDiscountFromCoupon(double totalOfAll, Coupon coupon)
        {
            Discount = 0;
            double sumItemTotal = totalOfAll;

            var percent = sumItemTotal / totalOfAll;

            if (totalOfAll <= 0)
            {
                percent = 0;
            }
            

            if (coupon == null)
            {
                Discount = 0;
                TotalAmountAfterDiscount = sumItemTotal;
            }
            else if (coupon.CouponTypeId == 2)
            {
                IsDiscounted = true;
                Discount =Math.Round(sumItemTotal - (double)(sumItemTotal - coupon.Amount * percent),2);

                var AmountAfterDiscount = (double)(sumItemTotal - coupon.Amount * percent);
                TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
                tabbyorderNet=TotalAmountAfterDiscount;
            }
            else
            {
                IsDiscounted = true;
                var couponAmount = totalOfAll * (coupon.Amount / 100);
                Discount =Math.Round(sumItemTotal - (double)(sumItemTotal - couponAmount * percent),2);
                var AmountAfterDiscount  = (double)(sumItemTotal - couponAmount * percent);
                TotalAmountAfterDiscount = Math.Round(AmountAfterDiscount, 2);
                tabbyorderNet = TotalAmountAfterDiscount;
            }
            if (TotalAmountAfterDiscount < 0)
            {
                TotalAmountAfterDiscount = 0;
            }


        }



        public async Task<IActionResult> OnPostTabbyCheckOut(double tabbyorderNet, string Country)
        {


            var testtoken = "sk_test_a212d9c5-4c21-4a64-8c96-bfab29894c19";
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            string updatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var country = perfumeContext.Country.FirstOrDefault(c => c.CountryId == int.Parse(Country));
            var currencyEN = perfumeContext.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlen;
            var currencyAR = perfumeContext.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlar;




            if (int.Parse(Country) == 2)
            {

                var sendPaymentRequest = new
                {
                    payment = new
                    {
                        amount = Math.Round((double)tabbyorderNet, 2),
                        currency = "SAR",
                        description = "string",

                        //buyer_history = new
                        //{
                        //    registered_since = orders.OrderDate,
                        //    loyalty_level = 0,
                        //    wishlist_count = 0,
                        //    is_social_networks_connected = true,
                        //    is_phone_number_verified = true,
                        //    is_email_verified = true
                        //},

                        //                            order_history = new[]
                        //{
                        //    new
                        //    {
                        //        purchased_at = "2019-08-24T14:15:22Z",
                        //        amount = "0.00",
                        //        payment_method = "card",
                        //        status = "new",
                        //        buyer = new
                        //        {
                        //            phone = "string",
                        //            email = "user@example.com",
                        //            name = "string",
                        //            dob = "2019-08-24"
                        //        },
                        //        shipping_address = new
                        //        {
                        //            city = "string",
                        //            address = "string",
                        //            zip = "string"
                        //        },
                        //        items = new[]
                        //        {
                        //            new
                        //            {
                        //                title = "string",
                        //                description = "string",
                        //                quantity = 1,
                        //                unit_price = "0.00",
                        //                discount_amount = "0.00",
                        //                reference_id = "string",
                        //                image_url = "http://example.com",
                        //                product_url = "http://example.com",
                        //                ordered = 0,
                        //                captured = 0,
                        //                shipped = 0,
                        //                refunded = 0,
                        //                gender = "Male",
                        //                category = "string",
                        //                color = "string",
                        //                product_material = "string",
                        //                size_type = "string",
                        //                size = "string",
                        //                brand = "string"
                        //            }
                        //        }
                        //    }
                        //},


                        //attachment = new
                        //{
                        //    body = "{\"flight_reservation_details\": {\"pnr\": \"TR9088999\",\"itinerary\": [...],\"insurance\": [...],\"passengers\": [...],\"affiliate_name\": \"some affiliate\"}}",
                        //    content_type = "application/vnd.tabby.v1+json"
                        //}
                    },
                    lang = "en",
                    merchant_code = "MKSA",
                    merchant_urls = new
                    {
                        success = "https://mashaer.store/TabbySuccess",
                        cancel = "https://mashaer.store/TabbyFailed",
                        failure = "https://mashaer.store/TabbyFailed"
                    },
                    create_token = false
                };

                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                string url = "https://api.tabby.ai/api/v2/checkout";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                var responseMessage = httpClient.PostAsync(url, httpContent);
                var res = await responseMessage.Result.Content.ReadAsStringAsync();

                var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);

                if (tabbyRes != null)
                {


                    if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                    {
                        if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                        {
                            var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            IsInstallment = true;
                            var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //List<Installment> installments = installmentInfo.Installments;
                            return new JsonResult(result);
                        }

                    }
                    else if (tabbyRes.status == "rejected" || tabbyRes.status == "expired")
                    {
                        var rejectionReason = tabbyRes.Configuration.Products.Installments.rejection_reason;

                        var result = new { rejectionReason, IsInstallment };
                        //List<Installment> installments = installmentInfo.Installments;
                        return new JsonResult(result);
                    }



                }
            }
            else if (int.Parse(Country) == 4) ////UAE 
            {
                var sendPaymentRequest = new
                {
                    payment = new
                    {
                        amount = Math.Round((double)tabbyorderNet, 2),
                        currency = "AED",
                        description = "Perfume",

                        //buyer_history = new
                        //{
                        //    registered_since = orders.OrderDate,
                        //    loyalty_level = 0,
                        //    wishlist_count = 0,
                        //    is_social_networks_connected = true,
                        //    is_phone_number_verified = true,
                        //    is_email_verified = true
                        //},

                        //                            order_history = new[]
                        //{
                        //    new
                        //    {
                        //        purchased_at = "2019-08-24T14:15:22Z",
                        //        amount = "0.00",
                        //        payment_method = "card",
                        //        status = "new",
                        //        buyer = new
                        //        {
                        //            phone = "string",
                        //            email = "user@example.com",
                        //            name = "string",
                        //            dob = "2019-08-24"
                        //        },
                        //        shipping_address = new
                        //        {
                        //            city = "string",
                        //            address = "string",
                        //            zip = "string"
                        //        },
                        //        items = new[]
                        //        {
                        //            new
                        //            {
                        //                title = "string",
                        //                description = "string",
                        //                quantity = 1,
                        //                unit_price = "0.00",
                        //                discount_amount = "0.00",
                        //                reference_id = "string",
                        //                image_url = "http://example.com",
                        //                product_url = "http://example.com",
                        //                ordered = 0,
                        //                captured = 0,
                        //                shipped = 0,
                        //                refunded = 0,
                        //                gender = "Male",
                        //                category = "string",
                        //                color = "string",
                        //                product_material = "string",
                        //                size_type = "string",
                        //                size = "string",
                        //                brand = "string"
                        //            }
                        //        }
                        //    }
                        //},

                        //attachment = new
                        //{
                        //    body = "{\"flight_reservation_details\": {\"pnr\": \"TR9088999\",\"itinerary\": [...],\"insurance\": [...],\"passengers\": [...],\"affiliate_name\": \"some affiliate\"}}",
                        //    content_type = "application/vnd.tabby.v1+json"
                        //}
                    },
                    lang = "en",

                    merchant_code = "MUAE",
                    merchant_urls = new
                    {
                        success = "https://mashaer.store/TabbySuccess",
                        cancel = "https://mashaer.store/TabbyFailed",
                        failure = "https://mashaer.store/TabbyFailed"
                    },
                    create_token = false
                };

                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                string url = "https://api.tabby.ai/api/v2/checkout";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                var responseMessage = httpClient.PostAsync(url, httpContent);
                var res = await responseMessage.Result.Content.ReadAsStringAsync();

                var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);
                if (tabbyRes != null)
                {
                    if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                    {
                        if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                        {
                            var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            IsInstallment = true;
                            var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //List<Installment> installments = installmentInfo.Installments;
                            return new JsonResult(result);
                        }

                    }
                    else if (tabbyRes.status == "rejected" || tabbyRes.status == "expired")
                    {
                        var rejectionReason = tabbyRes.Configuration.Products.Installments.rejection_reason;

                        var result = new { rejectionReason, IsInstallment };
                        //List<Installment> installments = installmentInfo.Installments;
                        return new JsonResult(result);
                    }



                }
            }
            else if (int.Parse(Country) == 1)
            {
                var sendPaymentRequest = new
                {
                    payment = new
                    {
                        amount = Math.Round((double)tabbyorderNet, 2),
                        currency = "KWD",
                        description = "Perfume",

                        //buyer_history = new
                        //{
                        //    registered_since = orders.OrderDate,
                        //    loyalty_level = 0,
                        //    wishlist_count = 0,
                        //    is_social_networks_connected = true,
                        //    is_phone_number_verified = true,
                        //    is_email_verified = true
                        //},

                        //                            order_history = new[]
                        //{
                        //    new
                        //    {
                        //        purchased_at = "2019-08-24T14:15:22Z",
                        //        amount = "0.00",
                        //        payment_method = "card",
                        //        status = "new",
                        //        buyer = new
                        //        {
                        //            phone = "string",
                        //            email = "user@example.com",
                        //            name = "string",
                        //            dob = "2019-08-24"
                        //        },
                        //        shipping_address = new
                        //        {
                        //            city = "string",
                        //            address = "string",
                        //            zip = "string"
                        //        },
                        //        items = new[]
                        //        {
                        //            new
                        //            {
                        //                title = "string",
                        //                description = "string",
                        //                quantity = 1,
                        //                unit_price = "0.00",
                        //                discount_amount = "0.00",
                        //                reference_id = "string",
                        //                image_url = "http://example.com",
                        //                product_url = "http://example.com",
                        //                ordered = 0,
                        //                captured = 0,
                        //                shipped = 0,
                        //                refunded = 0,
                        //                gender = "Male",
                        //                category = "string",
                        //                color = "string",
                        //                product_material = "string",
                        //                size_type = "string",
                        //                size = "string",
                        //                brand = "string"
                        //            }
                        //        }
                        //    }
                        //},

                        //attachment = new
                        //{
                        //    body = "{\"flight_reservation_details\": {\"pnr\": \"TR9088999\",\"itinerary\": [...],\"insurance\": [...],\"passengers\": [...],\"affiliate_name\": \"some affiliate\"}}",
                        //    content_type = "application/vnd.tabby.v1+json"
                        //}
                    },
                    lang = "en",

                    merchant_code = "MKWT",
                    merchant_urls = new
                    {
                        success = "https://mashaer.store/TabbySuccess",
                        cancel = "https://mashaer.store/TabbyFailed",
                        failure = "https://mashaer.store/TabbyFailed"
                    },
                    create_token = false
                };

                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                string url = "https://api.tabby.ai/api/v2/checkout";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testtoken);
                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                var responseMessage = httpClient.PostAsync(url, httpContent);
                var res = await responseMessage.Result.Content.ReadAsStringAsync();

                var tabbyRes = JsonConvert.DeserializeObject<TabbyResponse>(res);
                if (tabbyRes != null)
                {
                    if (tabbyRes.status == "created" || tabbyRes.status == "approved")
                    {
                        if (tabbyRes.Configuration?.available_products != null && tabbyRes.Configuration.available_products.Installments.Count > 0)
                        {
                            var Installment = tabbyRes.Configuration.available_products.Installments[0].downpayment;
                            var downpayment_total = tabbyRes.Configuration.available_products.Installments[0].downpayment_total;
                            var InstallmentList = tabbyRes.Configuration.available_products.Installments[0].installments;
                            IsInstallment = true;
                            var result = new { InstallmentList, Installment, IsInstallment, currencyEN, currencyAR, downpayment_total };
                            //List<Installment> installments = installmentInfo.Installments;
                            return new JsonResult(result);
                        }

                    }
                    else if ((tabbyRes.status == "rejected" || tabbyRes.status == "expired") && tabbyRes.Configuration.Products.Installments.rejection_reason != null)
                    {
                        var rejectionReason = tabbyRes.Configuration.Products.Installments.rejection_reason;

                        var result = new { rejectionReason, IsInstallment };
                        //List<Installment> installments = installmentInfo.Installments;
                        return new JsonResult(result);
                    }



                }
            }

            return new JsonResult(new { success = true, message = "TabbyCheckOut successful" });
        }

    }
}
