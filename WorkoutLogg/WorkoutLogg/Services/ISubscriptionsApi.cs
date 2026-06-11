using Refit;

namespace WorkoutLogg.Services
{
    public record SubscriptionCheckoutRequest(string Plan, string Locale);

    public record SubscriptionCheckoutResponse(string? CheckoutUrl, string? PaymentId);

    public record SubscriptionStatusResponse(
        bool IsActive, string? Plan, string? Status,
        DateTime? ExpiresAt, DateTime? TrialEndsAt);

    public interface ISubscriptionsApi
    {
        [Post("/api/subscriptions/checkout")]
        Task<IApiResponse<SubscriptionCheckoutResponse>> CheckoutAsync(
            [Header("Authorization")] string token,
            [Body] SubscriptionCheckoutRequest request);

        [Get("/api/subscriptions/status")]
        Task<IApiResponse<SubscriptionStatusResponse>> GetStatusAsync(
            [Header("Authorization")] string token);
    }
}
