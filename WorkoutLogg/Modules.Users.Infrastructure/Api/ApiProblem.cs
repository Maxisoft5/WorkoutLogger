using System.Text.Json;
using Refit;

namespace Modules.Users.Infrastructure.Api
{
    /// <summary>
    /// Extracts a human-readable message from an RFC 7807 ProblemDetails
    /// error body returned by the API.
    /// </summary>
    public static class ApiProblem
    {
        public static string GetDetail(IApiResponse response, string fallback)
        {
            var content = response.Error?.Content;
            if (string.IsNullOrWhiteSpace(content)) return fallback;

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind != JsonValueKind.Object) return fallback;
                if (doc.RootElement.TryGetProperty("detail", out var detail)
                    && detail.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(detail.GetString()))
                {
                    return detail.GetString()!;
                }
            }
            catch (JsonException)
            {
                // Not a JSON body — fall through to the fallback message.
            }

            return fallback;
        }
    }
}
