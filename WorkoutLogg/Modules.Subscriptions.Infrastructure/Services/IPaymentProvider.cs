using Modules.Subscriptions.Infrastructure.Domain;

namespace Modules.Subscriptions.Infrastructure.Services
{
    public record CheckoutRequest(
        string UserId,
        string UserEmail,
        SubscriptionPlan Plan,
        string ReturnUrl,
        string CancelUrl);

    public record CheckoutResult(
        bool Success,
        string? CheckoutUrl,
        string? PaymentId,
        string? Error);

    public record WebhookResult(
        bool Processed,
        string? UserId,
        string? SubscriptionId,
        bool IsActivated);

    public interface IPaymentProvider
    {
        PaymentProviderType ProviderType { get; }
        Task<CheckoutResult> CreateCheckoutAsync(CheckoutRequest request, CancellationToken ct = default);
        Task<WebhookResult> ProcessWebhookAsync(string payload, string signature, CancellationToken ct = default);
    }
}
