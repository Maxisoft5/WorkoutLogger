using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Common.Infrastructure.Tenancy;

namespace WorkoutLogger.WebApi.Site;

public sealed class SiteSettings
{
    public int Id { get; set; } = 1;
    public string Draft { get; set; } = "{}";
    public string Published { get; set; } = "{}";
    public long Version { get; set; }
}

public sealed class SiteDbContext(DbContextOptions<SiteDbContext> options, TenantContext? tenant = null)
    : DbContext(options), ITenantDbContext
{
    public string Schema { get; } = tenant?.Current.Schema ?? "users";
    public DbSet<SiteSettings> Settings => Set<SiteSettings>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<ClubMembership> Memberships => Set<ClubMembership>();
    public DbSet<MembershipEvent> MembershipEvents => Set<MembershipEvent>();
    public DbSet<AccessArea> AccessAreas => Set<AccessArea>();
    public DbSet<AccessCard> AccessCards => Set<AccessCard>();
    public DbSet<ClubPresence> Presences => Set<ClubPresence>();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        MembershipModel.Configure(modelBuilder);
        ClubAccessModel.Configure(modelBuilder);
        modelBuilder.Entity<SiteSettings>(entity =>
        {
            entity.ToTable("site_settings");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Draft).HasColumnType("jsonb");
            entity.Property(x => x.Published).HasColumnType("jsonb");
            entity.Property(x => x.Version).IsConcurrencyToken();
        });
    }
}
