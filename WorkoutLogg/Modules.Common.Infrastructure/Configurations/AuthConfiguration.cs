
namespace Modules.Common.Infrastructure.Configurations
{
    public record AuthConfiguration
    {
        public required string Key { get; init; }
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public int TokenExpiryMinutes { get; init; } = 60;
    }
}
