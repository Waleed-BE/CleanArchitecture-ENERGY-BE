namespace CleanArchitecture.Application.DTOs.StripePayment
{
    public class PurchasePlanDto
    {
        public Guid UserId { get; set; }
        public string StripeCustomerId { get; set; }
        public string StripePriceId { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Interval { get; set; }
        public string PlanPurchaseType { get; set; }
    }
}
