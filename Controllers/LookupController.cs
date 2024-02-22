using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Localization;

namespace CRM.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LookupController : Controller
    {
        private PerfumeContext _context;

        public LookupController(PerfumeContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CountriesLookup(DataSourceLoadOptions loadOptions)
        {
            

            
                var lookupEn = from i in _context.Country
                               orderby i.CountryTlen
                               select new
                               {
                                   Value = i.CountryId,
                                   Text = i.CountryTlen
                               };
                return Json(await DataSourceLoader.LoadAsync(lookupEn, loadOptions));
            
        }



        [HttpGet]
        public async Task<IActionResult> OrderStatusLookup(DataSourceLoadOptions loadOptions)
        {



            var lookupEn = from i in _context.OrderStatuses
                           orderby i.OrderStatusId
                           select new
                           {
                               Value = i.OrderStatusId,
                               Text = i.Status
                           };
            return Json(await DataSourceLoader.LoadAsync(lookupEn, loadOptions));

        }
        
        //    var lookupAr = from i in _context.Country
        //                   orderby i.CountryTlen
        //                   select new
        //                   {
        //                       Value = i.CountryId,
        //                       Text = i.CountryTlen
        //                   };
        //    return Json(await DataSourceLoader.LoadAsync(lookupAr, loadOptions));
        //}

        [HttpGet]
        public async Task<IActionResult> ItemsLookup(DataSourceLoadOptions loadOptions)
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();

            if (BrowserCulture == "en-US")
            {
                var lookupEn = from i in _context.Item
                               orderby i.ItemTitleEn
                               select new
                               {
                                   Value = i.ItemId,
                                   Text = i.ItemTitleEn
                               };
                return Json(await DataSourceLoader.LoadAsync(lookupEn, loadOptions));
            }
            var lookupAr = from i in _context.Item
                           orderby i.ItemTitleAr
                           select new
                           {
                               Value = i.ItemId,
                               Text = i.ItemTitleAr
                           };
            return Json(await DataSourceLoader.LoadAsync(lookupAr, loadOptions));
        }
    }
}