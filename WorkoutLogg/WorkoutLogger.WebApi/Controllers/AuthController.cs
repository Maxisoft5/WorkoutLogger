using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.Domain.Events;
using Modules.Common.Infrastructure.Messaging;
using Modules.Users.Domain.Authentication;
using Modules.Users.Domain.Mappers;
using Modules.Users.DTO.Auth;

namespace WorkoutLogger.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService,
    IUserService userService,
    IHttpContextAccessor httpContextAccessor,
    IEventPublisher eventPublisher,
    KafkaSettings kafkaSettings) : ControllerBase
{

    [Authorize]
    [HttpGet("CurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var login = await authService.GetCurrent();
        if (login.IsSuccess && login.Value != null)
        {
            return Ok(UserMapper.MapUser(login.Value));
        }
        return Forbid();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserDto user)
    {
        var login = await authService.LoginAsync(user.Email, user.Password);

        var ctx = httpContextAccessor.HttpContext;
        if (login.IsSuccess)
        {
            var logined = await userService.GetUserByEmail(user.Email);
            if (logined.IsSuccess)
            {
                await eventPublisher.PublishAsync(kafkaSettings.Topics.AuthEvents, new AuthEvent
                {
                    EventType = "user.login",
                    Email = user.Email ?? "unknown",
                    UserId = logined.Value?.Id ?? "",
                    IpAddress = ctx?.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = ctx?.Request.Headers.UserAgent.ToString()
                });
            }
        }
        else
        {
            await eventPublisher.PublishAsync(kafkaSettings.Topics.AuthEvents, new AuthEvent
            {
                EventType = "user.login_failed",
                Email = user.Email ?? "unknown",
                IpAddress = ctx?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = ctx?.Request.Headers.UserAgent.ToString()
            });
        }
        return Ok(login);
    }

    [Authorize]
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest token)
    {
        var refreshed = await authService.RefreshTokenAsync(token.Token, token.RefreshToken);
        return Ok(refreshed);
    }

    [HttpPost("CreateAccount")]
    public async Task<IActionResult> CreateAccount([FromBody] UserDto user)
    {
        var created = await authService.RegisterAsync(user, default);
        return Ok(created);
    }

    [Authorize]
    [HttpPut("UpdateAccount")]
    public async Task<IActionResult> UpdateAccount([FromBody] UserDto user)
    {
        var upd = await authService.UpdateUser(user);
        return Ok(UserMapper.MapUser(upd.Value));
    }
}
