namespace CRM.ViewModels
{
    public class TabbyOrderHistory
    {
        public string purchased_at { get; set; }
        public double amount { get; set; }
        public string payment_method { get; set; }
        public string status { get; set; }
        public buyer buyer { get; set; }
        public shipping_addressTabby shipping_address { get; set; }
        public List<TabbyOrderITems> items { get; set; }
    }
}
