using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WorkoutLogger.WebApi.Services
{
    public class AiSettings
    {
        public string ApiKey    { get; set; } = "";
        public string BaseUrl   { get; set; } = "https://api.groq.com/openai/v1";
        public string Model     { get; set; } = "llama-3.3-70b-versatile";
        public int    MaxTokens { get; set; } = 1024;
    }

    public class AiChatService(HttpClient http, AiSettings settings, ILogger<AiChatService> logger)
    {
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public async Task<string> ChatAsync(
            string systemPrompt,
            IEnumerable<(string role, string content)> messages,
            CancellationToken ct = default)
        {
            var allMessages = new List<object> { new { role = "system", content = systemPrompt } };
            allMessages.AddRange(messages.Select(m => (object)new { role = m.role, content = m.content }));

            var body = new
            {
                model      = settings.Model,
                max_tokens = settings.MaxTokens,
                messages   = allMessages,
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, $"{settings.BaseUrl.TrimEnd('/')}/chat/completions");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
            req.Content = JsonContent.Create(body, options: JsonOpts);

            using var resp = await http.SendAsync(req, ct);

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync(ct);
                logger.LogError("AI provider API error {Status}: {Body}", resp.StatusCode, err);
                resp.EnsureSuccessStatusCode();
            }

            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>(JsonOpts, ct);
            return doc.GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString() ?? "";
        }
    }
}
