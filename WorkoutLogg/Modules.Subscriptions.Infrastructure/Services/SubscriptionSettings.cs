namespace Modules.Subscriptions.Infrastructure.Services
{
    public class SubscriptionSettings
    {
        public string YooKassaShopId { get; set; } = string.Empty;
        public string YooKassaSecretKey { get; set; } = string.Empty;
        public string YooKassaWebhookSecret { get; set; } = string.Empty;
        public string StripeSecretKey { get; set; } = string.Empty;
        public string StripeWebhookSecret { get; set; } = string.Empty;
        public string StripePriceMonthlyId { get; set; } = string.Empty;
        public string StripePriceAnnualId { get; set; } = string.Empty;
        public string AppReturnUrl { get; set; } = "workoutlogg://payment/success";
        public string AppCancelUrl { get; set; } = "workoutlogg://payment/cancel";
    }
}
