using System.Net.Http.Json;

namespace Modules.Common.Infrastructure.Email;

public class MailtrapHttpEmailSender(MailtrapSettings settings, HttpClient httpClient) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var payload = new
        {
            from = new { email = settings.From, name = settings.FromName },
            to = new[] { new { email = to } },
            subject,
            html = body
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, settings.ApiUrl);
        request.Headers.Add("Authorization", $"Bearer {settings.ApiToken}");
        request.Content = JsonContent.Create(payload);

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}
