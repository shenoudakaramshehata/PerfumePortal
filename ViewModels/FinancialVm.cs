namespace CRM.ViewModels
{
    public class FinancialVm
    {
        public string? CustomerName { get; set; } 
        public string? CustomerTele { get; set; } 
        public string? CountryName { get; set; } 
        public int? CountryId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderSerial { get; set; }
        public double OrderTotal { get; set; }
    }
}
