namespace CRM.ViewModels
{
    public class ItemReportVm
    {
        public string? CountryName { get; set; }
        public int CountryId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? ItemName { get; set; }
        public int ItemId { get; set; }
        public double? ItemPrice { get; set; }
        public int? TotalQuantity { get; set; }
        public double? TotalAmount { get; set; }
    }
}
