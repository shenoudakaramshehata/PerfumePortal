using CRM.Services.TabbyModels;

namespace CRM.Models
{
    public class TabbyResponse
    {
        public string Id { get; set; }
        public Configuration Configuration { get; set; }
        public string ApiUrl { get; set; }
        public string Flow { get; set; }
        public Payment Payment { get; set; }
        public string status { get; set; }
        public Customer Customer { get; set; }
        public MerchantUrls MerchantUrls { get; set; }
        public string ProductType { get; set; }
        public string Lang { get; set; }
        public string Locale { get; set; }
        public string SeonSessionId { get; set; }
        public Merchant Merchant { get; set; }
        public string MerchantCode { get; set; }
        public bool TermsAccepted { get; set; }
        public object Promo { get; set; }
        public InstallmentPlan InstallmentPlan { get; set; }
    }
}
