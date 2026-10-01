using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WorkoutLogger.WebApi.Site.Migrations;

[DbContext(typeof(SiteDbContext))]
[Migration("20260926000100_InitialSite")]
public sealed class InitialSite : Migration
{
    protected override void Up(MigrationBuilder builder)
    {
        builder.EnsureSchema("users");
        builder.CreateTable("site_settings", schema: "users", columns: table => new
        {
            Id = table.Column<int>(type: "integer", nullable: false),
            Draft = table.Column<string>(type: "jsonb", nullable: false),
            Published = table.Column<string>(type: "jsonb", nullable: false),
            Version = table.Column<long>(type: "bigint", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_site_settings", x => x.Id));
    }
    protected override void Down(MigrationBuilder builder) => builder.DropTable("site_settings", "users");
    protected override void BuildTargetModel(ModelBuilder builder) => SiteDbContextModelSnapshot.BuildInitialModel(builder);
}

[DbContext(typeof(SiteDbContext))]
public sealed class SiteDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder builder) => ClubMemberships.BuildMembershipModel(builder);
    internal static void BuildInitialModel(ModelBuilder builder)
    {
        builder.HasDefaultSchema("users").HasAnnotation("ProductVersion", "10.0.7")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);
        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(builder);
        builder.Entity("WorkoutLogger.WebApi.Site.SiteSettings", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedNever().HasColumnType("integer");
            entity.Property<string>("Draft").IsRequired().HasColumnType("jsonb");
            entity.Property<string>("Published").IsRequired().HasColumnType("jsonb");
            entity.Property<long>("Version").IsConcurrencyToken().HasColumnType("bigint");
            entity.HasKey("Id");
            entity.ToTable("site_settings", "users");
        });
    }
}
