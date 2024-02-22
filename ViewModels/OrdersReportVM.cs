namespace CRM.ViewModels
{
    public class OrdersReportVM
    {
        public int Ordercount { get; set; }
        public double AverageAmount { get; set; }
        public double Discount { get; set; }
        
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemOrder { get; set; }
        public decimal NetOrder { get; set; }
        public double OrderTotal { get; set; }
        public string CusAddress { get; set; }
        public string CusName { get; set; }
        public string Status { get; set; }
        public string Country { get; set; }
        public int? CountryId { get; set; }
        public int? OrderStatusId { get; set; }
        
        public double Tax { get; set; }

        public double ShippingCost { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddressPhone { get; set; }
        public int InvoiceNumber { get; set; }
        public string WebSite { get; set; }
        public string SuppEmail { get; set; }
        public string ConntactNumber { get; set; }
        public string PaymentTit { get; set; }
        public string Address { get; set; }
        public string OrderTime { get; set; }
        public List<OrderItemVm> orderItemVms { get; set; }
        public string? ShippingLabel { get; set; }
        public string? ShippingNo { get; set; }
        public string ItemTitleEn { get; set; }
        public string ItemDis { get; set; }
        public string ItemImage { get; set; }
        public double ItemPrice { get; set; }

        public int ItemQuantity { get; set; }
        public double Total { get; set; }
        public string OrderSerail { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string AdminAddress { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int? currencyId { get; set; }
        public string PaymentMethod { get; set; }
        public double orderNetByTax { get; set; }

        public string currencyName { get; set; }
    }
}
