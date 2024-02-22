using CRM.Models;

namespace CRM.ViewModels
{
    public class ProductsVM
    {
        public int ItemId { get; set; }
        public int Country { get; set; }

        public bool OutOfStock { get; set; }
        public string? ItemImage { get; set; }
        public string? ItemTitleAr { get; set; }
        public string? ItemTitleEn { get; set; }
        public double? price { get; set; }
        public double? Beforeprice { get; set; }
        public int Stock { get; set; }
        public double? Currency { get; set; }
        public string? CurrencyAR { get; set; }
        public bool FavorateItem { get; set; }
        public string? CurrencyEN { get; set; }




    }
}
