using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Diagnostics.Metrics;

namespace CRM.Pages
{
    public class DefaultModel : PageModel
    {
        private readonly PerfumeContext _context;

        private readonly IToastNotification _toastNotification;

        private readonly UserManager<ApplicationUser> _userManager;

        public string ProductUrl { get; set; }

        public List<Category> categoriesList { get; set; }

        public int FirstCatId { get; set; }
        public int FirstCounId { get; set; }
        public string LanguageEn_Ar { get; set; }

        
        public DefaultModel(PerfumeContext perfumeContext,IToastNotification toastNotification
                                , UserManager<ApplicationUser> userManager)
        {
            _context = perfumeContext;
            _toastNotification = toastNotification;
            _userManager = userManager;
        }

        public void OnGet()
        {
            categoriesList = _context.Categories.ToList();
            ProductUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            FirstCatId = _context.Categories.FirstOrDefault().CategoryId;
            FirstCounId = _context.Country.FirstOrDefault().CountryId;
        }

        public ActionResult OnGetSingleCategory()
        {
            var result = _context.Categories.ToList();

            return new JsonResult(result);
        }



        public async Task<IActionResult> OnGetSingleCategoryItems(int CountryId,int CategoryId)
        {

            try
            {
                string userId = null;

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    userId = user.Id;

                }

                var country = _context.Country.Where(e => e.CountryId == CountryId).FirstOrDefault();
                if (country == null)
                {
                    var Fcountry = _context.Country.FirstOrDefault();
                    if (Fcountry == null)
                    {
                        CountryId = 0;
                    }
                    else
                    {
                        CountryId = Fcountry.CountryId;
                    }
                }

                var Result = _context.Item.Where(c => c.CategoryId == CategoryId
                                        && c.OutOfStock == false && c.IsActive )
                                    .Include(c => c.ItemPriceNs)
                                    .Include(i => i.ItemImageNavigation)
                                    .Select(i => new
                                    {
                                        ItemImage = i.ItemImage,
                                        ItemId = i.ItemId,
                                        ItemDescriptionAr = i.ItemDescriptionAr,
                                        ItemDescriptionEn = i.ItemDescriptionEn,
                                        ItemTitleEn = i.ItemTitleEn,
                                        ItemTitleAr = i.ItemTitleAr,
                                        Weight = i.Weight,
                                        OrderItem = i.OrderItem,
                                        OutOfStock = i.OutOfStock,
                                        OrderIndex = i.OrderIndex,
                                        IsActive = i.IsActive,
                                        ItemPrice = _context.itemPriceNs.Where(p => p.ItemId == i.ItemId && p.CountryId == CountryId).FirstOrDefault().Price,
                                        IsFavorate = _context.FavouriteItems.Any(f => f.ItemId == i.ItemId && f.UserId == userId)
                                    });

               

                if (Result is null)
                {
                    return NotFound();
                }

                return new JsonResult(Result);

            }

            catch 
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return RedirectToPage("/default");
            }



            //string userId = null;

            //var user = await _userManager.GetUserAsync(User);

            //if (user != null)
            //{
            //    userId = user.Id;
            //}
            //var Result = _context.Item.Where(c => c.CategoryId == CategoryId
            //                        && c.OutOfStock == false && c.IsActive)
            //                    .Include(c => c.ItemPriceNs)
            //                    .Include(i => i.ItemImageNavigation)
            //                    .Select(i => new
            //                    {
            //                        ItemImage = i.ItemImage,
            //                        ItemId = i.ItemId,
            //                        ItemDescriptionAr = i.ItemDescriptionAr,
            //                        ItemDescriptionEn = i.ItemDescriptionEn,
            //                        ItemTitleEn = i.ItemTitleEn,
            //                        ItemTitleAr = i.ItemTitleAr,
            //                        Weight = i.Weight,
            //                        OrderItem = i.OrderItem,
            //                        OutOfStock = i.OutOfStock,
            //                        OrderIndex = i.OrderIndex,
            //                        IsActive = i.IsActive,
            //                        ItemPrice = _context.itemPriceNs.Where(p => p.ItemId == i.ItemId).FirstOrDefault().Price,
            //                        IsFavorate = _context.FavouriteItems.Any(f => f.ItemId == i.ItemId && f.UserId == "1")
            //                    }) ;

            //return new JsonResult(Result);
        }


        public async Task<IActionResult> OnPostAddToFavorate(int ItemId)
        {
            bool Islogin = false;
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    
                    _toastNotification.AddErrorToastMessage("Must Be Login First");
                    return new JsonResult(Islogin);

                }
                Islogin = true;
                var Item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();

                if (Item is null)
                {
                    _toastNotification.AddErrorToastMessage("Item Not Found");
                   
                }

                var IsExists = _context.FavouriteItems
                                        .Any(favorite => favorite.ItemId == ItemId
                                                && favorite.UserId == user.Id);
                if (IsExists)
                {

                    var favouriteItem = _context.FavouriteItems
                                            .Where(e => e.ItemId == ItemId && e.UserId == user.Id)
                                            .FirstOrDefault();

                    if (favouriteItem == null)
                    {
                        _toastNotification.AddErrorToastMessage("Item Not Found In Favourite");

                    }

                    _context.Remove(favouriteItem);

                    _context.SaveChanges();


                    _toastNotification.AddSuccessToastMessage("Item removed from favorate");

                    return RedirectToPage("/default");
                   
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

                    _toastNotification.AddSuccessToastMessage("Item added to favorate");
                   
                }

            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went Wrong");
               
            }


            return new JsonResult(Islogin);
        }


        public async Task<IActionResult> OnPostAddToCart(int ItemId)
        {
            bool Islogin = false;

            try
            {
                int Quantity = 1;

                var user = await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    _toastNotification.AddErrorToastMessage("Must Be Login First");

                    return new JsonResult(Islogin);

                }

                Islogin = true;

                var UserId = _context.CustomerNs.Where(a => a.Email == user.Email)
                                .FirstOrDefault()
                                .CustomerId;

                var DbUserCart = await _context.ShoppingCart.AnyAsync(a => a.ItemId == ItemId
                                                && a.CustomerId == UserId);

                if (DbUserCart)
                {
                    var UserItem = await _context.ShoppingCart
                                            .FirstOrDefaultAsync(a => a.ItemId == ItemId);

                    UserItem.ItemQty += Quantity;

                    UserItem.ItemTotal += Quantity * UserItem.ItemPrice;

                    _context.SaveChanges();

                    _toastNotification.AddSuccessToastMessage("Item added to cart");
                   
                }
                else
                {
                    var Item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();

                    if (Item is null)
                    {
                        _toastNotification.AddSuccessToastMessage("Item is not exist");

                    }

                    double ItemPrice = GetItemPrice(ItemId);


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

            }
            catch 
            {
                _toastNotification.AddErrorToastMessage("Something went Wrong");

            }

            return new JsonResult(Islogin);
        }


        private double GetItemPrice(int ItemId)
        {
            var DbItemId = _context.itemPriceNs.FirstOrDefault(a => a.ItemId == ItemId);

            if (DbItemId is null)
            {
                return 0;
            }
            double? DbItemPrice = DbItemId.Price;

            return (double)DbItemPrice;
        }
    }
}
