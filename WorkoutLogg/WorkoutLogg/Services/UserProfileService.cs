using Modules.Users.Infrastructure.Api;
using Modules.Users.DTO.Auth;
using System.Text.Json;

namespace WorkoutLogg.Services
{
    public class UserProfileService
    {
        private const string CacheKey = "user_profile_v1";
        private const string JoinedKey = "user_joined_date";

        private readonly IAuthApi _api;

        public UserProfileService(IAuthApi api) => _api = api;

        public async Task<UserDto?> GetCachedProfileAsync()
        {
            var json = await SecureStorage.GetAsync(CacheKey);
            if (string.IsNullOrEmpty(json)) return null;
            try { return JsonSerializer.Deserialize<UserDto>(json); }
            catch { return null; }
        }

        public async Task<UserDto?> RefreshProfileAsync(CancellationToken ct = default)
        {
            var token = await LoginService.GetActiveToken();
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var resp = await _api.GetCurrentUser($"Bearer {token}");
                    if (resp.IsSuccessStatusCode && resp.Content is not null)
                    {
                        var json = JsonSerializer.Serialize(resp.Content);
                        await SecureStorage.SetAsync(CacheKey, json);

                        // Record first-time joined date
                        var existing = await SecureStorage.GetAsync(JoinedKey);
                        if (string.IsNullOrEmpty(existing))
                            await SecureStorage.SetAsync(JoinedKey, DateTime.UtcNow.ToString("o"));

                        return resp.Content;
                    }
                }
                catch { }
            }

            return await GetCachedProfileAsync();
        }

        public async Task<DateTime?> GetJoinedDateAsync()
        {
            var raw = await SecureStorage.GetAsync(JoinedKey);
            return DateTime.TryParse(raw, out var d) ? d : null;
        }

        public void ClearCache()
        {
            SecureStorage.Remove(CacheKey);
            SecureStorage.Remove(JoinedKey);
        }
    }
}
