namespace CRM.Models
{
    public class Installments
    {
        public string downpayment { get; set; }
        public string downpayment_percent { get; set; }
        public string downpayment_increased_reason { get; set; }
        public string amount_to_pay { get; set; }
        public string downpayment_total { get; set; }
        public string order_amount { get; set; }
        public string next_payment_date { get; set; }
        public List<InstallmentDetails> installments { get; set; }
        public bool PayAfterDelivery { get; set; }
        public string pay_per_installment { get; set; }
        public string web_url { get; set; }
        public string QrCode { get; set; }
        public int Id { get; set; }
        public int InstallmentsCount { get; set; }
        public string InstallmentPeriod { get; set; }
        public string ServiceFee { get; set; }
    }
}
