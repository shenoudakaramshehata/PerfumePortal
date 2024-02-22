namespace CRM.ViewModels
{
    public class OrderTrackingVm
    {
        public string OrderSerial { get; set; }
        public string CustomerName { get; set; }
        //public string Country { get; set; }
        public string Address { get; set; }
        public double OrderTotal { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string ActionDate { get; set; }
        //public string ActionTime { get; set; }
    }
}
