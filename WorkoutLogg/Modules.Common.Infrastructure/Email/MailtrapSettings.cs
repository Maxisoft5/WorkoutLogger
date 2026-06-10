namespace Modules.Common.Infrastructure.Email;

public class MailtrapSettings
{
    public string ApiUrl { get; set; } = "https://send.api.mailtrap.io/api/send";
    public string ApiToken { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string FromName { get; set; } = "WorkoutLogger";
}
