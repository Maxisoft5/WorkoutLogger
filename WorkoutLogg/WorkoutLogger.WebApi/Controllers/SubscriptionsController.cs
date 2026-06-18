using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Subscriptions.Infrastructure.Domain;
using Modules.Subscriptions.Infrastructure.Services;
using Modules.Users.Domain.Authentication;
using System.Security.Claims;

namespace WorkoutLogger.WebApi.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly IUserService _userService;

        public SubscriptionsController(SubscriptionService subscriptionService, IUserService userService)
        {
            _subscriptionService = subscriptionService;
            _userService = userService;
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> CreateCheckout(
            [FromBody] CheckoutApiRequest request, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            if (userId is null) return Unauthorized();

            var plan = string.Equals(request.Plan, "annual", StringComparison.OrdinalIgnoreCase)
                ? SubscriptionPlan.Annual
                : SubscriptionPlan.Monthly;
            var useYooKassa = request.Locale?.StartsWith("ru", StringComparison.OrdinalIgnoreCase) == true;

            var result = await _subscriptionService.StartCheckoutAsync(userId, email, plan, useYooKassa, ct);
            if (!result.Success)
                return BadRequest(new { error = result.Error });

            if (result.PaymentId == "test-mode")
            {
                await _userService.SetPremiumAsync(userId, true);
                return Ok(new { checkoutUrl = (string?)null, paymentId = result.PaymentId, activated = true });
            }

            return Ok(new { checkoutUrl = result.CheckoutUrl, paymentId = result.PaymentId, activated = false });
        }

        [HttpGet("status")]
        [Authorize]
        public async Task<IActionResult> GetStatus(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var sub = await _subscriptionService.GetActiveSubscriptionAsync(userId, ct);
            return Ok(new
            {
                isActive = sub is not null,
                plan = sub?.Plan.ToString(),
                status = sub?.Status.ToString(),
                expiresAt = sub?.ExpiresAt,
                trialEndsAt = sub?.TrialEndsAt,
            });
        }

        [HttpPost("cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            await _subscriptionService.CancelSubscriptionAsync(userId, ct);
            await _userService.SetPremiumAsync(userId, false);
            return Ok();
        }

        [HttpPost("/webhooks/yookassa")]
        [AllowAnonymous]
        public async Task<IActionResult> YooKassaWebhook(CancellationToken ct)
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync(ct);
            var provider = _subscriptionService.GetProvider(PaymentProviderType.YooKassa);
            var result = await provider.ProcessWebhookAsync(payload, string.Empty, ct);

            if (result.Processed && result.IsActivated && result.UserId is not null)
            {
                await _subscriptionService.ActivateSubscriptionAsync(
                    result.UserId, result.SubscriptionId ?? string.Empty, null, ct);
                await _userService.SetPremiumAsync(result.UserId, true);
            }

            return Ok();
        }

        [HttpPost("/webhooks/stripe")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook(CancellationToken ct)
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync(ct);
            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault() ?? "";
            var provider = _subscriptionService.GetProvider(PaymentProviderType.Stripe);
            var result = await provider.ProcessWebhookAsync(payload, signature, ct);

            if (result.Processed && result.IsActivated && result.UserId is not null)
            {
                await _subscriptionService.ActivateSubscriptionAsync(
                    result.UserId, result.SubscriptionId ?? string.Empty, result.SubscriptionId, ct);
                await _userService.SetPremiumAsync(result.UserId, true);
            }

            return Ok();
        }
    }

    public record CheckoutApiRequest(string? Plan, string? Locale);
}
