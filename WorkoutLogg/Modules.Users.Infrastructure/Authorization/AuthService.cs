using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.Common.Domain.Events;
using Modules.Common.Domain.Results;
using Modules.Common.Infrastructure.Caching;
using Modules.Common.Infrastructure.Configurations;
using Modules.Common.Infrastructure.Messaging;
using Modules.Users.Domain.Authentication;
using Modules.Users.Domain.Tokens;
using Modules.Users.Domain.Users;
using Modules.Users.DTO.Auth;
using Modules.Users.DTO.Users;
using Modules.Users.Infrastructure.Database;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Modules.Users.Infrastructure.Authorization
{
    public class AuthService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    SignInManager<User> signInManager,
    ILogger<AuthService> logger,
    IOptions<AuthConfiguration> authOptions,
    TokenValidationParameters tokenValidationParameters,
    IHttpContextAccessor httpContextAccessor,
    UsersDbContext dbContext,
    IEventPublisher eventPublisher,
    KafkaSettings kafkaSettings,
    ICacheService cacheService) : IAuthService
    {
        public async Task<Result<LoginUserResponse>> LoginAsync(string email, string password, 
            CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return new Result<LoginUserResponse>(new List<Error>());
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return new Result<LoginUserResponse>(new List<Error>());
            }

            var (token, refreshToken) = await GenerateJwtAndRefreshTokenAsync(user, null);

            await cacheService.GetOrCreateAsync(
                $"user:{user.Id}",
                async ct => user,
                TimeSpan.FromMinutes(5));

            return new Result<LoginUserResponse>(new LoginUserResponse(token, refreshToken));
        }

        public async Task<Result<RegisterUserResponse>> RegisterAsync(UserDto requestUser, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = requestUser.Email,
                UserName = requestUser.FullName,
                UserRegistrationStep = DTO.Users.UserRegistrationStep.Profile
            };
            var result = await userManager.CreateAsync(user, requestUser.Password);
            if (!result.Succeeded)
            {
                logger.LogInformation("Failed to register user: {@Errors}", result.Errors);
                return new Result<RegisterUserResponse>(result.
                    Errors.Select(x => new Error(x.Code, x.Description, ErrorType.Validation)).ToList());
            }
            var (token, refreshToken) = await GenerateJwtAndRefreshTokenAsync(user, null);
            await cacheService.GetOrCreateAsync(
             $"user:{user.Id}",
             async ct => user,
             TimeSpan.FromMinutes(5));

            return new Result<RegisterUserResponse>(new RegisterUserResponse(token, refreshToken));
        }

        public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(string token, string refreshToken, 
            CancellationToken cancellationToken=default)
        {
            var validatedToken = GetPrincipalFromToken(token, tokenValidationParameters);
            if (validatedToken is null)
            {
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            var jti = validatedToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            var storedRefreshToken = await dbContext.Set<RefreshToken>().FirstOrDefaultAsync(x => x.Token == refreshToken,
                cancellationToken);
            if (storedRefreshToken is null)
            {
                logger.LogWarning("Refresh token does not exist");
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                logger.LogWarning("Refresh token has expired");
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            if (storedRefreshToken.Invalidated)
            {
                logger.LogWarning("Refresh token has been invalidated");
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            if (storedRefreshToken.JwtId != jti)
            {
                logger.LogWarning("Refresh token does not match this JWT");
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            var userId = validatedToken.Claims.FirstOrDefault(x => x.Type == "userid")?.Value;
            if (userId is null)
            {
                logger.LogWarning("Current user is not found");
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("Current user is not found");
                return new Result<RefreshTokenResponse>(new Error("401", "401", ErrorType.Forbidden));
            }

            var (newToken, newRefreshToken) = await GenerateJwtAndRefreshTokenAsync(user, refreshToken);
            return new Result<RefreshTokenResponse>(new RefreshTokenResponse(newToken, newRefreshToken));
        }

        private async Task<(string token, string refreshToken)> GenerateJwtAndRefreshTokenAsync(User user, string? existingRefreshToken)
        {
            var roles = await userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "user";

            var role = await roleManager.FindByNameAsync(userRole);
            var roleClaims = role is not null ? await roleManager.GetClaimsAsync(role) : [];

            var token = GenerateJwtToken(user, authOptions.Value, userRole, roleClaims);
            var refreshToken = await GenerateRefreshTokenAsync(token, user, existingRefreshToken);

            return (token, refreshToken);
        }

        private async Task<string> GenerateRefreshTokenAsync(string token, User user, string? existingRefreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var jti = jwtToken.Id;

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = jti,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAtUtc = DateTime.UtcNow,
            };

            if (!string.IsNullOrEmpty(existingRefreshToken))
            {
                var existingToken = await dbContext.Set<RefreshToken>().FirstOrDefaultAsync(x => x.Token == existingRefreshToken);
                if (existingToken != null)
                {
                    dbContext.Set<RefreshToken>().Remove(existingToken);
                }
            }

            await dbContext.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();

            return refreshToken.Token;
        }

        private static string GenerateJwtToken(User user,
            AuthConfiguration authConfiguration,
            string userRole,
            IList<Claim> roleClaims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenId = Guid.NewGuid().ToString();
            List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, user.Email!),
                new("userid", user.Id),
                new("role", userRole),
                new(JwtRegisteredClaimNames.Jti, tokenId)
            ];

            foreach (var roleClaim in roleClaims)
            {
                claims.Add(new Claim(roleClaim.Type, roleClaim.Value));
            }

            var token = new JwtSecurityToken(
                issuer: authConfiguration.Issuer,
                audience: authConfiguration.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static ClaimsPrincipal? GetPrincipalFromToken(string token, TokenValidationParameters parameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = parameters.Clone();

#pragma warning disable CA5404
                tokenValidationParameters.ValidateLifetime = false;
#pragma warning restore CA5404

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return IsJwtWithValidSecurityAlgorithm(validatedToken) ? principal : null;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
            => validatedToken is JwtSecurityToken jwtSecurityToken
               && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Updates a user's role and invalidates their refresh tokens
        /// </summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <param name="newRole">The new role to assign to the user</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A result containing success or failure information</returns>
        public async Task<Result<UpdateRoleResponse>> UpdateUserRoleAsync(string userId, string newRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            // Verify the role exists
            //var role = await roleManager.FindByNameAsync(newRole);
            //if (role is null)
            //{
            //    logger.LogWarning("Role '{NewRole}' does not exist", newRole);
            //    return UserErrors.RoleNotFound(newRole);
            //}

            //// Find the user
            //var user = await userManager.FindByIdAsync(userId);
            //if (user is null)
            //{
            //    return UserErrors.NotFound(userId);
            //}

            //// Get current roles and remove them
            //var currentRoles = await userManager.GetRolesAsync(user);
            //if (currentRoles.Any())
            //{
            //    await userManager.RemoveFromRolesAsync(user, currentRoles);
            //}

            //// Add the new role
            //var addRoleResult = await userManager.AddToRoleAsync(user, newRole);
            //if (!addRoleResult.Succeeded)
            //{
            //    logger.LogError("Failed to add role '{NewRole}' to user '{UserId}': {@Errors}", newRole, userId, addRoleResult.Errors);
            //    return UserErrors.UpdateRoleFailed(addRoleResult.Errors);
            //}

            //// Invalidate all refresh tokens for this user
            //var refreshTokens = await dbContext.RefreshTokens
            //    .Where(rt => rt.UserId == userId && !rt.Invalidated)
            //    .ToListAsync(cancellationToken);

            //foreach (var refreshToken in refreshTokens)
            //{
            //    refreshToken.Invalidated = true;
            //    refreshToken.UpdatedAtUtc = DateTime.UtcNow;

            //    // Add to memory cache for the middleware to check
            //    memoryCache.Set(refreshToken.JwtId, RevocatedTokenType.RoleChanged);
            //}

            //await dbContext.SaveChangesAsync(cancellationToken);

            //return Result.Success;
        }

        public async Task<Result<User>> GetCurrent()
        {
            if (httpContextAccessor?.HttpContext?.User == null)
            {
                return new Result<User>(new Error("401", "401", ErrorType.Unauthorized));
            }
            var userId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid");

            if (userId == null)
            {
                return new Result<User>(new Error("401", "401", ErrorType.Unauthorized));
            }

            var user = await cacheService.GetOrCreateAsync(
                $"user:{userId}",
                async ct => await dbContext.Users
                    .Include(x => x.UserGoals)
                    .FirstOrDefaultAsync(x => x.Id == userId.Value),
                TimeSpan.FromMinutes(5));


            if(user == null) return new Result<User>(new Error("401", "401", ErrorType.Unauthorized));
            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUser(UserDto user)
        {
            var current = await GetCurrent();
            var userCurrent = current.Value;
            if (userCurrent == null) return new Result<User>(new Error("404", "", ErrorType.NotFound));
            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                userCurrent.UserName = user.FullName;
            }
            if (user.DateOfBirth.HasValue)
            {
                userCurrent.DateOfBirth = user.DateOfBirth.Value;
                userCurrent.DateOfBirth = DateTime.SpecifyKind(
                        user.DateOfBirth.Value, DateTimeKind.Utc);
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                userCurrent.Email = user.Email;
            }
            if(user.BodyStats != null)
            {
                userCurrent.BodyStats = new()
                {
                    Kg = user.BodyStats.Kg,
                    Cm = user.BodyStats.Cm,
                    Fat = user.BodyStats.Fat
                };
            }
            if (user.Identity.HasValue)
            {
                userCurrent.Identity = user.Identity.Value;
            }
            if (user.WorkOutCount.HasValue)
            {
                userCurrent.WorkOutCount = user.WorkOutCount.Value;
            }
            if (user.IsPremium.HasValue)
            {
                userCurrent.IsPremium = user.IsPremium.Value;
            }
            if (user.UserRegistrationStep.HasValue)
            {
                userCurrent.UserRegistrationStep = user.UserRegistrationStep.Value;
            }
            if (user.Goals != null)
            {
                await SyncGoals(userCurrent, user.Goals);
            }
            userCurrent.UpdatedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return new Result<User>(userCurrent);
        }

        private async Task SyncGoals(User userCurrent, List<UserGoalDto> incoming)
        {
            var incomingGoals = incoming.Select(g => g.Goal).ToHashSet();
            var existingGoals = userCurrent.UserGoals.Select(g => g.Goal).ToHashSet();

            // удалить те, которых больше нет
            var toRemove = userCurrent.UserGoals
                .Where(g => !incomingGoals.Contains(g.Goal))
                .ToList();
            foreach (var g in toRemove)
                userCurrent.UserGoals.Remove(g);
            // добавить новые
            foreach (var goalEnum in incomingGoals.Except(existingGoals))
            {
                userCurrent.UserGoals.Add(new UserGoal
                {
                    Goal = goalEnum,
                    UserId = userCurrent.Id
                });
            }
        }
    }
}
