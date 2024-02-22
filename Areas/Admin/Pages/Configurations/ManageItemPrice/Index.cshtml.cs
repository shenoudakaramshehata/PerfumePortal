
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;

namespace CRM.Areas.Admin.Pages.Configurations.ManageItemPrice
{

#nullable disable
    public class IndexModel : PageModel
    {
        private PerfumeContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        [BindProperty]
        public ItemPriceN itemPrice { get; set; }
        public List<ItemPriceN> itemPriceList = new List<ItemPriceN>();

        public IndexModel(PerfumeContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            itemPrice = new ItemPriceN();
            //countryObj = new CRM.Models.Country();
        }
        public void OnGet()
        {
            itemPriceList = _context.itemPriceNs.ToList();

            var ItemList = _context.Item.Where(i => i.IsActive).ToList();
            //ViewData["selectCountryList"] = new SelectList(_context.Country.ToList(), nameof(Country.CountryId), nameof(Country.CountryTlen));
            //ViewData["SelectItemList"] = new SelectList(ItemList, nameof(Item.ItemId), nameof(Item.ItemTitleEn));
            url = $"{this.Request.Scheme}://{this.Request.Host}";
        }
        


        

        public IActionResult OnGetSingleItemPriceForEdit(int ItemPriceId)
        {

            //itemPrice = _context.ItemPrice.Where(c => c.ItemPriceId == ItemPriceId)
            //                              .Include(a => a.Country).Include(a => a.Item)
            //                              .FirstOrDefault();
            itemPrice = _context.itemPriceNs.Where(c => c.ItemPriceId == ItemPriceId).FirstOrDefault();

            return new JsonResult(itemPrice);

            //var Result = _context.itemPriceNs.Where(c => c.ItemPriceId == ItemPriceId).Include(a => a.Country).Include(a => a.Item).Select(i => new
            //{
            //    ItemPriceId = i.ItemPriceId,
            //    CountryTlen = i.Country.CountryTlen,
            //    ItemTitleEn = i.Item.ItemTitleEn,
            //    Price = i.Price,
            //    ShippingPrice= i.ShippingPrice

            //}).FirstOrDefault();
            //return new JsonResult(Result);

        }

        public async Task<IActionResult> OnPostEditItemPrice(int ItemPriceId)
        {
            try
            {
                var model = _context.itemPriceNs.Where(c => c.ItemPriceId == ItemPriceId).FirstOrDefault();
                if (model == null)
                {
                    return Redirect("/Admin/Configurations/ManageItemPrice/Index");
                }
               

                model.ItemId = itemPrice.ItemId;
                model.Price = itemPrice.Price;
                model.CountryId = itemPrice.CountryId;
                model.ShippingPrice = itemPrice.ShippingPrice;
                model.BeforePrice = itemPrice.BeforePrice;
                _context.Attach(model).State = EntityState.Modified;
                var UpdatedAsset = _context.itemPriceNs.Attach(model);
                UpdatedAsset.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var BrowserCulture = locale.RequestCulture.UICulture.ToString();
                if (BrowserCulture == "en-US")

                    _toastNotification.AddSuccessToastMessage("Item price edited successfully");
                else
                    _toastNotification.AddSuccessToastMessage("تم تعديل ثمن السعر بنجاح");


            }
            catch (Exception)
            {
                var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var BrowserCulture = locale?.RequestCulture.UICulture.ToString();
                if (BrowserCulture == "en-US")
                    _toastNotification.AddErrorToastMessage("Something went wrong");
                else
                    _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
                return Redirect("/Admin/Configurations/ManageItemPrice/Index");
            }
            return Redirect("/Admin/Configurations/ManageItemPrice/Index");


        }

        //public async Task<IActionResult> OnPostEditItemPrice(int ItemPriceId)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
        //            return Redirect("/Admin/Configurations/ManageItemPrice/Index");
        //        }

        //        var model = await _context.itemPriceNs.Where(c => c.ItemPriceId == ItemPriceId).FirstOrDefaultAsync();
        //        if (model == null)
        //        {
        //            _toastNotification.AddErrorToastMessage("Item is not Found");

        //            return Redirect("/Admin/Configurations/ManageItemPrice/Index");
        //        }
        //        var RepeatedRow = _context.itemPriceNs.Any(i => i.ItemId == itemPrice.ItemId && i.CountryId == itemPrice.CountryId && i.Price==itemPrice.Price);
        //        if (RepeatedRow)
        //        {
        //            _toastNotification.AddErrorToastMessage(" Entered Data is repeated ");
        //            return Redirect("/Admin/Configurations/ManageItemPrice/Index");
        //        }
        //        model.ItemId = itemPrice.ItemId;
        //        model.CountryId = itemPrice.CountryId;
        //        model.Price = itemPrice.Price;
        //        model.ShippingPrice = itemPrice.ShippingPrice;
        //        var UpdatedItemPrice = _context.itemPriceNs.Attach(model);
        //        UpdatedItemPrice.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        //        _context.SaveChanges();
        //        _toastNotification.AddSuccessToastMessage("item price Edited successfully");


        //    }
        //    catch (Exception)
        //    {
        //        _toastNotification.AddErrorToastMessage("Something went Error");

        //    }
        //    return Redirect("/Admin/Configurations/ManageItemPrice/Index");
        //}


        public IActionResult OnGetSingleItemPriceForView(int ItemPriceId)
        {
            var Result = _context.itemPriceNs.Where(c => c.ItemPriceId == ItemPriceId).FirstOrDefault();


            return new JsonResult(Result);
            //var Result = _context.itemPriceNs.Where(I => I.ItemPriceId == ItemPriceId).Include(a => a.Country).Include(a => a.Item).Select(i => new
            //{
            //    CountryTlen = i.Country.CountryTlen,
            //    ItemTitleEn = i.Item.ItemTitleEn,
            //    Price = i.Price,
            //    ItemPriceId = i.ItemPriceId,
            //    ShippingPrice= i.ShippingPrice,

            //}).FirstOrDefault();


            //return new JsonResult(Result);
        }


        public IActionResult OnGetSingleItemPriceForDelete(int ItemPriceId)
        {
            itemPrice = _context.itemPriceNs.Where(c => c.ItemPriceId == ItemPriceId).FirstOrDefault();
            return new JsonResult(itemPrice);
        }


        public async Task<IActionResult> OnPostDeleteItemPrice(int ItemPriceId)
        {
            try
            {
                ItemPriceN itemPriceObj =await  _context.itemPriceNs.Where(e => e.ItemPriceId == ItemPriceId).FirstOrDefaultAsync();


                if (itemPriceObj != null)
                {


                    _context.itemPriceNs.Remove(itemPriceObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Item price deleted successfully");
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

            return Redirect("/Admin/Configurations/ManageItemPrice/Index");
        }


        public IActionResult OnPostAddItemPrice()
        {
            try
            {
                //if (!ModelState.IsValid)
                //{

                //    _toastNotification.AddErrorToastMessage("Please Enter All Required Data");
                //    return Redirect("/Admin/Configurations/ManageItemPrice/Index");
                //}
                var RepeatedRow = _context.itemPriceNs.Any(i => i.ItemId == itemPrice.ItemId &&i.ShippingPrice== itemPrice.ShippingPrice && i.CountryId == itemPrice.CountryId);
                if (RepeatedRow)
                {
                    _toastNotification.AddErrorToastMessage(" Entered Data is repeated ");
                    return Redirect("/Admin/Configurations/ManageItemPrice/Index");
                }



                _context.itemPriceNs.Add(itemPrice);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Item Price Added Successfully");

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/Admin/Configurations/ManageItemPrice/Index");
        }

    }
}
