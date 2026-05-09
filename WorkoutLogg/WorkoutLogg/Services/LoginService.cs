using Modules.Users.Domain.Constants;

namespace WorkoutLogg.Services
{
    public static class LoginService
    {
        public static async Task<bool> IsAuthenticated()
        {
            var token = await SecureStorage.GetAsync(UsersContants.CurrentToken);
            return !string.IsNullOrWhiteSpace(token);
        }

        public static async Task AddToken(string token)
        {
            await SecureStorage.SetAsync(UsersContants.CurrentToken, token);
        }

        public static async Task AddRefreshToken(string refresh)
        {
            await SecureStorage.SetAsync(UsersContants.RefreshToken, refresh);
        }

        public static async Task<string> GetActiveToken()
        {
            return await SecureStorage.GetAsync(UsersContants.CurrentToken) ?? "";
        }

        public static async Task<string> GetRefreshToken()
        {
            return await SecureStorage.GetAsync(UsersContants.RefreshToken) ?? "";
        }

        public static void RemoveToken()
        {
            SecureStorage.Remove(UsersContants.CurrentToken);
        }

        public static void RemoveRefreshToken()
        {
            SecureStorage.Remove(UsersContants.RefreshToken);
        }
    }
}
