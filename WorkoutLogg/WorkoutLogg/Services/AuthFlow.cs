using Modules.Users.Domain.Authentication;
using Modules.Users.Infrastructure.Api;

namespace WorkoutLogg.Services
{
    public class AuthFlow : IAuthFlow
    {
        private IAuthRefreshApi AuthApi { get; set; }
        
        public AuthFlow(IAuthRefreshApi authApi)
        {
            AuthApi = authApi;
        }
        public void SignOutAndRedirectToLogin()
        {
            LoginService.RemoveToken();
            LoginService.RemoveRefreshToken();
            Application.Current?.Windows[0].Page = new LoginPage();
        }

        public async Task<bool> TryRefreshAsync(CancellationToken ct)
        {
            var refresh = await LoginService.GetRefreshToken();
            if (string.IsNullOrEmpty(refresh)) return false;
            var token = await LoginService.GetActiveToken();

            var resp = await AuthApi.Refresh(refresh, new RefreshTokenRequest() { Token = token, RefreshToken = refresh });

            if (!resp.IsSuccessStatusCode) return false;

            var pair = resp.Content?.Value;
            if (pair == null) return false;

            await LoginService.AddToken(pair.Token);
            await LoginService.AddRefreshToken(pair.RefreshToken);
            return true;
        }
    }
}
