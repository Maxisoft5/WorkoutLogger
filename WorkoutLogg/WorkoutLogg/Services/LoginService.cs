using Modules.Users.Domain.Constants;
using Modules.Users.DTO.Auth;

namespace WorkoutLogg.Services
{
    public static class LoginService
    {
        public static async Task<bool> IsAuthenticated()
        {
            var token = await SecureStorage.GetAsync(UsersConstants.CurrentToken);
            return !string.IsNullOrWhiteSpace(token);
        }

        public static async Task AddToken(string token)
        {
            await SecureStorage.SetAsync(UsersConstants.CurrentToken, token);
        }

        public static async Task AddRefreshToken(string refresh)
        {
            await SecureStorage.SetAsync(UsersConstants.RefreshToken, refresh);
        }

        public static async Task<string> GetActiveToken()
        {
            return await SecureStorage.GetAsync(UsersConstants.CurrentToken) ?? "";
        }

        public static async Task<string> GetRefreshToken()
        {
            return await SecureStorage.GetAsync(UsersConstants.RefreshToken) ?? "";
        }

        public static void RemoveToken()
        {
            SecureStorage.Remove(UsersConstants.CurrentToken);
        }

        public static void RemoveRefreshToken()
        {
            SecureStorage.Remove(UsersConstants.RefreshToken);
        }

    }
}
