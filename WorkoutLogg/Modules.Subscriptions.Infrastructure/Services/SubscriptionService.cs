using Microsoft.EntityFrameworkCore;
using Modules.Subscriptions.Infrastructure.Database;
using Modules.Subscriptions.Infrastructure.Domain;

namespace Modules.Subscriptions.Infrastructure.Services
{
    public class SubscriptionService
    {
        private readonly SubscriptionsDbContext _db;
        private readonly IEnumerable<IPaymentProvider> _providers;
        private readonly SubscriptionSettings _settings;

        public SubscriptionService(
            SubscriptionsDbContext db,
            IEnumerable<IPaymentProvider> providers,
            SubscriptionSettings settings)
        {
            _db = db;
            _providers = providers;
            _settings = settings;
        }

        public async Task<CheckoutResult> StartCheckoutAsync(
            string userId, string userEmail,
            SubscriptionPlan plan, bool useYooKassa,
            CancellationToken ct = default)
        {
            var providerType = useYooKassa ? PaymentProviderType.YooKassa : PaymentProviderType.Stripe;
            var provider = _providers.First(p => p.ProviderType == providerType);

            var result = await provider.CreateCheckoutAsync(new CheckoutRequest(
                userId, userEmail, plan,
                ReturnUrl: _settings.AppReturnUrl,
                CancelUrl: _settings.AppCancelUrl), ct);

            if (result.Success && result.PaymentId is not null)
            {
                _db.Subscriptions.Add(new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Plan = plan,
                    Status = SubscriptionStatus.Trial,
                    Provider = providerType,
                    ExternalPaymentId = result.PaymentId,
                    StartedAt = DateTime.UtcNow,
                    TrialEndsAt = DateTime.UtcNow.AddDays(7),
                    ExpiresAt = plan == SubscriptionPlan.Annual
                        ? DateTime.UtcNow.AddYears(1)
                        : DateTime.UtcNow.AddMonths(1),
                    CreatedAt = DateTime.UtcNow,
                });
                await _db.SaveChangesAsync(ct);
            }

            return result;
        }

        public Task<Subscription?> GetActiveSubscriptionAsync(string userId, CancellationToken ct = default) =>
            _db.Subscriptions
                .Where(s => s.UserId == userId &&
                    (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial))
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(ct);

        public async Task ActivateSubscriptionAsync(
            string userId, string externalPaymentId, string? externalSubscriptionId,
            CancellationToken ct = default)
        {
            var sub = await _db.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ExternalPaymentId == externalPaymentId, ct);

            if (sub is null) return;
            sub.Status = SubscriptionStatus.Active;
            sub.ExternalSubscriptionId = externalSubscriptionId;
            sub.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task CancelSubscriptionAsync(string userId, CancellationToken ct = default)
        {
            var sub = await GetActiveSubscriptionAsync(userId, ct);
            if (sub is null) return;
            sub.Status = SubscriptionStatus.Cancelled;
            sub.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public IPaymentProvider GetProvider(PaymentProviderType type) =>
            _providers.First(p => p.ProviderType == type);
    }
}
