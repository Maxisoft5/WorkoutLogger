using Modules.Subscriptions.Infrastructure.Domain;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Modules.Subscriptions.Infrastructure.Services
{
    public class StripeProvider : IPaymentProvider
    {
        private readonly HttpClient _http;
        private readonly SubscriptionSettings _settings;

        public PaymentProviderType ProviderType => PaymentProviderType.Stripe;

        public StripeProvider(HttpClient http, SubscriptionSettings settings)
        {
            _http = http;
            _settings = settings;
            if (!string.IsNullOrEmpty(settings.StripeSecretKey))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.StripeSecretKey);
        }

        public async Task<CheckoutResult> CreateCheckoutAsync(CheckoutRequest request, CancellationToken ct = default)
        {
            var priceId = request.Plan == SubscriptionPlan.Annual
                ? _settings.StripePriceAnnualId
                : _settings.StripePriceMonthlyId;

            var form = new Dictionary<string, string>
            {
                ["mode"] = "subscription",
                ["success_url"] = request.ReturnUrl,
                ["cancel_url"] = request.CancelUrl,
                ["customer_email"] = request.UserEmail,
                ["line_items[0][price]"] = priceId,
                ["line_items[0][quantity]"] = "1",
                ["subscription_data[trial_period_days]"] = "7",
                ["metadata[user_id]"] = request.UserId,
                ["metadata[plan]"] = request.Plan.ToString(),
            };

            try
            {
                var resp = await _http.PostAsync(
                    "https://api.stripe.com/v1/checkout/sessions",
                    new FormUrlEncodedContent(form), ct);
                var json = await resp.Content.ReadAsStringAsync(ct);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!resp.IsSuccessStatusCode)
                {
                    var msg = root.TryGetProperty("error", out var err)
                        ? err.GetProperty("message").GetString()
                        : "Stripe error";
                    return new CheckoutResult(false, null, null, msg);
                }

                var sessionId = root.GetProperty("id").GetString();
                var url = root.GetProperty("url").GetString();
                return new CheckoutResult(true, url, sessionId, null);
            }
            catch (Exception ex)
            {
                return new CheckoutResult(false, null, null, ex.Message);
            }
        }

        public Task<WebhookResult> ProcessWebhookAsync(string payload, string signature, CancellationToken ct = default)
        {
            if (!VerifySignature(payload, signature, _settings.StripeWebhookSecret))
                return Task.FromResult(new WebhookResult(false, null, null, false));

            try
            {
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;
                var eventType = root.GetProperty("type").GetString();

                if (eventType != "checkout.session.completed" && eventType != "invoice.payment_succeeded")
                    return Task.FromResult(new WebhookResult(true, null, null, false));

                var data = root.GetProperty("data").GetProperty("object");
                var meta = data.GetProperty("metadata");
                var userId = meta.GetProperty("user_id").GetString()!;
                var subId = data.TryGetProperty("subscription", out var s) ? s.GetString() : data.GetProperty("id").GetString();

                return Task.FromResult(new WebhookResult(true, userId, subId, true));
            }
            catch
            {
                return Task.FromResult(new WebhookResult(false, null, null, false));
            }
        }

        private static bool VerifySignature(string payload, string signature, string secret)
        {
            // Fail closed: without a configured webhook secret we cannot verify
            // authenticity, so no webhook may activate a subscription.
            if (string.IsNullOrEmpty(secret)) return false;
            try
            {
                var parts = signature.Split(',')
                    .Select(p => p.Split('='))
                    .Where(p => p.Length == 2)
                    .ToDictionary(p => p[0], p => p[1]);

                var timestamp = parts["t"];
                var signed = $"{timestamp}.{payload}";
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signed));
                var computed = Convert.ToHexString(hash).ToLowerInvariant();
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(computed),
                    Encoding.UTF8.GetBytes(parts["v1"]));
            }
            catch { return false; }
        }
    }
}
