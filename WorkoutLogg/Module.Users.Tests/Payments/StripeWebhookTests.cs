using Modules.Subscriptions.Infrastructure.Services;
using System.Security.Cryptography;
using System.Text;

namespace Module.Users.Tests.Payments;

[TestFixture]
public class StripeWebhookTests
{
    private const string Secret = "whsec_test_secret";

    private const string ActivationPayload =
        """{"type":"checkout.session.completed","data":{"object":{"id":"cs_123","subscription":"sub_1","metadata":{"user_id":"u1","plan":"Monthly"}}}}""";

    private static StripeProvider CreateProvider(string webhookSecret) =>
        new(new HttpClient(), new SubscriptionSettings { StripeWebhookSecret = webhookSecret });

    private static string Sign(string payload, string secret)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{payload}"));
        return $"t={timestamp},v1={Convert.ToHexString(hash).ToLowerInvariant()}";
    }

    [Test]
    public async Task ValidSignature_ActivationEvent_IsProcessedAndActivated()
    {
        var provider = CreateProvider(Secret);

        var result = await provider.ProcessWebhookAsync(ActivationPayload, Sign(ActivationPayload, Secret));

        Assert.Multiple(() =>
        {
            Assert.That(result.Processed, Is.True);
            Assert.That(result.IsActivated, Is.True);
            Assert.That(result.UserId, Is.EqualTo("u1"));
            Assert.That(result.SubscriptionId, Is.EqualTo("sub_1"));
        });
    }

    [Test]
    public async Task InvalidSignature_IsRejected()
    {
        var provider = CreateProvider(Secret);

        var result = await provider.ProcessWebhookAsync(
            ActivationPayload, "t=1,v1=deadbeefdeadbeefdeadbeefdeadbeefdeadbeefdeadbeefdeadbeefdeadbeef");

        Assert.Multiple(() =>
        {
            Assert.That(result.Processed, Is.False);
            Assert.That(result.IsActivated, Is.False);
        });
    }

    [Test]
    public async Task SignatureForDifferentPayload_IsRejected()
    {
        var provider = CreateProvider(Secret);
        var signatureOfOtherBody = Sign("""{"type":"other"}""", Secret);

        var result = await provider.ProcessWebhookAsync(ActivationPayload, signatureOfOtherBody);

        Assert.That(result.Processed, Is.False);
    }

    [Test]
    public async Task MissingWebhookSecret_FailsClosed()
    {
        var provider = CreateProvider(webhookSecret: "");

        var result = await provider.ProcessWebhookAsync(ActivationPayload, Sign(ActivationPayload, Secret));

        Assert.That(result.Processed, Is.False,
            "Without a configured secret no webhook may be trusted (fail closed)");
    }

    [Test]
    public async Task ValidSignature_IrrelevantEvent_IsProcessedButNotActivated()
    {
        var provider = CreateProvider(Secret);
        var payload = """{"type":"customer.created","data":{"object":{"id":"cus_1"}}}""";

        var result = await provider.ProcessWebhookAsync(payload, Sign(payload, Secret));

        Assert.Multiple(() =>
        {
            Assert.That(result.Processed, Is.True);
            Assert.That(result.IsActivated, Is.False);
        });
    }
}
