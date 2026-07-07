using Modules.Subscriptions.Infrastructure.Domain;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Modules.Subscriptions.Infrastructure.Services
{
    public class YooKassaProvider : IPaymentProvider
    {
        private static readonly decimal PriceMonthly = 399m;
        private static readonly decimal PriceAnnual = 2990m;

        private readonly HttpClient _http;
        private readonly SubscriptionSettings _settings;

        public PaymentProviderType ProviderType => PaymentProviderType.YooKassa;

        public YooKassaProvider(HttpClient http, SubscriptionSettings settings)
        {
            _http = http;
            _settings = settings;
            if (!string.IsNullOrEmpty(settings.YooKassaShopId))
            {
                var encoded = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{settings.YooKassaShopId}:{settings.YooKassaSecretKey}"));
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
            }
        }

        public async Task<CheckoutResult> CreateCheckoutAsync(CheckoutRequest request, CancellationToken ct = default)
        {
            var amount = request.Plan == SubscriptionPlan.Annual ? PriceAnnual : PriceMonthly;

            var body = new
            {
                amount = new { value = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), currency = "RUB" },
                confirmation = new { type = "redirect", return_url = request.ReturnUrl },
                capture = true,
                description = $"WorkoutLog Premium {request.Plan} — {request.UserEmail}",
                metadata = new { user_id = request.UserId, plan = request.Plan.ToString() }
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.yookassa.ru/v2/payments");
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            req.Headers.Add("Idempotence-Key", Guid.NewGuid().ToString());

            try
            {
                var resp = await _http.SendAsync(req, ct);
                var json = await resp.Content.ReadAsStringAsync(ct);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!resp.IsSuccessStatusCode)
                {
                    var desc = root.TryGetProperty("description", out var d) ? d.GetString() : "YooKassa error";
                    return new CheckoutResult(false, null, null, desc);
                }

                var paymentId = root.GetProperty("id").GetString();
                var confirmUrl = root.GetProperty("confirmation").GetProperty("confirmation_url").GetString();
                return new CheckoutResult(true, confirmUrl, paymentId, null);
            }
            catch (Exception ex)
            {
                return new CheckoutResult(false, null, null, ex.Message);
            }
        }

        public async Task<WebhookResult> ProcessWebhookAsync(string payload, string signature, CancellationToken ct = default)
        {
            try
            {
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;
                var eventType = root.GetProperty("event").GetString();

                if (eventType != "payment.succeeded")
                    return new WebhookResult(true, null, null, false);

                var paymentId = root.GetProperty("object").GetProperty("id").GetString()!;

                // YooKassa webhooks are not signed, so the notification body cannot be
                // trusted: anyone can POST a fake "payment.succeeded" event and get
                // Premium for free. Per YooKassa docs, the payment must be re-fetched
                // from the API and only the API response used as the source of truth.
                var verified = await GetPaymentAsync(paymentId, ct);
                if (verified is null)
                    return new WebhookResult(false, null, null, false);

                var status = verified.Value.GetProperty("status").GetString();
                if (status != "succeeded")
                    return new WebhookResult(true, null, null, false);

                var userId = verified.Value.GetProperty("metadata").GetProperty("user_id").GetString()!;
                return new WebhookResult(true, userId, paymentId, true);
            }
            catch
            {
                return new WebhookResult(false, null, null, false);
            }
        }

        private async Task<JsonElement?> GetPaymentAsync(string paymentId, CancellationToken ct)
        {
            using var resp = await _http.GetAsync(
                $"https://api.yookassa.ru/v2/payments/{Uri.EscapeDataString(paymentId)}", ct);
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
    }
}
