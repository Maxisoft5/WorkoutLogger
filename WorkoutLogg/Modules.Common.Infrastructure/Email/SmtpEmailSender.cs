using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Modules.Common.Infrastructure.Email;

public class SmtpEmailSender(SmtpSettings settings) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.Auto, ct);
        await client.AuthenticateAsync(settings.UserName, settings.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
