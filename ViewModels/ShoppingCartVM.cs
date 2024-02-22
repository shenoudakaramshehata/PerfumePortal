namespace CRM.ViewModels
{
    public class ShoppingCartVM
    {
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public double ItemPrice { get; set; }
        public int ItemQunatity { get; set; }
        public int ItemTotal { get; set; }
    }
}
