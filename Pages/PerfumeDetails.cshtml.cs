using CRM.Data;
using CRM.Models;
using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NToastNotify;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace CRM.Pages
{
    public class PerfumeDetailsModel : PageModel
    {
        private readonly PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<ApplicationUser> _userManager;
        public HttpClient httpClient { get; set; }
        public Item item { get; set; }
        public double itemPriceN { get; set; }
        public int ItemID { get; set; }
        public ItemPriceN itemPrice {get; set;}
        //[BindProperty]
        //public ShoppingCart shoppingCart { get; set; }
        [BindProperty]
        public bool IsFavorite { get; set; } /*= true;*/
        [BindProperty]
        public int dbcountryId { get; set; }
		[BindProperty]

		public  string CurrencyNameAr { get; set; }
        bool IsInstallment = false;
        [BindProperty]

		public  string CurrencyNameEN { get; set; }

        [BindProperty]
        public int itemQuantity { get; set; } = 0;
		public PerfumeDetailsModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;
            httpClient = new HttpClient();
            item = new Item();
        }
        public async Task<IActionResult> OnGet(int Id)
        {
            try
            {
                ItemID = Id;
                var Country = HttpContext.Session.GetString("country");
                if (Country!=null)
                {
                    itemPriceN = _context.itemPriceNs.Where(i => i.ItemId == Id && i.CountryId ==int.Parse(Country)).FirstOrDefault().Price.Value;
                    var CurrencyName = _context.Country.Include(e=>e.Currency).Where(i => i.CountryId == int.Parse(Country)).FirstOrDefault().Currency;
                    CurrencyNameEN = CurrencyName.CurrencyTlen;
                    CurrencyNameAr = CurrencyName.CurrencyTlar;
                }
                string userId = null;

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var CustomerId = _context.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;

                    var hasShopingCart = _context.ShoppingCart.Any(s => s.ItemId == Id && s.CustomerId == CustomerId);

                    if (hasShopingCart)
                    {
                        itemQuantity = _context.ShoppingCart.Where(i => i.ItemId == Id && i.CustomerId == CustomerId).FirstOrDefault().ItemQty;
                    }
                    else
                    {
                        itemQuantity = 1;
                    }
                    userId = user.Id;
                }
                else
                {
                    itemQuantity = 1;
                }

               
                //var UserIpAddress = GetUserIpAddress();

                //var country = GetUserCountryByIp(UserIpAddress);

                //string CountryToUse;

                //if (country != null)
                //{
                //    var DbCountry = _context.Country.Any(i => i.CountryTlen == country);
                //    CountryToUse = country;
                //    if (!DbCountry)
                //    {
                //        var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
                //        CountryToUse = firstCountry;

                //    }


                //}
                //else
                //{
                //    var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
                //    CountryToUse = firstCountry;

                //}

                //var dbCurrencyId = _context.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().CurrencyId;

                


                //dbcountryId = _context.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().CountryId;

                item = _context.Item.Where(i => i.ItemId == Id).Include(i => i.Category).FirstOrDefault();

                if (item is null)
                {
                    return RedirectToPage("/index");
                }

                IsFavorite = _context.FavouriteItems.Any(favorite => favorite.ItemId == item.ItemId
                                                           && favorite.UserId == userId);



            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return RedirectToPage("/index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCart(int ItemId, double priceCart, string Country)
        {
            var items = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();
            var CountryShoppingCost = _context.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().ShippingCost.Value;
            var result = new { items, CountryShoppingCost };
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetDetailsItem(string country, int id)
        {
            if (country != null)
            {
                itemPriceN = _context.itemPriceNs.Where(i => i.ItemId == id && i.CountryId == int.Parse(country)).FirstOrDefault().Price.Value;
                var CurrencyName = _context.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(country)).FirstOrDefault().Currency;
                CurrencyNameEN = CurrencyName.CurrencyTlen;
                CurrencyNameAr = CurrencyName.CurrencyTlar;
                

            }
            var result = new { itemPriceN, CurrencyNameEN, CurrencyNameAr };

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPost(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Redirect("~/login");
                }
                var UserId = _context.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;

                var DbUserCart = await _context.ShoppingCart.AnyAsync(a => a.ItemId == id
                                              && a.CustomerId == UserId);
                if (DbUserCart)
                {
                    var UserItem = await _context.ShoppingCart
                                            .FirstOrDefaultAsync(a => a.ItemId == id && a.CustomerId == UserId);

                    UserItem.ItemQty = itemQuantity;

                    UserItem.ItemTotal = itemQuantity * UserItem.ItemPrice;

                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Quantity Updated successfully");

                }
                else
                {
                    var Item = _context.Item.Where(e => e.ItemId == id).FirstOrDefault();

                    if (Item is null)
                    {
                        _toastNotification.AddErrorToastMessage("Item Not Found");
                    }

                    GetItemPrice(id);


                    var CartItem = new ShoppingCart()
                    {
                        CustomerId = UserId,
                        ItemId = id,
                        ItemPrice = itemPrice.Price.Value,
                        ItemQty = itemQuantity,
                        ItemTotal = itemQuantity * itemPrice.Price.Value,

                    };
                    _context.ShoppingCart.Add(CartItem);
                    _context.SaveChanges();

                    _toastNotification.AddSuccessToastMessage("Item Added Successfully");
                }

                return Redirect("/PerfumeDetails?id=" + id);
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
                return Redirect("/PerfumeDetails?id=" + id);
            }
        }

        public void GetItemPrice(int Id)
        {
            var Country = HttpContext.Session.GetString("country");

            //var UserIpAddress = GetUserIpAddress();

            //var country = GetUserCountryByIp(UserIpAddress);

            string CountryToUse;

            if (Country != null)
            {
                dbcountryId = _context.Country.Where(c => c.CountryId == int.Parse(Country)).FirstOrDefault().CountryId;

                itemPrice = _context.itemPriceNs.Where(i => i.ItemId == Id && i.CountryId == dbcountryId).FirstOrDefault();

                //var DbCountry = _context.Country.Any(i => i.CountryTlen == country);
                //CountryToUse = country;
                //if (!DbCountry)
                //{
                //    var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
                //    CountryToUse = firstCountry;

                //}
            }
            //else
            //{
            //    var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
            //    CountryToUse = firstCountry;

            //}

           

        }

        public async Task<IActionResult> OnPostAddToFavorite(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Redirect("~/login");
                }
                var Item = _context.Item.Where(e => e.ItemId == id).FirstOrDefault();

                if (Item is null)
                {
                    _toastNotification.AddErrorToastMessage("Item Not Found");
                    IsFavorite = false;
                }
                var IsExists = _context.FavouriteItems
                             .Any(favorite => favorite.ItemId == id
                                     && favorite.UserId == user.Id);
                var favItem = _context.FavouriteItems
                                           .Where(e => e.ItemId == id && e.UserId == user.Id)
                                           .FirstOrDefault();
                if (IsExists)
                {
                    _context.Remove(favItem);
                    _context.SaveChanges();
                    IsFavorite = false;
                }
                else
                {
                    var favouriteItem = new FavouriteItem()
                    {
                        ItemId = id,
                        UserId = user.Id
                    };

                    _context.Add(favouriteItem);
                    _context.SaveChanges();

                    IsFavorite = true;

                    _toastNotification.AddSuccessToastMessage("Item Added Successfully");
                }
                return Redirect("/PerfumeDetails?id=" + id);
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
                return Redirect("/PerfumeDetails?id=" + id);
            }
           
        }

        public async Task<IActionResult> OnPostTabbyCheckOut(int ItemID,string Country)
        {
           
            var item = _context.itemPriceNs.Where(e => e.ItemId == ItemID && e.CountryId == int.Parse(Country)).FirstOrDefault();

            var testtoken = "sk_test_a212d9c5-4c21-4a64-8c96-bfab29894c19";
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            string updatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var country = _context.Country.FirstOrDefault(c => c.CountryId == int.Parse(Country));
            var currencyEN = _context.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlen;
            var currencyAR = _context.Currency.FirstOrDefault(c => c.CurrencyId == country.CurrencyId).CurrencyTlar;

         


            if (int.Parse(Country) == 2)
            {

                var sendPaymentRequest = new
                {
                    payment = new
                    {
                        amount = Math.Round((double)item.Price, 2),
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
                        amount = Math.Round((double)item.Price, 2),
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
                        amount = Math.Round((double)item.Price, 2),
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

        //public string GetUserIpAddress()
        //{
        //    string Ip = Response.HttpContext.Connection.RemoteIpAddress.ToString();

        //    if (Ip == "::1")
        //    {
        //        Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        //    }
        //    return Ip;
        //}


        //    public string GetUserCountryByIp(string IpAddress)
        //    {
        //        IpInfo ipInfo = new IpInfo();
        //        try
        //        {
        //            string info = new WebClient().DownloadString("http://ipinfo.io/" + IpAddress);
        //            ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
        //            RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
        //            ipInfo.Country = myRI1.EnglishName;
        //CurrencyNameEN = myRI1.ISOCurrencySymbol;
        //CurrencyNameAr = myRI1.CurrencySymbol;


        //        }
        //        catch
        //        {
        //CurrencyNameEN ="";
        //CurrencyNameAr = "";
        //ipInfo.Country = null;
        //        }

        //        return ipInfo.Country;
        //    }
    }
}
