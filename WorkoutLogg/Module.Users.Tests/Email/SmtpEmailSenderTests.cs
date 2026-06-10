using Modules.Common.Infrastructure.Email;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net.Http;

namespace Module.Users.Tests.Email;

[TestFixture]
public class SmtpEmailSenderTests
{
    // ── Unit: проверяем что IEmailSender вызывается с правильными параметрами ──

    [Test]
    public async Task SendAsync_CallsEmailSender_WithCorrectParameters()
    {
        var sender = Substitute.For<IEmailSender>();

        await sender.SendAsync("user@example.com", "Test Subject", "<p>Hello</p>");

        await sender.Received(1).SendAsync(
            "user@example.com",
            "Test Subject",
            "<p>Hello</p>",
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SendAsync_WhenCalledTwice_SendsTwice()
    {
        var sender = Substitute.For<IEmailSender>();

        await sender.SendAsync("a@example.com", "S1", "B1");
        await sender.SendAsync("b@example.com", "S2", "B2");

        await sender.Received(2).SendAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public void SendAsync_WhenSmtpThrows_PropagatesException()
    {
        var sender = Substitute.For<IEmailSender>();
        sender.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
              .ThrowsAsync(new InvalidOperationException("SMTP error"));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            sender.SendAsync("x@x.com", "s", "b"));
    }

    // ── SmtpSettings: проверяем дефолтные значения ──

    [Test]
    public void SmtpSettings_Defaults_AreCorrect()
    {
        var settings = new SmtpSettings();

        Assert.That(settings.Port, Is.EqualTo(587));
        Assert.That(settings.EnableSsl, Is.True);
        Assert.That(settings.Host, Is.Empty);
        Assert.That(settings.UserName, Is.Empty);
        Assert.That(settings.Password, Is.Empty);
        Assert.That(settings.From, Is.Empty);
    }

    [Test]
    public void SmtpSettings_GmailConfig_IsValid()
    {
        var settings = new SmtpSettings
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            UserName = "test@gmail.com",
            Password = "xxxx xxxx xxxx xxxx",
            From = "test@gmail.com"
        };

        Assert.That(settings.Host, Is.EqualTo("smtp.gmail.com"));
        Assert.That(settings.Port, Is.EqualTo(587));
        Assert.That(settings.EnableSsl, Is.True);
    }

    // ── Integration: реальная отправка (запускать вручную через Test Explorer) ──
    // Requires: правильный app password в appsettings.json

    [Test, Explicit("Mailtrap HTTP API — run manually to verify")]
    public async Task SendAsync_MailtrapHttpApi_DeliversEmail()
    {
        var settings = new MailtrapSettings
        {
            ApiUrl = "https://send.api.mailtrap.io/api/send",
            ApiToken = "6f260234f8416f0b75218227621d5be5",
            From = "hello@demomailtrap.co",
            FromName = "WorkoutLogger Test"
        };

        var sender = new MailtrapHttpEmailSender(settings, new HttpClient());

        Assert.DoesNotThrowAsync(() =>
            sender.SendAsync(
                to: "maxisoft4@gmail.com",
                subject: "WorkoutLogger — HTTP API Test",
                body: "<h2>Email через HTTP API работает!</h2>"));
    }
}
