namespace Modules.Subscriptions.Infrastructure.Services
{
    public class SubscriptionSettings
    {
        /// <summary>
        /// Если true — провайдер оплаты не дёргается, подписка активируется мгновенно.
        /// Только для разработки и тестирования.
        /// </summary>
        public bool TestMode { get; set; } = false;

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
