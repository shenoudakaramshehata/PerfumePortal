namespace CRM.ViewModels
{
    public class WishlistVm
    {
        public int FavouriteItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemImage { get; set; }
        public string ItemTitleEn { get; set; }
        public string ItemTitleAr { get; set; }
        public bool OutOfStock { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
