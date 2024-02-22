using CRM.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRM.ViewModels;
using NToastNotify;
using CRM.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using DevExpress.XtraRichEdit.Import.Html;

namespace CRM.Pages
{
    public class WISHLISTModel : PageModel
    {
        private readonly PerfumeContext _perfumeContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;


        public List<WishlistVm> wishlistVms = new List<WishlistVm>();

        public string CurrencyNameEN { get; set; }
        public string CurrencyNameAr { get; set; }

        public WISHLISTModel(PerfumeContext perfumeContext, IToastNotification toastNotification, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _perfumeContext = perfumeContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _toastNotification = toastNotification;

        }
        public async Task<IActionResult> OnGet()
        {

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Redirect("~/login");
                }

                var customer = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email).FirstOrDefault();

                if (customer == null)
                {
                    return Redirect("~/login");
                }

                var UserIpAddress = GetUserIpAddress();

                var country = GetUserCountryByIp(UserIpAddress);

                string CountryToUse;

                if (country != null)
                {
                    var DbCountry = _perfumeContext.Country.Any(i => i.CountryTlen == country);

                    CountryToUse = country;

                    if (!DbCountry)
                    {
                        var firstCountry = _perfumeContext.Country.FirstOrDefault().CountryTlen;
                        CountryToUse = firstCountry;
                    }


                }
                else
                {
                    var firstCountry = _perfumeContext.Country.FirstOrDefault().CountryTlen;
                    CountryToUse = firstCountry;
                }

                var CountryIdByIPAddress = _perfumeContext.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().CountryId;

                var dbCurrencyId = _perfumeContext.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().CurrencyId;
                var CurrencyName = _perfumeContext.Currency.Where(i => i.CurrencyId == dbCurrencyId).FirstOrDefault();
                CurrencyNameEN = CurrencyName.CurrencyTlen;
                CurrencyNameAr = CurrencyName.CurrencyTlar;

                var DbUserFavouriteItems = await _perfumeContext.FavouriteItems.AnyAsync(a => a.UserId == user.Id);

                if (DbUserFavouriteItems)
                {
                    wishlistVms = await _perfumeContext.FavouriteItems.Where(a => a.UserId == user.Id).Include(a => a.Item)

                                       .Select(a => new WishlistVm
                                       {
                                           FavouriteItemId = a.FavouriteItemId,
                                           ItemId = a.ItemId,
                                           ItemImage = a.Item.ItemImage,
                                           ItemTitleAr = a.Item.ItemTitleAr,
                                           ItemTitleEn = a.Item.ItemTitleEn,
                                           OutOfStock = a.Item.OutOfStock,
                                           Stock= a.Item.Stock.Value,
                                           Price = _perfumeContext.itemPriceNs
                                                .Where(e => e.ItemId == a.ItemId && e.CountryId == CountryIdByIPAddress)
                                                .FirstOrDefault().Price.Value
                                       }).ToListAsync();

                }
                else
                {
                    _toastNotification.AddInfoToastMessage("No Items In Your WishList");
                }

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again");

            }

            return Page();
        }


        public async Task<IActionResult> OnGetAddItemToCart(int ItemId)
        {
            var status = false;
            var UserIpAddress = GetUserIpAddress();

            var country = GetUserCountryByIp(UserIpAddress);

            string CountryToUse;

            if (country != null)
            {
                var DbCountry = _perfumeContext.Country.Any(i => i.CountryTlen == country);

                CountryToUse = country;

                if (!DbCountry)
                {
                    var firstCountry = _perfumeContext.Country.FirstOrDefault().CountryTlen;
                    CountryToUse = firstCountry;
                }


            }
            else
            {
                var firstCountry = _perfumeContext.Country.FirstOrDefault().CountryTlen;
                CountryToUse = firstCountry;
            }

            try
            {

                var user = await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    return new JsonResult(status);
                }

                status = true;

                var UserId = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email)
                                            .FirstOrDefault().CustomerId;

                var DbUserCart = await _perfumeContext.ShoppingCart.AnyAsync(a => a.ItemId == ItemId
                                         && a.CustomerId == UserId);

                if (!DbUserCart)
                {

                    var Item = _perfumeContext.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();



                    double ItemPrice = GetItemPrice(ItemId, CountryToUse);

                    int Quantity = 1;
                    var CartItem = new ShoppingCart()
                    {
                        CustomerId = UserId,
                        ItemId = ItemId,
                        ItemPrice = ItemPrice,
                        ItemQty = Quantity,
                        ItemTotal = Quantity * ItemPrice,

                    };


                    _perfumeContext.ShoppingCart.Add(CartItem);
                    _perfumeContext.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Item added to cart");
                }
                else
                {
                    _toastNotification.AddSuccessToastMessage("Item exists in cart");
                }



                return new JsonResult(status);
            }
            catch (Exception ex)
            {
                status = false;
                return new JsonResult(status);
            }
        }



        private double GetItemPrice(int ItemId, string Country)
        {
            var DbcountryId = _perfumeContext.Country.Where(c => c.CountryTlen == Country).FirstOrDefault().CountryId;

            var DbItemId = _perfumeContext.itemPriceNs.FirstOrDefault(a => a.ItemId == ItemId && a.CountryId == DbcountryId);

            if (DbItemId is null)
            {
                return 0;
            }
            double? DbItemPrice = DbItemId.Price;

            return (double)DbItemPrice;
        }


        public IActionResult OnGetDeleteItemFromFav(int favouriteId)
        {
            var status = false;
            var favObj = _perfumeContext.FavouriteItems.Where(c => c.FavouriteItemId == favouriteId).FirstOrDefault();

            if (favObj != null)
            {
                _perfumeContext.FavouriteItems.Remove(favObj);
                _perfumeContext.SaveChanges();
                status = true;
                _toastNotification.AddSuccessToastMessage("Item removed form favourite");
            }
            return new JsonResult(status);
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


        public static string GetUserCountryByIp(string IpAddress)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + IpAddress);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }
    }
}
