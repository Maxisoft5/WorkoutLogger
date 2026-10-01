using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace WorkoutLogger.WebApi.Site;

public sealed class AccessArea
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public long Version { get; set; } = 1;
}
public sealed class AccessCard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = "";
    public string CodeHash { get; set; } = "";
    public string Label { get; set; } = "";
    public bool IsActive { get; set; } = true;
}
public sealed class ClubPresence
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Entry operation ID for retry protection.
    public string UserId { get; set; } = "";
    public Guid MembershipId { get; set; }
    public Guid AreaId { get; set; }
    public DateTime EnteredAt { get; set; }
    public DateTime? ExitedAt { get; set; }
    public string EntryActorId { get; set; } = "";
    public string? ExitActorId { get; set; }
    public string? ExitNote { get; set; }
    public Guid? ExitOperationId { get; set; }
    public long Version { get; set; } = 1;
}
public static class ClubAccessModel
{
    public static string HashCode(string code) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code.Trim().ToUpperInvariant())));
    public static void Configure(ModelBuilder model)
    {
        model.Entity<AccessArea>(e => { e.ToTable("access_areas"); e.HasKey(x => x.Id); e.Property(x => x.Version).IsConcurrencyToken(); });
        model.Entity<AccessCard>(e => { e.ToTable("access_cards"); e.HasKey(x => x.Id); e.HasIndex(x => x.CodeHash).IsUnique(); e.HasIndex(x => x.UserId); });
        model.Entity<ClubPresence>(e => { e.ToTable("club_presence"); e.HasKey(x => x.Id); e.Property(x => x.Version).IsConcurrencyToken();
            e.HasIndex(x => x.UserId).IsUnique().HasFilter("\"ExitedAt\" IS NULL");
            e.HasIndex(x => x.EnteredAt);
            e.HasIndex(x => x.ExitOperationId).IsUnique();
            e.HasOne<ClubMembership>().WithMany().HasForeignKey(x => x.MembershipId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<AccessArea>().WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict); });
    }
}
