namespace Modules.Common.Infrastructure.Email;

public record EmailPayload(string To, string Subject, string Body);
