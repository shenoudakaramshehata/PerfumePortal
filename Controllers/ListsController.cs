using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using CRM.ViewModels;
using CRM.Migrations;

namespace CRM.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ListsController : Controller
    {
        private PerfumeContext _context;

        public ListsController(PerfumeContext context)
        {
            _context = context;
        }


        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var data = await _context.Categories.Select(i => new
            {
                CategoryTLAR = i.CategoryTlar,
                CategoryTLEN = i.CategoryTlen,
                CategoryPic = i.CategoryPic,
                DescriptionTLAR = i.DescriptionTlar,
                DescriptionTLEN = i.DescriptionTlen,
                CategoryId = i.CategoryId,
                SortOrder = i.SortOrder,
                IsActive = i.IsActive
            }).ToListAsync();


            return Ok(new { data });
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            var data = await _context.Currency.Select(i => new
            {
                CurrencyId = i.CurrencyId,
                CurrencyTlar = i.CurrencyTlar,
                CurrencyTlen = i.CurrencyTlen,
                CurrencyPic = i.CurrencyPic,
                IsActive = i.IsActive
            }).ToListAsync();


            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingMatrix>>> GetShippingMatrix()
        {
            var data = await _context.ShippingsMatrix.Include(a => a.Country).Select(i => new
            {
                ShippingMatrixId = i.ShippingMatrixId,
                ActualPrice = i.ActualPrice,
                AramexPrice = i.AramexPrice,
                FromWeight = i.FromWeight,
                ToWeight = i.ToWeight,
                Country = i.Country.CountryTlar,
                CountryId = i.CountryId,
            }).ToListAsync();


            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {


            var data = await _context.Country.Include(a => a.Currency).Select(i => new
            {
                Pic = i.Pic,
                CountryTlen = i.CountryTlen,
                ShippingCost = i.ShippingCost,
                CurrencyTlen = i.Currency.CurrencyTlen,
                CountryId = i.CountryId,

            }).ToListAsync();

            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {


            var data = await _context.Item.Select(i => new
            {
                ItemId = i.ItemId,
                ItemTLAR = i.ItemTitleAr,
                ItemTLEN = i.ItemTitleEn,
                ItemPic = i.ItemImage,
                IsActive = i.IsActive,
                OrderIndex = i.OrderIndex,
                OutOfStock = i.OutOfStock,
                Weight = i.Weight

            }).ToListAsync();


            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {


            var data = await _context.Order.Include(a => a.OrderStatus).Select(i => new
            {
                OrderId =i.OrderId,
                OrderNet = i.OrderNet,
                OrderDate = i.OrderDate.ToShortDateString(),
                Status = i.OrderStatus.Status,
                CustomerName = _context.CustomerNs.Where(a => a.CustomerId == i.CustomerId).FirstOrDefault().CustomerName

            }).ToListAsync();
          

            return Ok(new { data });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetStoredOrders()
        {

            var data = await _context.Order.Where(i => i.OrderStatusId==6).Select(i => new
            {
                OrderId = i.OrderId,
                OrderNet = i.OrderNet,
                OrderDate = i.OrderDate,
                CustomerName = _context.CustomerNs.Where(a => a.CustomerId == i.CustomerId).FirstOrDefault().CustomerName

            }).ToListAsync();


            return Ok(new { data });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetPackedOrders()
        {

            var data = await _context.Order.Where(i => i.OrderStatusId == 7).Select(i => new
            {
                OrderId = i.OrderId,
                OrderNet = i.OrderNet,
                OrderDate = i.OrderDate,
                CustomerName = _context.CustomerNs.Where(a => a.CustomerId == i.CustomerId).FirstOrDefault().CustomerName

            }).ToListAsync();


            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOperatedOrders()
        {
            var data = await _context.Order.Where(i => i.OrderStatusId == 2).Select(i => new
            {
                OrderId = i.OrderId,
                OrderNet = i.OrderNet,
                OrderDate = i.OrderDate.ToShortDateString(),
                CustomerName = _context.CustomerNs.Where(a => a.CustomerId == i.CustomerId).FirstOrDefault().CustomerName
            }).ToListAsync();

            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
        {


            var data = await _context.Coupon.Include(a => a.CouponType).Select(i => new
            {
                CouponId = i.CouponId,
                Serial = i.Serial,
                ExpirationDate = i.ExpirationDate.ToShortDateString(),
                IssueDate = i.IssueDate.ToShortDateString(),
                Amount = i.Amount,
                CouponTypeEN = i.CouponType.CouponTypeEN,
                Used = i.Used
            }).ToListAsync();


            return Ok(new { data });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PageContent>>> GetPageContent()
        {


            var data = await _context.PageContents.Select(i => new
            {
                PageContentId = i.PageContentId,
                PageTitleAr = i.PageTitleAr,
                ContentAr = i.ContentAr,
                PageTitleEn = i.PageTitleEn,
                ContentEn = i.ContentEn
            }).ToListAsync();


            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemPrice>>> GetItemPrice()
        {

            var data = await _context.itemPriceNs.Include(a => a.Country).Include(a => a.Item).Select(i => new
            {
                CountryTlen = i.Country.CountryTlen,
                ItemTitleEn = i.Item.ItemTitleEn,
                Price = i.Price,
                ItemPriceId = i.ItemPriceId,
                ShippingPrice = i.ShippingPrice

            }).ToListAsync();

            return Ok(new { data });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Newsletter>>> GetNewsLetter()
        {
            var data = await _context.Newsletters.Select(i => new
            {
                Email = i.Email,
                Date = i.Date.Value.ToLongDateString()
            }).ToListAsync();

            return Ok(new { data });
        }

        //[HttpGet]
        //public async  Task<IActionResult> CategoryList()
        //{

        //    var catList = _context.Categories.Select(i => new
        //    {
        //        CategoryTLAR = i.CategoryTLAR,
        //        CategoryTLEN = i.CategoryTLEN,
        //        CategoryPic = i.CategoryPic,
        //        DescriptionTLAR = i.DescriptionTLAR,
        //        DescriptionTLEN = i.DescriptionTLEN,
        //        CategoryId = i.CategoryId,
        //        IsActive = i.IsActive
        //    });

        //    return await catList.ToList();
        //}
        [HttpGet]
        public object GetImagesForItem([FromQuery] int id)
        {
            var productimages = _context.ItemImage.Where(p => p.ItemId == id)
                                .Select(i => new
                                {
                                    i.ItemImageId,
                                    i.ImageName,
                                    i.ItemId
                                });

            return productimages;
        }
        [HttpPost]
        public async Task<int> RemoveImageById([FromQuery] int id)
        {
            var itemPic = await _context.ItemImage.FirstOrDefaultAsync(p => p.ItemImageId == id);
            _context.ItemImage.Remove(itemPic);
            _context.SaveChanges();
            return id;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocialMediaLink>>> GetSocialLinks()
        {
            var data = await _context.SocialMediaLinks.Select(i => new
            {
                Facebook = i.Facebook,
                Twitter = i.Twitter,
                Instgram = i.Instgram,
                LinkedIn = i.LinkedIn,
                Fax = i.Fax,
                Address = i.Address,
                SocialMediaLinkId = i.SocialMediaLinkId,

            }).ToListAsync();


            return Ok(new { data });
        }

        public async Task<ActionResult<IEnumerable<SocialMediaLink>>> GetMessages()
        {
            var data = await _context.ContactUs.Select(i => new
            {
                FullName = i.FirstName + " " + i.LastName,
                TransDate = i.TransDate.Value.ToShortDateString(),
                Mobile = i.Mobile,
                Email = i.Email,
                ContactId = i.ContactId,
                Msg = i.Msg

            }).ToListAsync();


            return Ok(new { data });
        }

        public async Task<ActionResult<IEnumerable<CouponType>>> GetCouponTypes()
        {
            var data = await _context.CouponTypes.Select(i => new
            {

                CouponTypeId = i.CouponTypeId,
                CouponTypeAR = i.CouponTypeAR,
                CouponTypeEN = i.CouponTypeEN,

            }).ToListAsync();


            return Ok(new { data });
        }

        [HttpPost]
        public IActionResult GetSingleOrderDetailsForView(int OrderId)
        {
            var order = _context.Order.Where(e => e.OrderId == OrderId).Select(i => new Order
            {
                OrderId = i.OrderId
            })
            .FirstOrDefault();
            //return new JsonResult(order);
            return PartialView("_OrderDetailsModal", order);
        }
    }
}