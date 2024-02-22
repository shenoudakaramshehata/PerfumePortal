using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NToastNotify;
using NuGet.ContentModel;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace CRM.Pages
{
    public class HomeIndexModel : PageModel
    {
        private readonly PerfumeContext _context;

        private readonly IToastNotification _toastNotification;

        private readonly UserManager<ApplicationUser> _userManager;

        public string Country { get; set; }
        public string ProductUrl { get; set; }

        //public List<Category> categoriesList { get; set; }
        public List<Item> ItemsList { get; set; }
        [BindProperty]
        public List<ShoppingCart> shoppingCarts { get; set; }
        public double TotalAmount { get; set; }
        //public int FirstCatId { get; set; }
        //public int FirstCounId { get; set; }
        //public string LanguageEn_Ar { get; set; }

        public int CountryIdByIPAddress { get; set; }
        public string CurrencyNameEN { get; set; }
        public string CurrencyNameAr { get; set; }
        [BindProperty]
        public ItemSearchVM itemSearchVM { get; set; }

        [BindProperty]
        public string Email { get; set; }

        public List<ItemPriceN> itemPriceByCountryId { get; set; }
        public HomeIndexModel(PerfumeContext perfumeContext, IToastNotification toastNotification
                                , UserManager<ApplicationUser> userManager)
        {
            _context = perfumeContext;
            _toastNotification = toastNotification;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var countrySession = HttpContext.Session.GetString("Country");

            //ItemsList = _context.Item.Where(c => c.CategoryId == 1).ToList();

            //categoriesList = _context.Categories.ToList();
            //var user = await _userManager.GetUserAsync(User);

            shoppingCarts = _context.ShoppingCart.Include(i => i.Item)
                           .Include(i => i.CustomerN).ToList();


            TotalAmount = _context.ShoppingCart.Sum(i => i.ItemTotal);
            ProductUrl = $"{this.Request.Scheme}://{this.Request.Host}";

            var UserIpAddress = GetUserIpAddress();

            var country = GetUserCountryByIp(UserIpAddress);


            GetCurrencyName(country);

            CheckItemPrice();

            ItemsList = _context.Item.Where(c => c.CategoryId == 1 && c.IsActive == true).ToList();
            return Page();

        }

        public void CheckItemPrice()
        {
            var ItemsLists = _context.Item.Where(c => c.CategoryId == 1 && c.IsActive == true).ToList();
            try
            {
                for (int i = 0; i < ItemsLists.Count; i++)
                {
                    var itemid = ItemsLists[i].ItemId;
                    var ItemPrice = _context.itemPriceNs.Any(I => I.CountryId == CountryIdByIPAddress && I.ItemId == itemid);
                    if (!ItemPrice)
                    {
                        var NewItemPrice = new ItemPriceN
                        {
                            ItemId = itemid,
                            CountryId = CountryIdByIPAddress,
                            Price = 0,
                            ShippingPrice = 0
                        };
                        _context.itemPriceNs.Add(NewItemPrice);
                        _context.SaveChanges();

                    }
                }
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went Wrong");

            }

        }

        public void GetCurrencyName(string country)
        {
            string CountryToUse;

            if (country != null)
            {
                var DbCountry = _context.Country.Any(i => i.CountryTlen == country);

                CountryToUse = country;

                if (!DbCountry)
                {
                    var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
                    CountryToUse = firstCountry;

                }

            }
            else
            {
                var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
                CountryToUse = firstCountry;

            }

            var dbCurrencyId = _context.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().CurrencyId;
            var CurrencyName = _context.Currency.Where(i => i.CurrencyId == dbCurrencyId).FirstOrDefault();
            CurrencyNameEN = CurrencyName.CurrencyTlen;
            CurrencyNameAr = CurrencyName.CurrencyTlar;

            CountryIdByIPAddress = _context.Country.Where(c => c.CountryTlen == CountryToUse).FirstOrDefault().CountryId;
        }

        //public ActionResult OnGetSingleCategory()
        //{
        //    var result = _context.Categories.ToList();

        //    return new JsonResult(result);
        //}
        //public async Task<IActionResult> OnPost(ItemSearchVM itemSearchVM)
        //{    
        //    bool CheckSearchItem = false;
        //    int SearchItem = 0;
        //    CheckSearchItem = int.TryParse(itemSearchVM.SearchItem, out SearchItem);



        //        List<Item> ListOfAssets = _context.Item
        //            .Where(x => x.CategoryId == 1 && ( x.ItemTitleAr == itemSearchVM.SearchItem || x.ItemTitleEn == itemSearchVM.SearchItem )).ToList();
        //        ListOfAssets = ItemsList;
        //        if (ListOfAssets.Count == 0)
        //        {
        //            _toastNotification.AddErrorToastMessage("This item Not Found");
        //            return Page();
        //        }
        //        return Page();

        //}

        public async Task<IActionResult> OnPostAddToFavorate(int ItemId)
        {
            bool Islogin = true;
            bool IsExists = false;
            bool OutOfStock = false;
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    Islogin = false;
                    _toastNotification.AddErrorToastMessage("Must be login first");

                    return new JsonResult(Islogin);

                }

                var Item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();

                OutOfStock = Item.OutOfStock;

                if (Item is null)
                {
                    _toastNotification.AddErrorToastMessage("Item not found");

                }

                IsExists = _context.FavouriteItems
                                        .Any(favorite => favorite.ItemId == ItemId
                                                && favorite.UserId == user.Id);
                if (IsExists)
                {

                    var favouriteItem = _context.FavouriteItems
                                            .Where(e => e.ItemId == ItemId && e.UserId == user.Id)
                                            .FirstOrDefault();

                    if (favouriteItem == null)
                    {
                        _toastNotification.AddErrorToastMessage("Item not found in favourite");
                        return Page();
                    }

                    _context.Remove(favouriteItem);

                    _context.SaveChanges();


                    _toastNotification.AddSuccessToastMessage("Item removed from favourite");

                    Object obj1 = new { Islogin, ItemId, IsExists };

                    return new JsonResult(obj1);

                }
                else
                {

                    var favouriteItem = new FavouriteItem()
                    {
                        ItemId = ItemId,
                        UserId = user.Id
                    };

                    _context.Add(favouriteItem);
                    _context.SaveChanges();

                    _toastNotification.AddSuccessToastMessage("Item added to favourite");

                }

            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

            }
            Object obj = new { Islogin, ItemId, IsExists };

            return new JsonResult(obj);
        }

        public async Task<IActionResult> OnPostAddToCart(int ItemId,double priceCart)
        {
            var items = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();
            return new JsonResult(items);
        }
        

        //public async Task<IActionResult> OnPostAddToCart(int ItemId)
        //{
        //    bool Islogin = false;
        //    bool OutOfStock = false;

        //    try
        //    {
        //        int Quantity = 1;

        //        var user = await _userManager.GetUserAsync(User);

        //        if (user is null)
        //        {
        //            _toastNotification.AddErrorToastMessage("Must Be Login First");

        //            return new JsonResult(Islogin);

        //        }

        //        Islogin = true;

        //        var UserId = _context.CustomerNs.Where(a => a.Email == user.Email)
        //                        .FirstOrDefault().CustomerId;


        //        var UserIpAddress = GetUserIpAddress();

        //        var country = GetUserCountryByIp(UserIpAddress);

        //        string CountryToUse;

        //        if (country != null)
        //        {
        //            var DbCountry = _context.Country.Any(i => i.CountryTlen == country);

        //            CountryToUse = country;

        //            if (!DbCountry)
        //            {
        //                var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
        //                CountryToUse = firstCountry;
        //            }


        //        }
        //        else
        //        {
        //            var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
        //            CountryToUse = firstCountry;
        //        }

        //        var DbUserCart = await _context.ShoppingCart.AnyAsync(a => a.ItemId == ItemId
        //                                       && a.CustomerId == UserId);

        //        OutOfStock = _context.Item.Where(i => i.ItemId == ItemId).FirstOrDefault().OutOfStock;


        //        if (DbUserCart)
        //        {

        //            _toastNotification.AddSuccessToastMessage("Item exist in cart");

        //            return new JsonResult(Islogin);

        //            //var UserItem = await _context.ShoppingCart
        //            //                        .FirstOrDefaultAsync(a => a.ItemId == ItemId && a.CustomerId==UserId);

        //            //UserItem.ItemQty = Quantity;

        //            //UserItem.ItemTotal = Quantity * UserItem.ItemPrice;

        //            //_context.SaveChanges();

        //            //_toastNotification.AddSuccessToastMessage("Item added to cart");

        //        }
        //        else
        //        {
        //            var Item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();


        //            if (Item is null)
        //            {
        //                _toastNotification.AddSuccessToastMessage("Item is not exist");

        //            }

        //            if (!OutOfStock)
        //            {
        //                double ItemPrice = GetItemPrice(ItemId, CountryToUse);


        //                var CartItem = new ShoppingCart()
        //                {
        //                    CustomerId = UserId,
        //                    ItemId = ItemId,
        //                    ItemPrice = ItemPrice,
        //                    ItemQty = Quantity,
        //                    ItemTotal = Quantity * ItemPrice,

        //                };


        //                _context.ShoppingCart.Add(CartItem);
        //                _context.SaveChanges();

        //                _toastNotification.AddSuccessToastMessage("Item added to cart");
        //            }
        //            else
        //            {
        //                _toastNotification.AddWarningToastMessage("Item is out of stock");
        //            }
        //        }


        //    }
        //    catch
        //    {
        //        _toastNotification.AddErrorToastMessage("Something went wrong");

        //    }

        //    return new JsonResult(Islogin);
        //}

        public async Task<IActionResult> OnPostOnSubscribe()
        {
            try
            {
                var EmailsfromNewLetter = _context.Newsletters.Any(i => i.Email == Email);
                if (EmailsfromNewLetter)
                {
                    _toastNotification.AddInfoToastMessage("This Email Is already Subscribed");
                    return RedirectToPage("/index");
                }
                var Newsletter = new Newsletter
                {
                    Email = Email,
                    Date = DateTime.Now
                };

                _context.Newsletters.Add(Newsletter);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("You Have Been Subscribed");
                return RedirectToPage("/index");
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went Wrong");
                return RedirectToPage("/index");
            }

        }



        private double GetItemPrice(int ItemId, string Country)
        {
            var DbcountryId = _context.Country.Where(c => c.CountryTlen == Country).FirstOrDefault().CountryId;

            var DbItemId = _context.itemPriceNs.FirstOrDefault(a => a.ItemId == ItemId && a.CountryId == DbcountryId);

            if (DbItemId is null)
            {
                return 0;
            }
            double? DbItemPrice = DbItemId.Price;

            return (double)DbItemPrice;
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




        //private double GetItemPrice(int ItemId)
        //{
        //    var DbItemId = _context.itemPriceNs.FirstOrDefault(a => a.ItemId == ItemId);

        //    if (DbItemId is null)
        //    {
        //        return 0;
        //    }
        //    double? DbItemPrice = DbItemId.Price;

        //    return (double)DbItemPrice;
        //}

        //public async Task<IActionResult> OnGetSingleCategoryItems(int CountryId, int CategoryId)
        //{

        //    try
        //    {
        //        string userId = null;

        //        var user = await _userManager.GetUserAsync(User);

        //        if (user != null)
        //        {
        //            userId = user.Id;

        //        }

        //        var country = _context.Country.Where(e => e.CountryId == CountryId).FirstOrDefault();
        //        if (country == null)
        //        {
        //            var Fcountry = _context.Country.FirstOrDefault();
        //            if (Fcountry == null)
        //            {
        //                CountryId = 0;
        //            }
        //            else
        //            {
        //                CountryId = Fcountry.CountryId;
        //            }
        //        }

        //        var Result = _context.Item.Where(c => c.CategoryId == CategoryId
        //                                && c.OutOfStock == false && c.IsActive)
        //                            .Include(c => c.ItemPriceNs)
        //                            .Include(i => i.ItemImageNavigation)
        //                            .Select(i => new
        //                            {
        //                                ItemImage = i.ItemImage,
        //                                ItemId = i.ItemId,
        //                                ItemDescriptionAr = i.ItemDescriptionAr,
        //                                ItemDescriptionEn = i.ItemDescriptionEn,
        //                                ItemTitleEn = i.ItemTitleEn,
        //                                ItemTitleAr = i.ItemTitleAr,
        //                                Weight = i.Weight,
        //                                OrderItem = i.OrderItem,
        //                                OutOfStock = i.OutOfStock,
        //                                OrderIndex = i.OrderIndex,
        //                                IsActive = i.IsActive,
        //                                ItemPrice = _context.itemPriceNs.Where(p => p.ItemId == i.ItemId && p.CountryId == CountryId).FirstOrDefault().Price,
        //                                IsFavorate = _context.FavouriteItems.Any(f => f.ItemId == i.ItemId && f.UserId == userId)
        //                            });



        //        if (Result is null)
        //        {
        //            return NotFound();
        //        }

        //        return new JsonResult(Result);

        //    }

        //    catch
        //    {
        //        _toastNotification.AddErrorToastMessage("Something went wrong");

        //        return RedirectToPage("/default");
        //    }



        //    //string userId = null;

        //    //var user = await _userManager.GetUserAsync(User);

        //    //if (user != null)
        //    //{
        //    //    userId = user.Id;
        //    //}
        //    //var Result = _context.Item.Where(c => c.CategoryId == CategoryId
        //    //                        && c.OutOfStock == false && c.IsActive)
        //    //                    .Include(c => c.ItemPriceNs)
        //    //                    .Include(i => i.ItemImageNavigation)
        //    //                    .Select(i => new
        //    //                    {
        //    //                        ItemImage = i.ItemImage,
        //    //                        ItemId = i.ItemId,
        //    //                        ItemDescriptionAr = i.ItemDescriptionAr,
        //    //                        ItemDescriptionEn = i.ItemDescriptionEn,
        //    //                        ItemTitleEn = i.ItemTitleEn,
        //    //                        ItemTitleAr = i.ItemTitleAr,
        //    //                        Weight = i.Weight,
        //    //                        OrderItem = i.OrderItem,
        //    //                        OutOfStock = i.OutOfStock,
        //    //                        OrderIndex = i.OrderIndex,
        //    //                        IsActive = i.IsActive,
        //    //                        ItemPrice = _context.itemPriceNs.Where(p => p.ItemId == i.ItemId).FirstOrDefault().Price,
        //    //                        IsFavorate = _context.FavouriteItems.Any(f => f.ItemId == i.ItemId && f.UserId == "1")
        //    //                    }) ;

        //    //return new JsonResult(Result);
        //}
    }
}
