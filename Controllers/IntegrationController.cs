using CRM.Data;
using CRM.Models;
using CRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    public class IntegrationController : ControllerBase
    {
        private readonly PerfumeContext _perfumeContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public HttpClient httpClient { get; set; }
        public IntegrationController(PerfumeContext perfumeContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _perfumeContext = perfumeContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            httpClient = new HttpClient();
        }


        //====================================Category APIs=================================//
        [HttpGet]
        [Route("GetAllCategoryWithItems")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategoryWithItems(int countryId)
        {
            try
            {
                string userId = null;

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    userId = user.Id;
                }
                var country = _perfumeContext.Country.Where(e => e.CountryId == countryId).FirstOrDefault();
                if (country == null)
                {
                    var Fcountry = _perfumeContext.Country.FirstOrDefault();
                    if (Fcountry == null)
                    {
                        countryId = 0;
                    }
                    else
                    {
                        countryId = Fcountry.CountryId;
                    }
                }

                var CategoryWithItems = await _perfumeContext.Categories
                                    .Where(a => a.IsActive == true && a.Item.Count() > 0)
                                    .OrderByDescending(a => a.CategoryId)
                                    .Select(a => new
                                    {
                                        CategoryId = a.CategoryId,
                                        CategoryPic = a.CategoryPic,
                                        CategoryTlen = a.CategoryTlen,
                                        CategoryTlar = a.CategoryTlar,
                                        DescriptionTlen = a.DescriptionTlen,
                                        DescriptionTlar = a.DescriptionTlar,

                                        Item = a.Item.Where(e => e.ItemPriceNs.Any(e => e.CountryId == countryId))
                                        .Select(i => new
                                        {
                                            CategoryId = i.CategoryId,
                                            itemId = i.ItemId,
                                            ItemImage = i.ItemImage,
                                            ItemDescriptionAr = i.ItemDescriptionAr,
                                            ItemDescriptionEn = i.ItemDescriptionEn,
                                            ItemTitleEn = i.ItemTitleEn,
                                            ItemTitleAr = i.ItemTitleAr,
                                            Weight = i.Weight,
                                            OrderIndex = i.OrderIndex,
                                            OutOfStock = i.OutOfStock,

                                            ItemImageNavigation = i.ItemImageNavigation,

                                            IsFavorate = _perfumeContext.FavouriteItems
                                                    .Any(f => f.ItemId == i.ItemId && f.UserId == userId),

                                            ItemPrice = i.ItemPriceNs
                                            .Select(p => new
                                            {
                                                p.Price,
                                                p.CountryId,
                                                p.ItemId,
                                            })
                                        })
                                    })
                                    .ToListAsync();

                if (CategoryWithItems is null)
                {
                    return NotFound();
                }

                //if (CategoryWithItems.Any(a => a.IsActive == true))

                return Ok(new { Status = true, CategoryWithItems, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }


        //[HttpGet]
        //[Route("GetCategoryWithItemsById/{id:int}")]
        //public async Task<ActionResult> GetCategoryWithItemsById(int id)
        //{
        //    try
        //    {
        //        var CategoryWithItems = await _perfumeContext.Categories.Include(a => a.Item)
        //                                        .FirstOrDefaultAsync(a => a.CategoryId == id);

        //        if (CategoryWithItems is null)
        //        {
        //            return NotFound();
        //        }


        //        return Ok(new { Status = true, CategoryWithItems, Message = "Process completed successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = "Something went wrong" });
        //    }

        //}


        [HttpGet]
        [Route("GetAllItemsByLastest")]
        public async Task<ActionResult<IEnumerable<Item>>> GetAllItemsByLastest(int countryId)
        {
            try
            {
                string userId = null;

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    userId = user.Id;
                }
                var country = _perfumeContext.Country.Where(e => e.CountryId == countryId).FirstOrDefault();
                if (country == null)
                {
                    var Fcountry = _perfumeContext.Country.FirstOrDefault();
                    if (Fcountry == null)
                    {
                        countryId = 0;
                    }
                    else
                    {
                        countryId = Fcountry.CountryId;
                    }
                }
                var Items = await _perfumeContext.Item.Where(e => e.ItemPriceNs.Any(e => e.CountryId == countryId)).OrderByDescending(a => a.ItemId)
                                    .Select(i => new
                                    {
                                        ItemId = i.ItemId,
                                        CategoryId = i.CategoryId,
                                        ItemTitleAr = i.ItemTitleAr,
                                        ItemTitleEn = i.ItemTitleEn,
                                        ItemDescriptionAr = i.ItemDescriptionAr,
                                        ItemDescriptionEn = i.ItemDescriptionEn,
                                        IsActive = i.IsActive,
                                        OutOfStock = i.OutOfStock,
                                        OrderIndex = i.OrderIndex,
                                        Weight = i.Weight,
                                        ItemImage = i.ItemImage,
                                        ItemImageList = i.ItemImageNavigation,

                                        IsFavorate = _perfumeContext.FavouriteItems
                                                    .Any(favorate => favorate.ItemId == i.ItemId
                                                     && favorate.UserId == userId),

                                        ItemPrice = i.ItemPriceNs,
                                    })
                                    .ToListAsync();

                return Ok(new { Status = true, Items, Message = "Process completed successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }


        [HttpGet]
        [Route("GetItemById/{id:int}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            try
            {
                string userId = null;

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    userId = user.Id;
                }

                var data = await _perfumeContext.Item
                                    .Include(i => i.ItemImageNavigation)
                                    .Where(item => item.ItemId == id)
                                    .Select(item => new
                                    {
                                        CategoryId = item.CategoryId,
                                        ItemId = item.ItemId,
                                        ItemTitleAr = item.ItemTitleAr,
                                        ItemTitleEn = item.ItemTitleEn,
                                        ItemDescriptionAr = item.ItemDescriptionAr,
                                        ItemDescriptionEn = item.ItemDescriptionEn,
                                        ItemImage = item.ItemImage,
                                        IsActive = item.IsActive,
                                        ItemPrice = item.ItemPriceNs,
                                        OrderIndex = item.OrderIndex,
                                        OrderItem = item.OrderItem,
                                        OutOfStock = item.OutOfStock,
                                        Weight = item.Weight,

                                        ItemImageNavigation = item.ItemImageNavigation,

                                        IsFavorite = _perfumeContext.FavouriteItems
                                                        .Any(favorite => favorite.ItemId == item.ItemId
                                                        && favorite.UserId == userId)
                                    }).ToListAsync();


                if (data is null)
                {
                    return NotFound();
                }

                return Ok(new { Status = true, data, Message = "Process completed successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });

            }


        }


        //---------------------------------Favorate API---------------------------------//
        [HttpPost]
        [Route("addItemToFavourite/{itemId:int}")]
        public async Task<IActionResult> addItemToFavourite(int itemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var Item = _perfumeContext.Item.Where(e => e.ItemId == itemId).FirstOrDefault();

                if (Item is null)
                {
                    return Ok(new { Status = false, Message = "Item Not Found", isLogin = true });

                }

                var IsExists = _perfumeContext.FavouriteItems
                                        .Any(favorite => favorite.ItemId == itemId
                                                && favorite.UserId == user.Id);
                if (IsExists)
                {
                    return Ok(new { Status = true, Message = "Item is already added" });
                }
                else
                {
                    var favouriteItem = new FavouriteItem()
                    {
                        ItemId = itemId,
                        UserId = user.Id
                    };

                    _perfumeContext.Add(favouriteItem);
                    _perfumeContext.SaveChanges();

                    return Ok(new { Status = true, Message = "Item Added Successfully" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }

        }


        [HttpDelete]
        [Route("DeleteItemFromFavourite/{itemId:int}")]
        public async Task<IActionResult> DeleteItemFromFavourite(int itemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var Item = _perfumeContext.Item.Where(e => e.ItemId == itemId).FirstOrDefault();

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Item Not Found", isLogin = true });

                }

                var favouriteItem = _perfumeContext.FavouriteItems
                                        .Where(e => e.ItemId == itemId && e.UserId == user.Id)
                                        .FirstOrDefault();

                if (favouriteItem == null)
                {
                    return Ok(new { Status = false, Message = "Item Not Found In Favourite", isLogin = true });

                }

                _perfumeContext.Remove(favouriteItem);
                _perfumeContext.SaveChanges();

                return Ok(new { Status = true, Message = "Item Removed Successfully From Favourite" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }
        }


        [HttpGet]
        [Route("ShowFavouriteItems")]
        public async Task<ActionResult<Item>> ShowFavouriteItems()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }
                var DbUserFavouriteItems = await _perfumeContext.FavouriteItems.AnyAsync(a => a.UserId == a.UserId);
                if (DbUserFavouriteItems)
                {
                    var Cart = await _perfumeContext.FavouriteItems.Where(a => a.UserId == user.Id)

                                        .Select(a => new
                                        {
                                            ItemId = a.ItemId,
                                            ItemImageAndTitle = _perfumeContext.Item
                                                .Where(i => i.ItemId == a.ItemId)
                                                .Select(i => new
                                                {
                                                    CategoryId = i.CategoryId,
                                                    itemId = i.ItemId,
                                                    ItemImage = i.ItemImage,
                                                    ItemDescriptionAr = i.ItemDescriptionAr,
                                                    ItemDescriptionEn = i.ItemDescriptionEn,
                                                    ItemTitleEn = i.ItemTitleEn,
                                                    ItemTitleAr = i.ItemTitleAr,
                                                    Weight = i.Weight,
                                                    OrderIndex = i.OrderIndex,
                                                    OutOfStock = i.OutOfStock,
                                                    ItemImageNavigation = i.ItemImageNavigation,



                                                    ItemPrice = i.ItemPriceNs
                                            .Select(p => new
                                            {
                                                p.Price,
                                                p.CountryId,
                                                p.ItemId,
                                            })

                                                }).ToList(),
                                        }).ToListAsync();

                    double TotalCostOfAllItems = ItemsTotalCostPerUser(1);


                    if (Cart is null)
                    {
                        return NotFound();
                    }

                    return Ok(new { Status = true, Cart, TotalCostOfAllItems, Message = "Process completed successfully" });
                }

                return Ok(new { Status = false, Message = "Cart is empty" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }
        }


        //--------------------------------Cart APIs-----------------------------------//
        [HttpPost]
        [Route("addItemToCart/{itemId:int}/{Quantity:int}")]
        public async Task<IActionResult> addItemToCart(int itemId, int Quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var UserId = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email)
                                .FirstOrDefault()
                                .CustomerId;

                var DbUserCart = await _perfumeContext.ShoppingCart.AnyAsync(a => a.ItemId == itemId
                                         && a.CustomerId == UserId);

                if (DbUserCart)
                {
                    var UserItem = await _perfumeContext.ShoppingCart
                                            .FirstOrDefaultAsync(a => a.ItemId == itemId);

                    UserItem.ItemQty += Quantity;

                    UserItem.ItemTotal += Quantity * UserItem.ItemPrice;

                    _perfumeContext.SaveChanges();

                    return Ok(new { Status = true, Message = "Quantity Updated Successfully" });
                }
                else
                {
                    var Item = _perfumeContext.Item.Where(e => e.ItemId == itemId).FirstOrDefault();

                    if (Item is null)
                    {
                        return Ok(new { Status = false, Message = "Item Not Found", isLogin = true });

                    }

                    double ItemPrice = GetItemPrice(itemId);

                   
                    var CartItem = new ShoppingCart()
                    {
                        CustomerId = UserId,
                        ItemId = itemId,
                        ItemPrice = ItemPrice,
                        ItemQty = Quantity,
                        ItemTotal = Quantity * ItemPrice,
                       
                    };


                    _perfumeContext.ShoppingCart.Add(CartItem);
                    _perfumeContext.SaveChanges();

                    return Ok(new { Status = true, Message = "Item Added Successfully" });
                }

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }
        }

        [HttpPut]
        [Route("UpdateItemQuantityInCart/{itemId:int}/{Quantity:int}")]
        public async Task<IActionResult> UpdateItemQuantityInCart(int itemId, int Quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var UserId = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email).FirstOrDefault().CustomerId;

                var DbUserCart = await _perfumeContext.ShoppingCart.AnyAsync(a => a.ItemId == itemId
                                && a.CustomerId == UserId);

                if (DbUserCart)
                {
                    var UserItem = await _perfumeContext.ShoppingCart.FirstOrDefaultAsync(a => a.ItemId == itemId&& a.CustomerId == UserId);

                    UserItem.ItemQty = Quantity;

                    UserItem.ItemTotal = Quantity * UserItem.ItemPrice;
                    _perfumeContext.Attach(UserItem).State = EntityState.Modified;
                    _perfumeContext.SaveChanges();

                    double TotalPriceofAllItems = ItemsTotalCostPerUser(UserId);

                    return Ok(new { Status = true, UserItem.ItemTotal, TotalPriceofAllItems, Message = "Quantity Updated Successfully" });
                }
                else
                {
                    return Ok(new { Status = false, Message = "Item does not exists!" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }
        }


        [HttpDelete]
        [Route("DeleteItemFromShoppingCart/{itemId:int}")]
        public async Task<IActionResult> DeleteItemFromShoppingCart(int itemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var Item = _perfumeContext.ShoppingCart.Where(e => e.ItemId == itemId).FirstOrDefault();

                if (Item == null)
                {
                    return Ok(new { Status = false, Message = "Item Not Found", isLogin = true });

                }

                var UserId = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email)
                                                    .FirstOrDefault()
                                                    .CustomerId;

                var cartItem = _perfumeContext.ShoppingCart
                                    .Where(e => e.ItemId == itemId && e.CustomerId == UserId)
                                    .FirstOrDefault();

                if (cartItem == null)
                {
                    return Ok(new { Status = false, Message = "Item Not Found in shopping cart", isLogin = true });

                }

                _perfumeContext.ShoppingCart.Remove(cartItem);
                _perfumeContext.SaveChanges();

                return Ok(new { Status = true, Message = "Item Removed Successfully From shopping cart" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }
        }


        [HttpDelete]
        [Route("RemoveAllItemsFromShoppingCart")]
        public async Task<IActionResult> RemoveAllItemsFromShoppingCart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }



                var customer = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email).FirstOrDefault();
                if (customer == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });

                }
                var UserId = customer.CustomerId;

                var shoppingCartList = _perfumeContext.ShoppingCart.Where(e => e.CustomerId == UserId).ToList();

                if (shoppingCartList == null)
                {
                    return Ok(new { Status = false, Message = "No Item In Cart", isLogin = true });

                }
                _perfumeContext.ShoppingCart.RemoveRange(shoppingCartList);
                _perfumeContext.SaveChanges();
                return Ok(new { Status = true, Message = "Items Removed Successfully From shopping cart" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }
        }



        [HttpGet]
        [Route("ShowCartList")]
        public async Task<ActionResult<IEnumerable<ShoppingCart>>> ShowCartList()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var UserId = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email).FirstOrDefault().CustomerId;

                var DbUserCart = await _perfumeContext.ShoppingCart.AnyAsync(a => a.CustomerId == UserId);

                if (DbUserCart)
                {
                    var Cart = await _perfumeContext.ShoppingCart.Where(a => a.CustomerId == UserId)
                                        .Select(a => new
                                        {
                                            ShoppingCartId = a.ShoppingCartId,
                                            ItemId = a.ItemId,
                                            CustomerId = a.CustomerId,
                                            ItemPrice = a.ItemPrice,
                                            ItemQty = a.ItemQty,
                                            ItemTotal = a.ItemTotal,

                                            ItemImageAndTitle = _perfumeContext.Item
                                                .Where(i => i.ItemId == a.ItemId)
                                                .Select(m => new
                                                {
                                                    ItemImage = m.ItemImage,
                                                    ItemTitleAr = m.ItemTitleAr,
                                                    ItemTitleEn = m.ItemTitleEn
                                                }).ToList(),

                                            IsFavorite = _perfumeContext.FavouriteItems
                                            .Any(f => f.ItemId == a.ItemId && f.UserId == user.Id)
                                        }).ToListAsync();

                    double TotalCostOfAllItems = ItemsTotalCostPerUser(UserId);

                    if (Cart is null)
                    {
                        return NotFound();
                    }

                    return Ok(new { Status = true, Cart, TotalCostOfAllItems, Message = "Process completed successfully" });
                }

                return Ok(new { Status = false, Message = "Cart is empty" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }
        }


        /*========================================Customer APIs===========================================*/


        [HttpPost]
        [Route("AddCustomerAddress")]
        public async Task<IActionResult> AddCustomerAddress( CustomerAddressVM customerAddressVM)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }
                var customerObj = _perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });

                }



                var customerAddress = new CustomerAddress()
                {

                    CustomerId = customerObj.CustomerId,
                    Address = customerAddressVM.Address,
                    CountryId = customerAddressVM.CountryId,
                    CityName = customerAddressVM.CityName,
                    AreaName = customerAddressVM.AreaName,
                    BuildingNo = customerAddressVM.BuildingNo,
                    Mobile = customerAddressVM.Mobile,
                };

                _perfumeContext.customerAddresses.Add(customerAddress);
                _perfumeContext.SaveChanges();

                return Ok(new { Status = true, Message = "Address Added Successfully" });
            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });

            }

        }


        [HttpPut]
        [Route("EditCustomerAddress")]
        public async Task<IActionResult> EditCustomerAddress( EditCustomerAddressVM editCustomerAddressVM)
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }
                var customerObj = _perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });

                }
                var customerAddressObj = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == editCustomerAddressVM.CustomerAddressId).FirstOrDefault();
                if (customerAddressObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Address Object Not Found" });

                }
                customerAddressObj.Address = editCustomerAddressVM.Address;
                customerAddressObj.CountryId = editCustomerAddressVM.CountryId;
                customerAddressObj.CityName = editCustomerAddressVM.CityName;
                customerAddressObj.AreaName = editCustomerAddressVM.AreaName;
                customerAddressObj.BuildingNo = editCustomerAddressVM.BuildingNo;
                customerAddressObj.Mobile = editCustomerAddressVM.Mobile;


                _perfumeContext.Attach(customerAddressObj).State = EntityState.Modified;
                _perfumeContext.SaveChanges();
                return Ok(new { Status = true, Message = "Address Edited Successfully" });
            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, message = ex.Message });

            }

        }

       
        [HttpDelete]
        [Route("DeleteCustomerAddress/{customerAddressId:int}")]
        public async Task<IActionResult> DeleteCustomerAddress(int customerAddressId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var customerAddr = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == customerAddressId).FirstOrDefault();

                if (customerAddr == null)
                {
                    return Ok(new { Status = false, Message = "Address Not Found", isLogin = true });

                }


                _perfumeContext.customerAddresses.Remove(customerAddr);
                _perfumeContext.SaveChanges();

                return Ok(new { Status = true, Message = "Address Removed Successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }
        }
       
        
        [HttpGet]
        [Route("GetCustomerAddressById/{customerAddressId:int}")]
        public async Task<IActionResult> GetCustomerAddressById(int customerAddressId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var customerAddr = _perfumeContext.customerAddresses.Where(e => e.CustomerAddressId == customerAddressId).Select(c => new
                {
                    CountryId = c.CountryId,
                    CityName = c.CityName,
                    AreaName = c.AreaName,
                    Address = c.Address,
                    Mobile = c.Mobile,
                    BuildingNo = c.BuildingNo,
                    CustomerAddressId = c.CustomerAddressId,
                    CustomerId = c.CustomerId


                }).FirstOrDefault();

                if (customerAddr == null)
                {
                    return Ok(new { Status = false, Message = "Address Not Found", isLogin = true });

                }

                return Ok(new { Status = true, customerAddress = customerAddr });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });

            }

        }


        [HttpGet]
        [Route("GetAllCustomersAddress")]
        public async Task<IActionResult> GetAllCustomersAddress()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }
                var customerObj = _perfumeContext.CustomerNs.Where(e => e.Email == user.Email).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });

                }

                var customerAddressList = _perfumeContext.customerAddresses.Where(e => e.CustomerId == customerObj.CustomerId).ToList();
                return Ok(new { Status = true, CustomerAddressList = customerAddressList });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }


        }


        //=====================================Country API================================//

        [HttpGet]
        [Route("GetAllCountries")]
        public IActionResult GetAllCountries()
        {
            try
            {
                var countryList = _perfumeContext.Country.Include(e => e.Currency).Where(c => c.IsActive == true).OrderBy(c => c.OrderIndex)
                    .Select(c => new
                    {
                        CountryId = c.CountryId,
                        //CountryTlen = c.CountryTlen,
                        CountryTlen = c.CountryTlen,
                        Pic = c.Pic,
                        ShippingCost = c.ShippingCost,
                        CurrencyId = c.CurrencyId,
                        CurrencyTlar = c.Currency.CurrencyTlar,
                        CurrencyTlen = c.Currency.CurrencyTlen,
                       



                    });
                return Ok(new { Status = true, countryList = countryList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });

            }

        }
        [HttpGet]
        [Route("GetCountryById/{CountryId:int}")]
        public IActionResult GetCountryById(int CountryId)
        {
            try
            {
                var country = _perfumeContext.Country.Include(e => e.Currency).Where(c => c.CountryId == CountryId)
                    .Select(c => new
                    {
                        CountryId = c.CountryId,
                        //CountryTlen = c.CountryTlen,
                        CountryTlen = c.CountryTlen,
                        Pic = c.Pic,
                        ShippingCost = c.ShippingCost,
                        CurrencyId = c.CurrencyId,
                        CurrencyTlar = c.Currency.CurrencyTlar,
                        CurrencyTlen = c.Currency.CurrencyTlen,




                    }).FirstOrDefault();
                if (country == null)
                {
                    return Ok(new { Status = false, Message = "Country Obj Not Found" });
                }
                return Ok(new { Status = true, country = country });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });

            }

        }



        //==================================Copoun API==========================================//
        [HttpGet]
        [Route("ValidateCopoun")]
        public async Task<IActionResult> ValidateCopoun([FromQuery] string CouponSerial)
        {
            try
            {
                var coupon = await _perfumeContext.Coupon.FirstOrDefaultAsync(c => c.Serial == CouponSerial);
                if (coupon == null)
                {
                    return Ok(new { Status = false, Message = "Coupon Not Exist" });
                }
                if (
                    (DateTime.Now.Date >= coupon.IssueDate.Date) &&
                    (DateTime.Now.Date <= coupon.ExpirationDate.Date) &&
                    (coupon.Used != true))
                {
                    //coupon.Used = true;
                    //_perfumeContext.Attach(coupon).State = EntityState.Modified;
                    //_perfumeContext.SaveChanges();
                    //var user = await _userManager.GetUserAsync(User);
                    //if (user == null)
                    //{
                    //    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                    //}

                    //var customerObj = _perfumeContext.Customer.Where(e => e.Email == user.Email).FirstOrDefault();
                    //if (customerObj == null)
                    //{
                    //    return Ok(new { Status = false, Message = "Customer Object Not Found" });

                    //}
                    //var ShoppingCartCost = _perfumeContext.ShoppingCart.Where(e => e.CustomerId == customerObj.CustomerId).Sum(e => e.ItemTotal);
                    //var Total = ShoppingCartCost - coupon.Amount;
                    return Ok(new { Status = true, Message = "Coupon Applied", CouponId = coupon.CouponId, CouponAmount = coupon.Amount });
                }

                return Ok(new { Status = false, Message = "Coupon Not Exist" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }


        }
  

        //========================================Helper Functions==============================//
        private double ItemsTotalCostPerUser(int UserId)
        {
            var UserShoppingCart = _perfumeContext.ShoppingCart.Where(u => u.CustomerId == UserId);

            var UserTotalCost = UserShoppingCart.Sum(a => a.ItemTotal);

            return UserTotalCost;
        }

        private double GetItemPrice(int ItemId)
        {
            var DbItemId = _perfumeContext.itemPriceNs.FirstOrDefault(a => a.ItemId == ItemId);

            if (DbItemId is null)
            {
                return 0;
            }
            double? DbItemPrice = DbItemId.Price;

            return (double)DbItemPrice;
        }
        [HttpGet]
        [Route("GetAllPaymentMethods")]
        public IActionResult GetAllPaymentMethods()
        {
            try
            {
                var paymentMehodsList = _perfumeContext.paymentMehods

                .Select(c => new
                {
                    PaymentMethodId = c.PaymentMethodId,
                    PaymentMethodAR = c.PaymentMethodAR,
                    PaymentMethodEN = c.PaymentMethodEN,
                    PaymentMethodPic = c.PaymentMethodPic,

                });
                return Ok(new { Status = true, paymentMehodsList = paymentMehodsList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }
        }
        [HttpPost]
        [Route("CheckOut")]
        public async Task<IActionResult> CheckOut(CRM.ViewModels.CheckOutVM checkOutVM)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }

                var customer = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email).FirstOrDefault();
                if (customer == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });

                }
                var UserId = customer.CustomerId;

                var customerAddressObj = _perfumeContext.customerAddresses.FirstOrDefault(c => c.CustomerAddressId == checkOutVM.CustomerAddressId);
                if (customerAddressObj == null)
                {
                    return Ok(new { Status = false, Message = "Customer Address Object Not Found" });

                }
                var paymentObj = _perfumeContext.paymentMehods.FirstOrDefault(c => c.PaymentMethodId == checkOutVM.PaymentMethodId);
                if (paymentObj == null)
                {
                    return Ok(new { Status = false, Message = "Payment Object Not Found" });

                }
                var CountryObj = _perfumeContext.Country.Where(e => e.CountryId == checkOutVM.CountryId).FirstOrDefault();
                if (CountryObj == null)
                {
                    return Ok(new { Status = false, Message = "Country Object Not Found" });

                }

                double discount = 0;


                //Get Customer ShoppingCart Items List
                var customerShoppingCartList = _perfumeContext.
                    ShoppingCart.Include(s => s.CustomerN)
                    .Include(s => s.Item)
                    .Where(c => c.CustomerId == UserId);

                double shoppingCost = 0.0;

                shoppingCost = CountryObj.ShippingCost.Value;

                var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);

                // make coupon used

                Coupon coupon = null;
                coupon = _perfumeContext.Coupon.FirstOrDefault(c => c.CouponId == checkOutVM.CouponId);


                //calc ordernet
                double calcOrderNet(double sumItemTotal)
                {
                    var percent = sumItemTotal / totalOfAll;

                    if (coupon == null)
                    {
                        discount = 0;
                        return sumItemTotal;
                    }
                    else if (coupon.CouponTypeId == 2)
                    {
                        discount = sumItemTotal - (float)(sumItemTotal - coupon.Amount * percent);

                        return (float)(sumItemTotal - coupon.Amount * percent);
                    }
                    else
                    {
                        var couponAmount = totalOfAll * (coupon.Amount / 100);
                        discount = sumItemTotal - (float)(sumItemTotal - couponAmount * percent);

                        return (float)(sumItemTotal - couponAmount * percent);
                    }

                }

                int maxUniqe = 1;
                var newList = _perfumeContext.Order.ToList();
                if (newList != null)
                {
                    if (newList.Count > 0)
                    {
                        maxUniqe = newList.Max(e => e.UniqeId).Value;
                    }
                }


                var orders =
                new Order
                {
                    OrderDate = DateTime.Now,
                    OrderSerial = Guid.NewGuid().ToString() + "/" + DateTime.Now.Year,
                    CustomerId = UserId,
                    CustomerAddressId = checkOutVM.CustomerAddressId,
                    OrderTotal = totalOfAll,
                    CouponId = coupon != null ? checkOutVM.CouponId : null,
                    CouponTypeId = coupon != null ? coupon.CouponTypeId : null,
                    CouponAmount = coupon != null ? (float?)coupon.Amount : null,
                    Deliverycost = shoppingCost,
                    OrderNet = totalOfAll + shoppingCost,
                    PaymentMethodId = checkOutVM.PaymentMethodId,
                    OrderDiscount = discount,
                    IsCanceled = false,
                    OrderStatusId = 1,
                    CountryId = CountryObj.CountryId,
                    UniqeId = maxUniqe + 1,
                    IsDeliverd = false,
                

                };



                    _perfumeContext.Order.Add(orders);
                    _perfumeContext.SaveChanges();

                    //transfer shoppingcart to orderitems table and clear shoppingcart

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

                        _perfumeContext.OrderItem.Add(orderItem);


                    }

                var TrakingOrderObj = new OrderTraking()
                {
                    OrderId = orders.OrderId,
                    OrderStatusId = 1,
                    ActionDate = DateTime.Now,
                    Remarks = "Order Initiated"
                };
                _perfumeContext.OrderTrakings.Add(TrakingOrderObj);
                   
                _perfumeContext.SaveChanges();

                if (checkOutVM.PaymentMethodId == 1)
                {

                    bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                    var TestToken = _configuration["TestToken"];
                    var LiveToken = _configuration["LiveToken"];
                    if (Fattorahstatus) // fattorah live
                    {
                        var sendPaymentRequest = new
                        {
                            CustomerName = customer.CustomerName,
                            NotificationOption = "LNK",
                            InvoiceValue = orders.OrderNet,
                            CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderSuccess",
                            ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderFaild",
                            UserDefinedField = orders.OrderId,
                            CustomerEmail = customer.Email
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://api.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);

                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                            return Ok(new { Status = true, Url = InvoiceRes.InvoiceURL });
                        }
                        else
                        {
                            return Ok(new { Status = false, Message = FattoraRes.Message });
                        }
                    }
                    else               // fattorah test
                    {
                        var sendPaymentRequest = new
                        {
                            CustomerName = customer.CustomerName,
                            NotificationOption = "LNK",
                            InvoiceValue = orders.OrderNet,
                            CallBackUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderSuccess",
                            ErrorUrl = "http://basamalabdan-001-site4.ctempurl.com/FattorahOrderFaild",
                            UserDefinedField = orders.OrderId,
                            CustomerEmail = customer.Email
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);
                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();

                            return Ok(new { Status = true, Url = InvoiceRes.InvoiceURL });

                        }
                        else
                        {
                            return Ok(new { Status = false, Message = FattoraRes.Message });
                        }
                    }


                }
                else
                {

                    if (orders.CustomerId != null)
                    {
                        if (coupon != null)
                        {
                            coupon.Used = true;
                            var UpdatedCoupon = _perfumeContext.Coupon.Attach(coupon);
                            UpdatedCoupon.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }

                        _perfumeContext.ShoppingCart.RemoveRange(customerShoppingCartList);
                        _perfumeContext.SaveChanges();
                        return Ok(new { Status = true, Url = "http://basamalabdan-001-site4.ctempurl.com/Thankyou" });

                    }

                }
                return Ok();

            }

            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });

            }

        }
        [HttpGet]
        [Route("GetOrderTraking")]
        public async Task<IActionResult> GetOrderTraking()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });
                }



                var customer = _perfumeContext.CustomerNs.Where(a => a.Email == user.Email).FirstOrDefault();
                if (customer == null)
                {
                    return Ok(new { Status = false, Message = "Must Be Login First", isLogin = false });

                }
               
                var UserId = customer.CustomerId;
                var LatestOrder = _perfumeContext.Order.Where(e => e.CustomerId == UserId).Include(e=>e.CustomerAddress).OrderByDescending(e => e.OrderId).FirstOrDefault();
                if (LatestOrder == null)
                {
                    return Ok(new { Status = false, Message = "Not Found Any Order ", isLogin = true });

                }
                var countryOrder = _perfumeContext.Country.Where(e => e.CountryId == LatestOrder.CountryId).Include(e=>e.Currency).FirstOrDefault();
                var orderTrakingList  = _perfumeContext.OrderTrakings.Include(e => e.Order).Include(e => e.OrderStatus).Where(c => c.OrderId == LatestOrder.OrderId)
                    .Select(c => new
                    {
                        OrderSerial = c.Order.OrderSerial,
                        CustomerName = customer.CustomerName,
                        //Country = countryOrder.CountryTlen,
                        Address = LatestOrder.CustomerAddress.Address,
                        OrderTotal = c.Order.OrderNet+" "+countryOrder.Currency.CurrencyTlen,
                        Status = c.OrderStatus.Status,
                        Remarks = c.Remarks,
                        ActionDate = c.ActionDate.ToShortDateString(),
                        ActionTime = c.ActionDate.TimeOfDay,
                    });
                return Ok(new { Status = true, orderTrakingList = orderTrakingList, isLogin = true });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message, isLogin = true });

            }

        }

    }

}


