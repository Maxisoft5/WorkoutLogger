namespace Modules.Subscriptions.Infrastructure.Domain
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public SubscriptionPlan Plan { get; set; }
        public SubscriptionStatus Status { get; set; }
        public PaymentProviderType Provider { get; set; }
        public string? ExternalPaymentId { get; set; }
        public string? ExternalSubscriptionId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? TrialEndsAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public enum SubscriptionPlan { Monthly, Annual }

    public enum SubscriptionStatus { Trial, Active, Cancelled, Expired, PaymentFailed }

    public enum PaymentProviderType { YooKassa, Stripe }
}
