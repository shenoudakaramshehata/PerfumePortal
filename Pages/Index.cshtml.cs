using CRM.Data;
using CRM.Migrations;
using CRM.Models;
using CRM.ViewModels;
using DevExpress.CodeParser;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.WebUtils;
using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NToastNotify;
using NuGet.ContentModel;
using System;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Microsoft.AspNetCore.Localization;
using static DevExpress.Xpo.Helpers.PerformanceCounters;

namespace CRM.Pages
{
    public class IndexModel : PageModel
    {

        private readonly PerfumeContext _context;

        private readonly IToastNotification _toastNotification;

        private readonly UserManager<ApplicationUser> _userManager;

        public string Country { get; set; }
        //public static string contryImage { get; set; }
        [BindProperty]
        public bool IsFavorite { get; set; }
        public static string staticContryImage { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ArLang { get; set; }
        public string contryImage { get; set; }
        public string ProductUrl { get; set; }

        [BindProperty]
        public int itemQuantity { get; set; } = 1;

        //public List<Category> categoriesList { get; set; }
        public List<ProductsVM> ItemsList { get; set; }
        public static List<ProductsVM> ItemsListstatic { get; set; }

        [BindProperty]
        public List<ShoppingCart> shoppingCarts { get; set; }
        public string CurrencyNameEN { set; get; }
        public string CurrencyNameAr { set; get; }
        public Item items { get; set; }
        public double TotalAmount { get; set; }
        //public int FirstCatId { get; set; }
        //public int FirstCounId { get; set; }
        //public string LanguageEn_Ar { get; set; }
        public ContactUs EmailsfromNewLetter { get; set; }
        public int CountryIdByIPAddress { get; set; }
        public string CurrencyNameENCart { get; set; }
        public string CurrencyNameArCart { get; set; }

        [BindProperty]
        public ItemSearchVM itemSearchVM { get; set; }

        [BindProperty]
        public string Email { get; set; }
        //[BindProperty]
        //public string CountryImageGet { get; set; }
        public List<ShippingCartObjectVM> shippingCartObjectVMs { get; set; }
        public List<CustomerN> customerNs { get; set; }
        [BindProperty]
        public CustomerVM customerVM { get; set; }
        public List<ItemPriceN> itemPriceByCountryId { get; set; }
        public IRequestCultureFeature locale;
        public string BrowserCulture;
        public IndexModel(PerfumeContext perfumeContext, IToastNotification toastNotification
                                , UserManager<ApplicationUser> userManager)
        {
            _context = perfumeContext;
            _toastNotification = toastNotification;
            _userManager = userManager;
            shippingCartObjectVMs = new List<ShippingCartObjectVM>();
            customerVM = new CustomerVM();
        }

        public async Task<IActionResult> OnGet()
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();
            if (BrowserCulture == "en-US")

                ArLang = false;

            else
                ArLang = true;
        
    

        shoppingCarts = _context.ShoppingCart.Include(i => i.Item)
                           .Include(i => i.CustomerN).ToList();


            TotalAmount = _context.ShoppingCart.Sum(i => i.ItemTotal);
            ProductUrl = $"{this.Request.Scheme}://{this.Request.Host}";

            //var UserIpAddress = GetUserIpAddress();

            //var country = GetUserCountryByIp(UserIpAddress);

            //GetCurrencyName(country);

            //CheckItemPrice();

            //var CountryImageGet = HttpContext.Session.GetString("countryImage");
            //ViewData["couImage"] = CountryImageGet;
            //var newcountrySession = HttpContext.Session.GetString("country");
            //int CountryId = 0;
            //bool countryCheck = int.TryParse(newcountrySession, out CountryId);
            //if (countryCheck == true)
            //{
            //    contryImage = _context.Country.Where(e => e.CountryId == CountryId).FirstOrDefault().Pic;

            //    //HttpContext.Session.SetString("countryImage", contryImage);
            //}
             //contryImage = staticContryImage;
            //staticContryImage = "";

            return Page();

        }




        //public void CheckItemPrice()
        //{
        //    var ItemsLists = _context.Item.Where(c => c.CategoryId == 1 && c.IsActive == true).ToList();
        //    try
        //    {
        //        for (int i = 0; i < ItemsLists.Count; i++)
        //        {
        //            var itemid = ItemsLists[i].ItemId;
        //            var ItemPrice = _context.itemPriceNs.Any(I => I.CountryId == CountryIdByIPAddress && I.ItemId == itemid);
        //            if (!ItemPrice)
        //            {
        //                var NewItemPrice = new ItemPriceN
        //                {
        //                    ItemId = itemid,
        //                    CountryId = CountryIdByIPAddress,
        //                    Price = 0,
        //                    ShippingPrice = 0
        //                };
        //                _context.itemPriceNs.Add(NewItemPrice);
        //                _context.SaveChanges();

        //            }
        //        }
        //    }
        //    catch
        //    {
        //        _toastNotification.AddErrorToastMessage("Something went Wrong price");

        //    }

        //}

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

        public async Task<IActionResult> OnPostAddToCart(int ItemId, double itemPriceN, string Country)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                bool IsRegister = false;    
                 items = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();
                var CountryShoppingCost = _context.Country.Where(e => e.CountryId == int.Parse(Country)).FirstOrDefault().ShippingCost.Value;
               var priceCart = _context.itemPriceNs.Where(i => i.ItemId == ItemId && i.CountryId == int.Parse(Country)).FirstOrDefault().Price.Value;
                

                var result = new { items, CountryShoppingCost, IsRegister, priceCart };
                return new JsonResult(result);
            }
            if (user != null)
            {
                bool OutOfStock = false;

                try
                {
                    int Quantity = 1;




                    var UserId = _context.CustomerNs.Where(a => a.Email == user.Email)
                                    .FirstOrDefault().CustomerId;

                    var DbUserCart = await _context.ShoppingCart.AnyAsync(a => a.ItemId == ItemId
                                                   && a.CustomerId == UserId);

                    OutOfStock = _context.Item.Where(i => i.ItemId == ItemId).FirstOrDefault().OutOfStock;


                    if (DbUserCart)
                    {


                        var UserItem = await _context.ShoppingCart
                                                .FirstOrDefaultAsync(a => a.ItemId == ItemId && a.CustomerId == UserId);

                        UserItem.ItemQty += 1;

                        UserItem.ItemTotal = (UserItem.ItemQty) * UserItem.ItemPrice;
                        var UpdatedCart = _context.ShoppingCart.Attach(UserItem);
                        UpdatedCart.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Item exist in cart");
                        //_toastNotification.AddSuccessToastMessage("Item added to cart");

                    }

                    else
                    {
                        var Item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();


                        if (Item is null)
                        {
                            _toastNotification.AddSuccessToastMessage("Item is not exist");

                        }

                        if (!OutOfStock)
                        {
                            double ItemPrice = GetItemPrice(ItemId, Country);


                            var CartItem = new ShoppingCart()
                            {
                                CustomerId = UserId,
                                ItemId = ItemId,
                                ItemPrice = ItemPrice,
                                ItemQty = Quantity,
                                ItemTotal = Quantity * ItemPrice,

                            };


                            _context.ShoppingCart.Add(CartItem);
                            _context.SaveChanges();

                            _toastNotification.AddSuccessToastMessage("Item added to cart");
                        }
                        else
                        {
                            _toastNotification.AddWarningToastMessage("Item is out of stock");
                        }
                    }


                }
                catch
                {
                    _toastNotification.AddErrorToastMessage("Something went wrong item");

                }

                return new JsonResult("true");
            }
            return new JsonResult("true");
        }

        public async Task<IActionResult> OnGetAllItems(string country)
        {
            var currUser = await _userManager.GetUserAsync(User);
            //staticContryImage = _context.Country.Where(e => e.CountryId == int.Parse(country)).FirstOrDefault().Pic;

            var products = _context.Item.Where(c => c.CategoryId == 1 && c.IsActive == true).OrderBy(e => e.OrderIndex).Select(e => new
            {
                ItemId = e.ItemId,
                OutOfStock = e.OutOfStock,
                ItemImage = e.ItemImage,
                ItemTitleAr = e.ItemTitleAr,
                ItemTitleEn = e.ItemTitleEn,
                price = _context.itemPriceNs.Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().Price != 0 ? _context.itemPriceNs.Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().Price : 0,
                currencyTEName = _context.itemPriceNs.Include(e => e.Country).ThenInclude(e => e.Currency).Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().Country.Currency.CurrencyTlen != null ? _context.itemPriceNs.Include(e => e.Country).ThenInclude(e => e.Currency).Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().Country.Currency.CurrencyTlen : null,
                currencyARName = _context.itemPriceNs.Include(e => e.Country).ThenInclude(e => e.Currency).Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().Country.Currency.CurrencyTlar != null ? _context.itemPriceNs.Include(e => e.Country).ThenInclude(e => e.Currency).Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().Country.Currency.CurrencyTlar : null,
                FavorateItem = currUser != null ? _context.FavouriteItems.Any(c => c.UserId == currUser.Id && c.ItemId == e.ItemId) : false,
               Stock = e.Stock!=null? e.Stock:0,
               BeforePrice= _context.itemPriceNs.Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().BeforePrice != 0 ? _context.itemPriceNs.Where(c => c.ItemId == e.ItemId && c.CountryId == int.Parse(country)).FirstOrDefault().BeforePrice : 0,
            }).ToList();
            ItemsList = new List<ProductsVM>();
            foreach (var item in products)
            {
                var productVM = new ProductsVM()
                {
                    ItemId = item.ItemId!=0? item.ItemId:0,
                    price = item.price!=0? item.price:0,
                    ItemImage = item.ItemImage!=null? item.ItemImage:null,
                    ItemTitleAr = item.ItemTitleAr != null ? item.ItemTitleAr : null,
                    ItemTitleEn = item.ItemTitleEn != null ? item.ItemTitleEn : null,
                    OutOfStock = item.OutOfStock!=false? item.OutOfStock:false,
                    CurrencyAR = item.currencyARName!=null? item.currencyARName:null,
                    CurrencyEN = item.currencyTEName != null ? item.currencyTEName : null,
                    FavorateItem = item.FavorateItem!=false? item.FavorateItem:false,
                    Beforeprice= item.BeforePrice != null ? item.BeforePrice : 0,
                    Stock= item.Stock!=null? item.Stock.Value:0,   

                };
                ItemsList.Add(productVM);
            }

            return new JsonResult(ItemsList);
        }

        public async Task<IActionResult> OnGetQuickViewItems(int ItemId,string country)
        {
            var items = _context.Item.Where(e => e.ItemId == ItemId).Select(e => new
            {
                e.ItemId,
                e.ItemDescriptionAr,
                e.ItemDescriptionEn,
                e.Category.CategoryTlar,
                e.Category.CategoryTlen,
                e.Category.DescriptionTlen,
                e.ItemImage,
                e.ItemTitleAr,
                e.ItemTitleEn,
                e.OutOfStock,
                e.Weight,
                e.Stock

            }).FirstOrDefault();
            
            double itemPriceN =  _context.itemPriceNs.Where(i => i.ItemId == ItemId && i.CountryId == int.Parse(country)).FirstOrDefault().Price.Value;
            var CurrencyName = _context.Country.Include(e => e.Currency).Where(i => i.CountryId == int.Parse(country)).FirstOrDefault().Currency;
           
            var CurrencyNameEN = CurrencyName.CurrencyTlen;
           var CurrencyNameAr = CurrencyName.CurrencyTlar;
            var user = await _userManager.GetUserAsync(User);
            

            if (user != null)
            {
                var CustomerId = _context.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;

                var hasShopingCart = _context.ShoppingCart.Any(s => s.ItemId == ItemId && s.CustomerId == CustomerId);
                IsFavorite = _context.FavouriteItems.Any(favorite => favorite.ItemId == ItemId
                                                          && favorite.UserId == user.Id);
                if (hasShopingCart)
                {
                    itemQuantity = _context.ShoppingCart.Where(i => i.ItemId == ItemId && i.CustomerId == CustomerId).FirstOrDefault().ItemQty;
                }
                else
                {
                    itemQuantity = 1;
                }
                
            }
            else
            {
                itemQuantity = 1;
            }
            var result = new { items, itemPriceN, CurrencyName, CurrencyNameEN, CurrencyNameAr, IsFavorite, itemQuantity };
            try
            {
                
                return new JsonResult(result);
            }
            catch(Exception e)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return new JsonResult(result);
        }
        public async Task<IActionResult> OnPostAddToFavorite(int ItemId)
        {
            bool Islogin = true;
            bool IsExists = false;
            bool OutOfStock = false;
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    Islogin = false;
                    _toastNotification.AddErrorToastMessage("Must be login first");

                    return new JsonResult(Islogin);

                    //return Redirect("~/login");
                }
                var Item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();

                if (Item is null)
                {
                    _toastNotification.AddErrorToastMessage("Item Not Found");
                    IsFavorite = false;
                }
                 IsExists = _context.FavouriteItems
                             .Any(favorite => favorite.ItemId == ItemId
                                     && favorite.UserId == user.Id);
                var favItem = _context.FavouriteItems
                                           .Where(e => e.ItemId == ItemId && e.UserId == user.Id)
                                           .FirstOrDefault();
                if (IsExists)
                {
                    _context.Remove(favItem);

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

                    IsFavorite = true;

                    _toastNotification.AddSuccessToastMessage("Item Added Successfully");
                }
                Object obj = new { Islogin, ItemId, IsExists };

                return new JsonResult(obj);
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
                return Redirect("/Index");
            }

        }
        public IActionResult OnPostStoredata(string parsecartItems, string country)
        {

            HttpContext.Session.SetString("parsecartItems", parsecartItems);
            HttpContext.Session.SetString("country", country);

            var newcountrySession = HttpContext.Session.GetString("country");
            int CountryId = 0;
            bool countryCheck = int.TryParse(newcountrySession, out CountryId);
            Country NewContry=new Country();
            if (countryCheck == true)
            {
                NewContry = _context.Country.Where(e => e.CountryId == CountryId).FirstOrDefault();

                //if (BrowserCulture == "en-US")
                //{
                //    NewContryText = _context.Country.Where(e => e.CountryId == CountryId).FirstOrDefault().CountryTlen;
                //}
                //else
                //{
                //    NewContryText = _context.Country.Where(e => e.CountryId == CountryId).FirstOrDefault().CountryTlar;
                //}

                //HttpContext.Session.SetString("countryImage", contryImage);
            }


            //var data = HttpContext.Session.GetString("parsecartItems");
            //var Country = HttpContext.Session.GetString("country");


            return new JsonResult(NewContry);

        }
        public async Task<IActionResult> OnPostShoppingCartIcon(string parsecartItems)
        {
            var user = await _userManager.GetUserAsync(User);
            bool IsRegister = false;
            int countA = 0;
            if (user == null)
            {
                List<ShippingCartObjectVM> shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(parsecartItems);
                List<ShippingCartObjectVM> EmptyShippingCartList = new List<ShippingCartObjectVM>();
                if (shippingCartObjectVMs != null)
                {
                    if (shippingCartObjectVMs.Count != 0)
                    {
                        countA = shippingCartObjectVMs.Count();
                        var result = new { countA, IsRegister };
                        return new JsonResult(result);
                    }
                }
             
            }
            else
            {
                var customerId = _context.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;

                int Count = _context.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item)
                                .Include(i => i.CustomerN).Count();
                IsRegister = true;
                var result = new { Count, TotalAmount, IsRegister };

                return new JsonResult(result);
            }
            return new JsonResult("result");

        }

        public async Task<IActionResult> OnPostUserCart(string parsecartItems) {
            bool ISLogin = false;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                List<ShoppingCart> EmptyShippingCartList = new List<ShoppingCart>();
                shippingCartObjectVMs = JsonConvert.DeserializeObject<List<ShippingCartObjectVM>>(parsecartItems);


                var customerObj = _context.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return NotFound();

                }
                if (shippingCartObjectVMs != null)
                {
                    if (shippingCartObjectVMs.Count != 0)
                    {
                        foreach (var item in shippingCartObjectVMs)
                        {
                            var shippingCartObj = new ShoppingCart()
                            {
                                CustomerId = customerObj.CustomerId,
                                ItemId = item.ItemId,
                                ItemPrice = item.priceCart,
                                ItemQty = item.Qunatity,
                                ItemTotal = item.priceCart * item.Qunatity
                            };
                            EmptyShippingCartList.Add(shippingCartObj);
                            _context.ShoppingCart.Add(shippingCartObj);
                            _context.SaveChanges();
                        }
                        ISLogin = true;
                        return new JsonResult(ISLogin);
                    }
                }

            }

            return new JsonResult(ISLogin);
        }
        public async Task<IActionResult> OnPostDisplayProducts(string prodcutcartItems,string country)
        {
            int countryid = int.Parse(country);
            bool IsRegister = false;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                string CurrencyEN = _context.Country.Include(e => e.Currency).Where(i => i.CountryId == countryid).Select(e=>e.CountryTlen).FirstOrDefault();
                var CurrencyNameAr = _context.Country.Include(e => e.Currency).Where(i => i.CountryId == countryid).FirstOrDefault().Currency.CurrencyTlar;
                var result = new { prodcutcartItems, IsRegister, CurrencyEN, CurrencyNameAr };

                return new JsonResult(result);
            }
            else
            {
                var customerId = _context.CustomerNs.Where(i => i.Email == user.Email).FirstOrDefault().CustomerId;

                shoppingCarts = _context.ShoppingCart.Where(i => i.CustomerId == customerId).Include(i => i.Item)
                                .Include(i => i.CustomerN).ToList();
                TotalAmount = _context.ShoppingCart.Where(i => i.CustomerId == customerId).Sum(i => i.ItemTotal);

                IsRegister = true;
                string CurrencyEN = _context.Country.Include(e => e.Currency).Where(i => i.CountryId == countryid).Select(e => e.CountryTlen).FirstOrDefault();
                var CurrencyNameAr = _context.Country.Include(e => e.Currency).Where(i => i.CountryId == countryid).FirstOrDefault().Currency.CurrencyTlar;
               
                var result = new { shoppingCarts, TotalAmount, IsRegister, CurrencyEN, CurrencyNameAr };


                return new JsonResult(result);
            }
        }

        public async Task<IActionResult> OnPostDecreaseItemQuantity(int ItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var UserId = _context.CustomerNs.Where(a => a.Email == user.Email)
                                                   .FirstOrDefault()
                                                   .CustomerId;

                var itemObj = _context.ShoppingCart
                            .Where(e => e.ItemId == ItemId && e.CustomerId == UserId)
                            .FirstOrDefault();
                if (itemObj != null)
                {

                    if (itemObj.ItemQty > 1)
                    {
                        itemObj.ItemQty -= 1;
                        itemObj.ItemTotal = itemObj.ItemQty * itemObj.ItemPrice;
                        _context.Attach(itemObj).State = EntityState.Modified;
                        _context.SaveChanges();

                    }
                }
                return new JsonResult(itemObj);
            }
            return new JsonResult("false");
        }

        public async Task<IActionResult> OnPostIncreaseItemQuantity(int ItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var UserId = _context.CustomerNs.Where(a => a.Email == user.Email)
                                                   .FirstOrDefault()
                                                   .CustomerId;

                var itemObj = _context.ShoppingCart
                            .Where(e => e.ItemId == ItemId && e.CustomerId == UserId)
                            .FirstOrDefault();
                if (itemObj != null)
                {

                    
                    
                        itemObj.ItemQty += 1;
                        itemObj.ItemTotal = itemObj.ItemQty * itemObj.ItemPrice;
                        _context.Attach(itemObj).State = EntityState.Modified;
                        _context.SaveChanges();

                   
                }
                return new JsonResult(itemObj);
            }
            return new JsonResult("false");
        }

        public async Task<IActionResult> OnPostRemoveItem(int ItemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                

                var Item = _context.ShoppingCart.Where(e => e.ItemId == ItemId).FirstOrDefault();

                if (Item == null)
                {
                    _toastNotification.AddErrorToastMessage("Item Not Found");
                    return Page();

                }

                var UserId = _context.CustomerNs.Where(a => a.Email == user.Email)
                                                    .FirstOrDefault()
                                                    .CustomerId;

                var cartItem = _context.ShoppingCart
                                    .Where(e => e.ItemId == ItemId && e.CustomerId == UserId)
                                    .FirstOrDefault();

                if (cartItem == null)
                {
                    _toastNotification.AddErrorToastMessage("Item Not Found in shopping cart");
                    return Page();
                }

                _context.ShoppingCart.Remove(cartItem);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Item Removed Successfully From shopping cart");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage("Somthing Went Wrong remove");
                return Page();
            }
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

            //public async Task<IActionResult> OnPostOnSubscribe()
            //{
            //    try
            //    {
            //        var EmailsfromNewLetter = _context.Newsletters.Any(i => i.Email == Email);
            //        if (EmailsfromNewLetter)
            //        {
            //            _toastNotification.AddInfoToastMessage("This Email Is already Subscribed");
            //            return RedirectToPage("/index");
            //        }
            //        var Newsletter = new Newsletter
            //        {
            //            Email = Email,
            //            Date = DateTime.Now
            //        };

            //        _context.Newsletters.Add(Newsletter);
            //        _context.SaveChanges();
            //        _toastNotification.AddSuccessToastMessage("You Have Been Subscribed");
            //        return RedirectToPage("/index");
            //    }
            //    catch
            //    {
            //        _toastNotification.AddErrorToastMessage("Something went Wrong");
            //        return RedirectToPage("/index");
            //    }

            //}

        public async Task<IActionResult> OnPostCartcleared()
        {
            bool ISLogin = false;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                

                var customerObj = _context.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                
                if (customerObj == null)
                {
                    return NotFound();

                }
                var ShoppingCartList = _context.ShoppingCart.Where(e => e.CustomerId == customerObj.CustomerId).ToList();
                var favourite = _context.FavouriteItems.Where(e => e.UserId == user.Id).ToList();
                if (ShoppingCartList != null)
                {
                    _context.ShoppingCart.RemoveRange(ShoppingCartList);
                    _context.SaveChanges();
                }

                if (favourite != null)
                {
                    _context.FavouriteItems.RemoveRange(favourite);
                    _context.SaveChanges();
                }
                ISLogin = true;
            }

            return new JsonResult(ISLogin);
        }

        private double GetItemPrice(int ItemId, string Country)
            {
                var DbcountryId = _context.Country.Where(c => c.CountryId == int.Parse(Country)).FirstOrDefault().CountryId;

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

        public IActionResult OnPost()
        {
            try
            {
                var Email = Request.Form["EMAIL"].ToString();

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