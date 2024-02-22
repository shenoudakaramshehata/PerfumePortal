using CRM.ViewModels;

namespace CRM.Models
{
    public class Configuration
    {
        public string Currency { get; set; }
        public string AppType { get; set; }
        public bool NewCustomer { get; set; }
        public string AvailableLimit { get; set; }
        public string MinLimit { get; set; }
        public available_products available_products { get; set; }
        public string Country { get; set; }
        public string ExpiresAt { get; set; }
        public bool IsBankCardRequired { get; set; }
        public string BlockedUntil { get; set; }
        public bool HideClosingIcon { get; set; }
        public string PosProvider { get; set; }
        public bool IsTokenized { get; set; }
        public string Disclaimer { get; set; }
        public string Help { get; set; }
        public Products Products { get; set; }
    }
}
