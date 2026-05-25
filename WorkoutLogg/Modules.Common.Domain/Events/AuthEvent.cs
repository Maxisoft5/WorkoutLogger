using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Domain.Events
{
    public record AuthEvent
    {
        public required string EventType { get; init; }    // "user.registered", "user.login", "user.login_failed"
        public string UserId { get; init; }
        public required string Email { get; init; }
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = new();
    }
}
