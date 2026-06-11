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

        public async Task<UserBodyStatsDto?> GetCachedBodyStatsAsync()
        {
            var profile = await GetCachedProfileAsync();
            return profile?.BodyStats;
        }

        public async Task<bool> UpdateBodyStatsAsync(double? kg = null, double? cm = null, double? fat = null)
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return false;

            var current = (await GetCachedProfileAsync())?.BodyStats;

            var newStats = new UserBodyStatsDto
            {
                Kg = kg.HasValue ? (int)Math.Round(kg.Value) : (current?.Kg ?? 0),
                Cm = cm.HasValue ? (int)Math.Round(cm.Value) : (current?.Cm ?? 0),
                Fat = fat.HasValue ? fat.Value : (current?.Fat ?? 0),
            };

            try
            {
                var resp = await _api.UpdateAccount($"Bearer {token}", new UserDto { BodyStats = newStats });
                if (resp.IsSuccessStatusCode)
                {
                    var updated = resp.Content;
                    if (updated is not null)
                    {
                        await SecureStorage.SetAsync(CacheKey, JsonSerializer.Serialize(updated));
                    }
                    else
                    {
                        var cached = await GetCachedProfileAsync();
                        if (cached is not null)
                        {
                            cached.BodyStats = newStats;
                            await SecureStorage.SetAsync(CacheKey, JsonSerializer.Serialize(cached));
                        }
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

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
                        await CurrentUserStore.SetCurrentUser(resp.Content);

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

        public async Task<bool> UpdateProfilePictureAsync(string dataUrl)
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return false;

            try
            {
                var resp = await _api.UpdateAccount($"Bearer {token}", new UserDto { ProfilePicture = dataUrl });
                if (resp.IsSuccessStatusCode)
                {
                    var cached = await GetCachedProfileAsync();
                    if (cached is not null)
                    {
                        cached.ProfilePicture = dataUrl;
                        await SecureStorage.SetAsync(CacheKey, JsonSerializer.Serialize(cached));
                        await CurrentUserStore.SetCurrentUser(cached);
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        public void ClearCache()
        {
            SecureStorage.Remove(CacheKey);
            SecureStorage.Remove(JoinedKey);
        }
    }
}
