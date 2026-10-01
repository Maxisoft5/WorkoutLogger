using System.ComponentModel.DataAnnotations;
using Modules.Users.DTO.Auth;
using WorkoutLogger.WebApi.Controllers;

namespace Modules.Users.Tests.Auth;

public class PhoneLoginTests
{
    [TestCase("+7 (999) 123-45-67", "+79991234567")]
    [TestCase("8 (999) 123-45-67", "+79991234567")]
    [TestCase("79991234567", "+79991234567")]
    [TestCase("+44 7700 900123", "+447700900123")]
    [TestCase("+1 202 555 0100", "+12025550100")]
    [TestCase("123", null)]
    [TestCase("9991234567", null)]
    [TestCase("+7999abc4567", null)]
    [TestCase("+0 999 1234567", null)]
    [TestCase("+7999123456789012345", null)]
    [TestCase("", null)]
    public void NormalizePhone(string input, string? expected) => Assert.That(LoginIdentifier.NormalizePhone(input), Is.EqualTo(expected));

    [Test]
    public void WebContactRequiresExactlyOneIdentifier()
    {
        bool Valid(WebLoginRequest request) => Validator.TryValidateObject(request, new ValidationContext(request), [], true);
        Assert.That(Valid(new() { Email = "member@example.test", Password = "Test!12345" }), Is.True);
        Assert.That(Valid(new() { PhoneNumber = "8 (999) 123-45-67", Password = "Test!12345" }), Is.True);
        Assert.That(Valid(new() { Password = "Test!12345" }), Is.False);
        Assert.That(Valid(new() { Email = "member@example.test", PhoneNumber = "+79991234567", Password = "Test!12345" }), Is.False);
        Assert.That(Valid(new() { PhoneNumber = "abcdef", Password = "Test!12345" }), Is.False);
    }
    [Test]
    public void PhoneFormattingSharesOneRateLimitKey()
    {
        var a = new WebLoginRequest { PhoneNumber = "8 (999) 123-45-67" };
        var b = new WebLoginRequest { PhoneNumber = "+79991234567" };
        Assert.That(a.Identifier, Is.EqualTo(b.Identifier));
        Assert.That(new WebLoginRequest { Email = "", PhoneNumber = b.PhoneNumber }.Identifier, Is.EqualTo(b.Identifier));
    }
}
