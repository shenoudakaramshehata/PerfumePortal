using DevExpress.Utils.Text.Internal;

namespace CRM.Models
{
    public class OrderView
    {
      
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public string? OrderSerial { get; set; }
            public int? CustomerId { get; set; }
            public string? CustomerName { get; set; }
            public string? CustomerEmail { get; set; }
            public string? CustomerPhone { get; set; }
            public int? OrderStatusId { get; set; }
            public string? OrderStatus { get; set; }
            public bool IsCanceled { get; set; }
            public bool IsDeliverd { get; set; }
            public string? Notes { get; set; }
            public int? PaymentMethodId { get; set; }
            public int? CustomerAddressId { get; set; }
            public int? CountryId { get; set; }
            public string? CountryTitleEn { get; set; }
            public string? CountryTitleAr { get; set; }
            public double? OrderTotal { get; set; }
            public double? OrderDiscount { get; set; }
            public int? CouponId { get; set; }
            public int? CouponTypeId { get; set; }
            public double? CouponAmount { get; set; }
            public double? Deliverycost { get; set; }
            public double? OrderNet { get; set; }
            public bool IsPaid { get; set; }
            public int? UniqeId { get; set; }
            public string? PaymentID { get; set; }
            public DateTime? PostDate { get; set; }
            public int? CustomerNCustomerId { get; set; }
            public string? CustomerAddressCity { get; set; }
            public string? CustomerAddressArea { get; set; }
            public string? CustomerAddressAddress { get; set; }
            public string? CustomerAddressBuildingNo { get; set; }
            public string? CustomerAddressMobile { get; set; }
            public string? ShippingLabel { get; set; }
            public string? ShippingNo { get; set; }
            public double? tax { get; set; }
            public double? TotalAfterDiscount { get; set; }
            public double? DiscountAmount { get; set; }

        public string? PaymentMethodTitleEn { get; set; }
        public string? PaymentMethodTitleAr { get; set; }



    }
}
