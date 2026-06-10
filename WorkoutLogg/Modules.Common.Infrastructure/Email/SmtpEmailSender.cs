using System.Net;
using System.Net.Mail;

namespace Modules.Common.Infrastructure.Email;

public class SmtpEmailSender(SmtpSettings settings) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.EnableSsl,
            Credentials = new NetworkCredential(settings.UserName, settings.Password)
        };

        var message = new MailMessage(settings.From, to, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message, ct);
    }
}
