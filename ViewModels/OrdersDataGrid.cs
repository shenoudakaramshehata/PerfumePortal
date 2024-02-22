namespace CRM.ViewModels
{
    public class OrdersDataGrid
    {
        public DateTime ORDERDATE { get; set; }
        public int orderId { get; set; }
        public string SHIPMENTNUMBER { get; set; }
        public string customerName { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string Address { get; set; }
        public string customerPhone { get; set; }
        public double orderNet { get; set; }
        public double orderTotal { get; set; }
        public double discount { get; set; }
        public double tax { get; set; }
        public double delieveryCost { get; set; }
        public string paymentMethod { get; set; }
        public string notes { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string Perfume1 { get; set; }
        public int quantity1 { get; set; }

       
        public string Perfume2 { get; set; }
        public int quantity2 { get; set; }

        public string Perfume3 { get; set; }
        public int quantity3 { get; set; }

        public string Perfume4 { get; set; }
        public int quantity4 { get; set; }

        public string Perfume5 { get; set; }
        public int quantity5 { get; set; }

        public string Perfume6 { get; set; }
        public int quantity6 { get; set; }

        public string Perfume7 { get; set; }
        public int quantity7 { get; set; }

        public string Perfume8 { get; set; }
        public int quantity8 { get; set; }

        public string Perfume9 { get; set; }
        public int quantity9 { get; set; }
        public string Perfume10 { get; set; }
        public int quantity10 { get; set; }
        public List<OrderItemsDataGrid> orderItems { get; set; }
    }
}
