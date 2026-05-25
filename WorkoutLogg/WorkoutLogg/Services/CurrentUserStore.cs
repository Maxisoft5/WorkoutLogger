using Modules.Users.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WorkoutLogg.Services
{
    public static class CurrentUserStore
    {
        private static readonly string FilePath =
            Path.Combine(FileSystem.AppDataDirectory, "current_user.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static UserDto? _cached; // in-memory кэш, чтобы не читать файл каждый раз

        public static async Task SetCurrentUser(UserDto user)
        {
            _cached = user;
            var json = JsonSerializer.Serialize(user, JsonOptions);
            await File.WriteAllTextAsync(FilePath, json);
        }

        public static async Task<UserDto?> GetCurrentUser()
        {
            if (_cached is not null)
                return _cached;

            if (!File.Exists(FilePath))
                return null;

            try
            {
                var json = await File.ReadAllTextAsync(FilePath);
                _cached = JsonSerializer.Deserialize<UserDto>(json, JsonOptions);
                return _cached;
            }
            catch (JsonException)
            {
                // файл повреждён — чистим
                File.Delete(FilePath);
                return null;
            }
        }

        public static void Clear()
        {
            _cached = null;
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}
