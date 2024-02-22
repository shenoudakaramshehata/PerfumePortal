using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CRM.Areas.Admin.Pages.Configurations.ManageItem
{

#nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public List<Category> CategoryList { get; set; }

        public string url { get; set; }


        [BindProperty]
        public Item item { get; set; }
        [BindProperty]
        public List<ItemPriceN> itemPriceN { get; set; }
        public List<Item> itemlist = new List<Item>();
        public Item itemObject { get; set; }


        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            item = new Item();
            itemObject = new Item();
            itemPriceN = new List<ItemPriceN>();
        }
        public void OnGet()
        {
            itemlist = _context.Item.ToList();
            url = $"{this.Request.Scheme}://{this.Request.Host}";
            CategoryList = _context.Categories.Where(e => e.IsActive == true).ToList();

        }
        //done
        public IActionResult OnGetSingleItemForEdit(int itemId)
        {
            item = _context.Item.Where(c => c.ItemId == itemId).FirstOrDefault();

            return new JsonResult(item);

        }


        public IActionResult OnGetSingleItemForView(int ItemId)
        {
            //itemPriceN = _context.itemPriceNs.Include(e=>e.Item).Include(e => e.Country).Where(c => c.ItemId == ItemId).ToList();
            //var Result = _context.Item.Where(c => c.ItemId == ItemId).FirstOrDefault();
            var ItemPrice = _context.itemPriceNs.Where(c => c.ItemId == ItemId).ToList();
            var Country = _context.Country.Select(e => new { e.CountryTlen, e.CountryId }).ToList();
            var item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();
            Object obj = new { ItemPrice, Country, item };
            return new JsonResult(obj);
        }

        //done
        public IActionResult OnGetSingleItemForDelete(int itemId)
        {
            item = _context.Item.Where(c => c.ItemId == itemId).FirstOrDefault();
            return new JsonResult(item);

        }

        public async Task<IActionResult> OnPostDeleteItem(Models.Item item)
        {
            try
            {
                Item _assetDocument = _context.Item.Where(e => e.ItemId == item.ItemId).FirstOrDefault();
                var imgList = _context.ItemImage.Where(c => c.ItemId == item.ItemId).ToList();
                var itemPrice = _context.itemPriceNs.Where(c => c.ItemId == item.ItemId).ToList();
                var ImagePathv = Path.Combine(_hostEnvironment.WebRootPath, "/" + _assetDocument.ItemImage);
                var orderItem = _context.OrderItem.Where(e => e.ItemId == item.ItemId).ToList();
                var shoppingitem = _context.ShoppingCart.Where(e => e.ItemId == item.ItemId).ToList();
               
                if (_assetDocument != null)
                {
                    if (imgList.Count > 0)
                    {
                        _context.ItemImage.RemoveRange(imgList);

                    }
                    if (itemPrice.Count > 0)
                    {
                        _context.itemPriceNs.RemoveRange(itemPrice);

                    }
                    if (orderItem.Count > 0)
                    {
                        _context.OrderItem.RemoveRange(orderItem);
                    }
                    if (shoppingitem.Count > 0)
                    {
                        _context.ShoppingCart.RemoveRange(shoppingitem);
                    }
                    if (shoppingitem.Count > 0)
                    {
                        _context.ShoppingCart.RemoveRange(shoppingitem);
                    }
                    foreach (var orderIdForOrderItem in orderItem)
                    {
                        var Order = _context.Order.Where(e => e.OrderId == orderIdForOrderItem.OrderId).ToList();
                       
                        foreach(var ordertracking in Order)
                        {
                            var OrderTracking = _context.OrderTrakings.Where(e => e.OrderId == ordertracking.OrderId).ToList();
                            if (OrderTracking.Count > 0)
                            {
                                _context.OrderTrakings.RemoveRange(OrderTracking);
                            }
                        }
                        
                        if (Order.Count > 0)
                        {
                            _context.Order.RemoveRange(Order);
                        }
                    }
                    _context.SaveChanges();
                    _context.Item.Remove(_assetDocument);
                    await _context.SaveChangesAsync();

                    if (System.IO.File.Exists(ImagePathv))
                    {
                        System.IO.File.Delete(ImagePathv);
                    }
                    _context.SaveChanges();

                    var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                    var BrowserCulture = locale.RequestCulture.UICulture.ToString();
                    if (BrowserCulture == "en-US")

                        _toastNotification.AddSuccessToastMessage("Item Request Deleted successfully");

                    else
                        _toastNotification.AddSuccessToastMessage("تم مسح العنصر  بنجاح");
                }


            }
            catch (Exception)

            {
                var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var BrowserCulture = locale.RequestCulture.UICulture.ToString();
                if (BrowserCulture == "en-US")
                    _toastNotification.AddErrorToastMessage("Something went wrong");
                else
                    _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
                return Redirect("/Admin/Configurations/ManageItem/Index");

            }

            return Redirect("/Admin/Configurations/ManageItem/Index");
        }
        //done
        //public async Task<IActionResult> OnPostDeleteItem(int itemId)
        //{
        //    try
        //    {
        //        Item ItemObj = _context.Item.Where(e => e.ItemId == itemId).FirstOrDefault();


        //        if (ItemObj != null)
        //        {


        //            _context.Item.Remove(ItemObj);
        //            await _context.SaveChangesAsync();
        //            _toastNotification.AddSuccessToastMessage("Item Deleted successfully");
        //            var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "/" + item.ItemImage);
        //            if (System.IO.File.Exists(ImagePath))
        //            {
        //                System.IO.File.Delete(ImagePath);
        //            }
        //        }
        //        else
        //        {
        //            _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
        //        }
        //    }
        //    catch (Exception)

        //    {
        //        _toastNotification.AddErrorToastMessage("Something went wrong");
        //    }

        //    return Redirect("/Admin/Configurations/ManageItem/Index");
        //}


        public async Task<IActionResult> OnPostAddItem(IFormFile file, IFormFileCollection MorePhoto)
        {

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                return Redirect("/Admin/Configurations/ManageItem/Index");
            }
            try
            {
                if (file != null)
                {
                    string folder = "Images/Item/";
                    item.ItemImage = UploadImage(folder, file);
                }

                List<ItemImage> itemImagesList = new List<ItemImage>();

                if (file != null)
                {
                    string folder = "Images/Item/";
                    item.ItemImage = UploadImage(folder, file);
                }
                if (MorePhoto.Count != 0)
                {
                    foreach (var item in MorePhoto)
                    {
                        var itemImageObj = new ItemImage();
                        string folder = "Images/Item/";
                        itemImageObj.ImageName = UploadImage(folder, item);
                        itemImagesList.Add(itemImageObj);


                    }
                    item.ItemImageNavigation = itemImagesList;
                }
               
                _context.Item.Add(item);

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("item Added Successfully");

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/Admin/Configurations/ManageItem/Index");
        }

        public async Task<IActionResult> OnPostEditItem(int ItemId, IFormFile Editfile, IFormFileCollection Editfilepond)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                    return Redirect("/Admin/Configurations/ManageItem/Index");
                }
                var DbItem = _context.Item.Where(c => c.ItemId == ItemId).FirstOrDefault();

                if (DbItem == null)
                {
                    _toastNotification.AddErrorToastMessage("Item Not Found");

                    return Redirect("/Admin/Configurations/ManageItem/Index");
                }
                if (Editfile != null)
                {


                    string folder = "Images/Item/";

                    DbItem.ItemImage = UploadImage(folder, Editfile);
                }
                else
                {
                    DbItem.ItemImage = item.ItemImage;
                }

                List<ItemImage> EditItemImagesList = new List<ItemImage>();

                if (Editfilepond.Count != 0)
                {

                    var ImagesOfItem = _context.ItemImage.Where(i => i.ItemId == ItemId).ToList();
                    _context.RemoveRange(ImagesOfItem);

                    foreach (var item in Editfilepond)
                    {

                        var itemImageObj = new ItemImage();
                        string folder = "Images/Item/";
                        itemImageObj.ImageName = UploadImage(folder, item);
                        itemImageObj.ItemId = ItemId;
                        EditItemImagesList.Add(itemImageObj);


                    }
                    _context.ItemImage.AddRange(EditItemImagesList);
                }

                DbItem.ItemTitleAr = item.ItemTitleAr;

                DbItem.ItemTitleEn = item.ItemTitleEn;

                DbItem.ItemDescriptionAr = item.ItemDescriptionAr;

                DbItem.ItemDescriptionEn = item.ItemDescriptionEn;

                DbItem.IsActive = item.IsActive;

                DbItem.OrderIndex = item.OrderIndex;

                DbItem.OutOfStock = item.OutOfStock;

                DbItem.Weight = item.Weight;

                DbItem.Stock = item.Stock;
                DbItem.ItemPriceNs = item.ItemPriceNs;

                DbItem.CategoryId = item.CategoryId;

                DbItem.OrderItem = item.OrderItem;

                var UpdatedItem = _context.Item.Attach(DbItem);

                UpdatedItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Item Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Configurations/ManageItem/Index");
        }


        private string UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }

    }
}