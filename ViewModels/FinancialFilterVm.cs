namespace CRM.ViewModels
{
    public class FinancialFilterVm
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerTele { get; set; }
        public string? OrderSerial { get; set; }
        public int? OrderStatusId { get; set; }
        public int? CountryId { get; set; }
        public int? ItemId { get; set; }

    }
}
