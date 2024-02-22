namespace CRM.ViewModels
{
    public class OrderVM
    {
        public DateTime OrderDate { get; set; }
        public double NetOrder { get; set; }
        public string CusAddress { get; set; }
        public string CusName { get; set; }
        public string Deliverd { get; set; }
        public string Cancelled { get; set; }
        public string Status { get; set; }
        public string Country { get; set; }
     

    }
}
