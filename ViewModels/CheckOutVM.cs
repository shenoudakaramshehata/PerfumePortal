namespace CRM.ViewModels
{
    public class CheckOutVM
    {
       
        public int CustomerAddressId { get; set; }

        public int? CouponId { get; set; }

        public int PaymentMethodId { get; set; }
        public int CountryId { get; set; }
    }
}
