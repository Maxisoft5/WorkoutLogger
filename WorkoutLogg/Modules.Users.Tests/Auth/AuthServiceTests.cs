using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.Common.Infrastructure.Caching;
using Modules.Common.Infrastructure.Configurations;
using Modules.Common.Infrastructure.Messaging;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Authorization;
using Modules.Users.Infrastructure.Database;
using NSubstitute;

namespace Modules.Users.Tests.Auth;

[TestFixture]
public class AuthServiceTests
{
    private UserManager<User> _userManager = null!;
    private SignInManager<User> _signInManager = null!;
    private ICacheService _cache = null!;
    private AuthService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _userManager = Substitute.For<UserManager<User>>(
            Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        // Identity 2.3.x: SignInManager ctor has 6 parameters (no IUserConfirmation).
        _signInManager = Substitute.For<SignInManager<User>>(
            _userManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<User>>(),
            null, null, null);
        _cache = Substitute.For<ICacheService>();

        _sut = new AuthService(
            _userManager,
            Substitute.For<RoleManager<Role>>(Substitute.For<IRoleStore<Role>>(), null, null, null, null),
            _signInManager,
            Substitute.For<ILogger<AuthService>>(),
            Options.Create(new AuthConfiguration { Key = new string('k', 64), Issuer = "test", Audience = "test" }),
            new TokenValidationParameters(),
            Substitute.For<IHttpContextAccessor>(),
            null!, // UsersDbContext — not used by the paths under test
            Substitute.For<IEventPublisher>(),
            new KafkaSettings(),
            _cache);
    }

    [TearDown]
    public void TearDown()
    {
        _userManager.Dispose();
    }

    // ── LoginAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task LoginAsync_UnknownEmail_ReturnsExplicitError()
    {
        _userManager.FindByEmailAsync("ghost@example.com").Returns((User?)null);

        var result = await _sut.LoginAsync("ghost@example.com", "whatever");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False,
                "Failed login must carry an explicit error, not an empty list");
            Assert.That(result.FirstError.Code, Does.Contain("InvalidCredentials"));
        });
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ReturnsSameErrorAsUnknownEmail()
    {
        var user = new User { Id = "u1", Email = "user@example.com" };
        _userManager.FindByEmailAsync("user@example.com").Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, "wrong", false)
            .Returns(SignInResult.Failed);

        var result = await _sut.LoginAsync("user@example.com", "wrong");
        _userManager.FindByEmailAsync("ghost@example.com").Returns((User?)null);
        var unknown = await _sut.LoginAsync("ghost@example.com", "wrong");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            // Same error for both cases — no account enumeration.
            Assert.That(result.FirstError.Code, Is.EqualTo(unknown.FirstError.Code));
        });
    }

    // ── VerifyResetCodeAsync ─────────────────────────────────────────────────

    private const string Email = "user@example.com";
    private const string CodeKey = $"reset:{Email}";
    private const string AttemptsKey = $"reset-attempts:{Email}";

    [Test]
    public async Task VerifyResetCode_NoCodeIssued_Fails()
    {
        _cache.GetAsync<string>(CodeKey, Arg.Any<CancellationToken>()).Returns((string?)null);

        var result = await _sut.VerifyResetCodeAsync(Email, "123456");

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public async Task VerifyResetCode_CorrectCode_Succeeds()
    {
        _cache.GetAsync<string>(CodeKey, Arg.Any<CancellationToken>()).Returns("123456");
        _cache.GetAsync<string>(AttemptsKey, Arg.Any<CancellationToken>()).Returns((string?)null);

        var result = await _sut.VerifyResetCodeAsync(Email, "123456");

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task VerifyResetCode_WrongCode_FailsAndIncrementsAttempts()
    {
        _cache.GetAsync<string>(CodeKey, Arg.Any<CancellationToken>()).Returns("123456");
        _cache.GetAsync<string>(AttemptsKey, Arg.Any<CancellationToken>()).Returns("2");

        var result = await _sut.VerifyResetCodeAsync(Email, "000000");

        Assert.That(result.IsSuccess, Is.False);
        await _cache.Received(1).SetAsync(AttemptsKey, "3", Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task VerifyResetCode_TooManyAttempts_InvalidatesCode_EvenIfCodeIsCorrect()
    {
        _cache.GetAsync<string>(CodeKey, Arg.Any<CancellationToken>()).Returns("123456");
        _cache.GetAsync<string>(AttemptsKey, Arg.Any<CancellationToken>()).Returns("5");

        var result = await _sut.VerifyResetCodeAsync(Email, "123456");

        Assert.That(result.IsSuccess, Is.False,
            "After the attempt limit the code must be rejected even if guessed correctly");
        await _cache.Received(1).RemoveAsync(CodeKey, Arg.Any<CancellationToken>());
    }
}
