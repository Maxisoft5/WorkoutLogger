using Modules.Subscriptions.Infrastructure.Services;
using System.Net;
using System.Text;

namespace Module.Users.Tests.Payments;

[TestFixture]
public class YooKassaWebhookTests
{
    private const string WebhookPayload =
        """{"event":"payment.succeeded","object":{"id":"pay_1","status":"succeeded","metadata":{"user_id":"attacker-controlled"}}}""";

    private sealed class FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public int Calls { get; private set; }
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            LastRequest = request;
            return Task.FromResult(respond(request));
        }
    }

    private static HttpResponseMessage Json(HttpStatusCode status, string body) =>
        new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private static YooKassaProvider CreateProvider(FakeHandler handler) =>
        new(new HttpClient(handler), new SubscriptionSettings
        {
            YooKassaShopId = "shop",
            YooKassaSecretKey = "secret"
        });

    [Test]
    public async Task PaymentSucceeded_VerifiedViaApi_UsesApiResponseAsSourceOfTruth()
    {
        var handler = new FakeHandler(_ => Json(HttpStatusCode.OK,
            """{"id":"pay_1","status":"succeeded","metadata":{"user_id":"real-user-from-api"}}"""));
        var provider = CreateProvider(handler);

        var result = await provider.ProcessWebhookAsync(WebhookPayload, string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(handler.Calls, Is.EqualTo(1), "The payment must be re-fetched from the API");
            Assert.That(handler.LastRequest!.RequestUri!.AbsolutePath, Does.EndWith("/v2/payments/pay_1"));
            Assert.That(result.Processed, Is.True);
            Assert.That(result.IsActivated, Is.True);
            Assert.That(result.UserId, Is.EqualTo("real-user-from-api"),
                "user_id must come from the API response, not the forgeable webhook body");
        });
    }

    [Test]
    public async Task PaymentNotSucceededInApi_IsNotActivated()
    {
        var handler = new FakeHandler(_ => Json(HttpStatusCode.OK,
            """{"id":"pay_1","status":"pending","metadata":{"user_id":"u1"}}"""));
        var provider = CreateProvider(handler);

        var result = await provider.ProcessWebhookAsync(WebhookPayload, string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(result.Processed, Is.True);
            Assert.That(result.IsActivated, Is.False,
                "A forged webhook for a non-succeeded payment must not activate Premium");
        });
    }

    [Test]
    public async Task UnknownPaymentId_IsRejected()
    {
        var handler = new FakeHandler(_ => Json(HttpStatusCode.NotFound, """{"description":"not found"}"""));
        var provider = CreateProvider(handler);

        var result = await provider.ProcessWebhookAsync(WebhookPayload, string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(result.Processed, Is.False);
            Assert.That(result.IsActivated, Is.False);
        });
    }

    [Test]
    public async Task IrrelevantEvent_DoesNotCallApiAndDoesNotActivate()
    {
        var handler = new FakeHandler(_ => throw new InvalidOperationException("must not be called"));
        var provider = CreateProvider(handler);

        var result = await provider.ProcessWebhookAsync(
            """{"event":"payment.canceled","object":{"id":"pay_1"}}""", string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(handler.Calls, Is.EqualTo(0));
            Assert.That(result.Processed, Is.True);
            Assert.That(result.IsActivated, Is.False);
        });
    }

    [Test]
    public async Task MalformedPayload_IsRejected()
    {
        var handler = new FakeHandler(_ => Json(HttpStatusCode.OK, "{}"));
        var provider = CreateProvider(handler);

        var result = await provider.ProcessWebhookAsync("not-json-at-all", string.Empty);

        Assert.That(result.Processed, Is.False);
    }
}
