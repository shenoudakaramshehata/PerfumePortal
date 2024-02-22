namespace CRM.ViewModels
{
    public class CustomerVM
    {
        public int CustomerAddressId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public int? CountryId { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? Address { get; set; }
        public string? BuildingNo { get; set; }
        public string? Mobile { get; set; }
    }
}
