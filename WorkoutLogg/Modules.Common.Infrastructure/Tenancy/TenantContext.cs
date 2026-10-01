using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Modules.Common.Infrastructure.Tenancy;

public sealed record TenantDefinition
{
    public string Id { get; init; } = "";
    public string? Schema { get; init; }
    public string Name { get; init; } = "WorkoutLogg";
    public string[] Hosts { get; init; } = [];
    public string[] OwnerUserIds { get; init; } = [];
}

public sealed class TenantCatalog
{
    private readonly Dictionary<string, TenantDefinition> byHost = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<TenantDefinition> Tenants { get; }

    public TenantCatalog(IEnumerable<TenantDefinition> tenants)
    {
        Tenants = tenants.ToArray();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var schemas = new HashSet<string>(StringComparer.Ordinal);
        foreach (var tenant in Tenants)
        {
            if (!Regex.IsMatch(tenant.Id, "^[a-z0-9][a-z0-9_-]{0,39}$") || !ids.Add(tenant.Id))
                throw new InvalidOperationException("Tenant IDs must be unique lowercase identifiers.");
            if (tenant.Schema is null && tenant.Id != "legacy")
                throw new InvalidOperationException("Only the legacy tenant may use module schemas.");
            if (tenant.Schema is not null && (!IsValidSchema(tenant.Schema) || !schemas.Add(tenant.Schema)))
                throw new InvalidOperationException("Tenant schemas must be unique gym_ identifiers (maximum 50 characters).");
            if (tenant.Hosts.Length == 0) throw new InvalidOperationException("A tenant needs at least one host.");
            foreach (var host in tenant.Hosts)
            {
                if (host != host.Trim() || Uri.CheckHostName(host) == UriHostNameType.Unknown || !byHost.TryAdd(host, tenant))
                    throw new InvalidOperationException("Tenant hosts must be unique hostnames without protocol, port or path.");
            }
        }
    }

    public static bool IsValidSchema(string schema) => Regex.IsMatch(schema, "^gym_[a-z0-9_]{1,46}$");
    public TenantDefinition? FindByHost(string host) => byHost.GetValueOrDefault(host);
}

// One immutable selection per request/background scope. Never accept a schema from client input.
public sealed class TenantContext
{
    private TenantDefinition? current;
    public TenantDefinition Current => current ?? throw new InvalidOperationException("Tenant has not been resolved.");
    public void Set(TenantDefinition tenant)
    {
        if (current is not null) throw new InvalidOperationException("Tenant cannot change within a scope.");
        current = tenant;
    }

    public bool Accepts(ClaimsPrincipal principal) =>
        principal.FindFirst("tenant_id")?.Value == Current.Id ||
        (Current.Id == "legacy" && Current.Schema is null && !principal.HasClaim(c => c.Type == "tenant_id"));
}
