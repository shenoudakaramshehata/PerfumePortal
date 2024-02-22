namespace CRM.ViewModels
{
    public class EditCustomerAddressVM
    {
        public int CustomerAddressId { get; set; }
        public int CountryId { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string Address { get; set; }
        public string BuildingNo { get; set; }
        public string Mobile { get; set; }
    }
}
