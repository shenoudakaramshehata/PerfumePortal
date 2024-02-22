namespace CRM.ViewModels
{
    public class OrderFiltterModel
    {
        public DateTime OrderDate { get; set; }
        public double NetOrder { get; set; }
        public string CusAddress { get; set; }
        public string CusName { get; set; }
		public string Email { get; set; }

		public string Phone { get; set; }

		public string SerialNo { get; set; }
        public string Status { get; set; }
        public string Country { get; set; }
        public int CountryId { get; set; }
        public int statusId { get; set; }
        public int OrderId { get; set; }


    }
}
