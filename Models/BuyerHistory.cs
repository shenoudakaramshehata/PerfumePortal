namespace CRM.Models
{
    public class BuyerHistory
    {
        public string RegisteredSince { get; set; }
        public int LoyaltyLevel { get; set; }
        public int WishlistCount { get; set; }
        public bool IsSocialNetworksConnected { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
