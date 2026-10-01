using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace WorkoutLogger.WebApi.Site.Migrations;

[DbContext(typeof(SiteDbContext))]
[Migration("20260927000100_ClubMemberships")]
public sealed class ClubMemberships : Migration
{
    protected override void Up(MigrationBuilder b)
    {
        b.CreateTable("membership_plans", schema: "users", columns: t => new {
            Id = t.Column<Guid>(type: "uuid", nullable: false),
            TermsJson = t.Column<string>(type: "jsonb", nullable: false),
            IsActive = t.Column<bool>(type: "boolean", nullable: false),
            Version = t.Column<long>(type: "bigint", nullable: false),
        }, constraints: t => { t.PrimaryKey("PK_membership_plans", x => x.Id);
        });
        b.CreateTable("club_memberships", schema: "users", columns: t => new {
            Id = t.Column<Guid>(type: "uuid", nullable: false),
            PlanId = t.Column<Guid>(type: "uuid", nullable: false),
            UserId = t.Column<string>(type: "text", nullable: false),
            TermsJson = t.Column<string>(type: "jsonb", nullable: false),
            StartsAt = t.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            ExpiresAt = t.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            FrozenUntil = t.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            UsedFreezeDays = t.Column<int>(type: "integer", nullable: false),
            UsedVisits = t.Column<int>(type: "integer", nullable: false),
            IsCancelled = t.Column<bool>(type: "boolean", nullable: false),
            Version = t.Column<long>(type: "bigint", nullable: false),
        }, constraints: t => { t.PrimaryKey("PK_club_memberships", x => x.Id);
            t.ForeignKey("FK_club_memberships_membership_plans_PlanId", x => x.PlanId, principalSchema: "users", principalTable: "membership_plans", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        });
        b.CreateTable("membership_events", schema: "users", columns: t => new {
            Id = t.Column<Guid>(type: "uuid", nullable: false),
            MembershipId = t.Column<Guid>(type: "uuid", nullable: false),
            OperationId = t.Column<Guid>(type: "uuid", nullable: false),
            Kind = t.Column<string>(type: "text", nullable: false),
            ActorId = t.Column<string>(type: "text", nullable: false),
            Note = t.Column<string>(type: "text", nullable: false),
            CreatedAt = t.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        }, constraints: t => { t.PrimaryKey("PK_membership_events", x => x.Id);
            t.ForeignKey("FK_membership_events_club_memberships_MembershipId", x => x.MembershipId, principalSchema: "users", principalTable: "club_memberships", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        });
        b.CreateTable("access_areas", schema: "users", columns: t => new {
            Id = t.Column<Guid>(type: "uuid", nullable: false),
            Name = t.Column<string>(type: "text", nullable: false),
            IsActive = t.Column<bool>(type: "boolean", nullable: false),
            Version = t.Column<long>(type: "bigint", nullable: false),
        }, constraints: t => { t.PrimaryKey("PK_access_areas", x => x.Id);
        });
        b.CreateTable("access_cards", schema: "users", columns: t => new {
            Id = t.Column<Guid>(type: "uuid", nullable: false),
            UserId = t.Column<string>(type: "text", nullable: false),
            CodeHash = t.Column<string>(type: "text", nullable: false),
            Label = t.Column<string>(type: "text", nullable: false),
            IsActive = t.Column<bool>(type: "boolean", nullable: false),
        }, constraints: t => { t.PrimaryKey("PK_access_cards", x => x.Id);
        });
        b.CreateTable("club_presence", schema: "users", columns: t => new {
            Id = t.Column<Guid>(type: "uuid", nullable: false),
            UserId = t.Column<string>(type: "text", nullable: false),
            MembershipId = t.Column<Guid>(type: "uuid", nullable: false),
            AreaId = t.Column<Guid>(type: "uuid", nullable: false),
            EnteredAt = t.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            ExitedAt = t.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            EntryActorId = t.Column<string>(type: "text", nullable: false),
            ExitActorId = t.Column<string>(type: "text", nullable: true),
            ExitNote = t.Column<string>(type: "text", nullable: true),
            ExitOperationId = t.Column<Guid>(type: "uuid", nullable: true),
            Version = t.Column<long>(type: "bigint", nullable: false),
        }, constraints: t => { t.PrimaryKey("PK_club_presence", x => x.Id);
            t.ForeignKey("FK_club_presence_club_memberships_MembershipId", x => x.MembershipId, principalSchema: "users", principalTable: "club_memberships", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            t.ForeignKey("FK_club_presence_access_areas_AreaId", x => x.AreaId, principalSchema: "users", principalTable: "access_areas", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        });
        b.CreateIndex("IX_club_memberships_UserId", "club_memberships", columns: new[] { "UserId" }, schema: "users", unique: false);
        b.CreateIndex("IX_club_memberships_PlanId", "club_memberships", columns: new[] { "PlanId" }, schema: "users", unique: false);
        b.CreateIndex("IX_membership_events_MembershipId_OperationId", "membership_events", columns: new[] { "MembershipId", "OperationId" }, schema: "users", unique: true);
        b.CreateIndex("IX_membership_events_MembershipId_CreatedAt", "membership_events", columns: new[] { "MembershipId", "CreatedAt" }, schema: "users", unique: false);
        b.CreateIndex("IX_access_cards_CodeHash", "access_cards", columns: new[] { "CodeHash" }, schema: "users", unique: true);
        b.CreateIndex("IX_access_cards_UserId", "access_cards", columns: new[] { "UserId" }, schema: "users", unique: false);
        b.CreateIndex("IX_club_presence_UserId", "club_presence", columns: new[] { "UserId" }, schema: "users", unique: true, filter: "\"ExitedAt\" IS NULL");
        b.CreateIndex("IX_club_presence_EnteredAt", "club_presence", columns: new[] { "EnteredAt" }, schema: "users", unique: false);
        b.CreateIndex("IX_club_presence_MembershipId", "club_presence", columns: new[] { "MembershipId" }, schema: "users", unique: false);
        b.CreateIndex("IX_club_presence_AreaId", "club_presence", columns: new[] { "AreaId" }, schema: "users", unique: false);
        b.CreateIndex("IX_club_presence_ExitOperationId", "club_presence", columns: new[] { "ExitOperationId" }, schema: "users", unique: true);
    }
    protected override void Down(MigrationBuilder b)
    {
        b.DropTable("club_presence", "users");
        b.DropTable("access_cards", "users");
        b.DropTable("access_areas", "users");
        b.DropTable("membership_events", "users");
        b.DropTable("club_memberships", "users");
        b.DropTable("membership_plans", "users");
    }
    protected override void BuildTargetModel(ModelBuilder b) => BuildMembershipModel(b);
    internal static void BuildMembershipModel(ModelBuilder b)
    {
        SiteDbContextModelSnapshot.BuildInitialModel(b);
        b.Entity("WorkoutLogger.WebApi.Site.MembershipPlan", e => {
            e.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            e.Property<string>("TermsJson").IsRequired().HasColumnType("jsonb");
            e.Property<bool>("IsActive").HasColumnType("boolean");
            e.Property<long>("Version").IsConcurrencyToken().HasColumnType("bigint");
            e.HasKey("Id");
            e.ToTable("membership_plans", "users");
        });
        b.Entity("WorkoutLogger.WebApi.Site.ClubMembership", e => {
            e.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            e.Property<Guid>("PlanId").HasColumnType("uuid");
            e.Property<string>("UserId").IsRequired().HasColumnType("text");
            e.Property<string>("TermsJson").IsRequired().HasColumnType("jsonb");
            e.Property<DateTime>("StartsAt").HasColumnType("timestamp with time zone");
            e.Property<DateTime>("ExpiresAt").HasColumnType("timestamp with time zone");
            e.Property<DateTime?>("FrozenUntil").HasColumnType("timestamp with time zone");
            e.Property<int>("UsedFreezeDays").HasColumnType("integer");
            e.Property<int>("UsedVisits").HasColumnType("integer");
            e.Property<bool>("IsCancelled").HasColumnType("boolean");
            e.Property<long>("Version").IsConcurrencyToken().HasColumnType("bigint");
            e.HasKey("Id");
            e.HasIndex("UserId");
            e.HasIndex("PlanId");
            e.ToTable("club_memberships", "users");
        });
        b.Entity("WorkoutLogger.WebApi.Site.MembershipEvent", e => {
            e.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            e.Property<Guid>("MembershipId").HasColumnType("uuid");
            e.Property<Guid>("OperationId").HasColumnType("uuid");
            e.Property<string>("Kind").IsRequired().HasColumnType("text");
            e.Property<string>("ActorId").IsRequired().HasColumnType("text");
            e.Property<string>("Note").IsRequired().HasColumnType("text");
            e.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");
            e.HasKey("Id");
            e.HasIndex("MembershipId", "OperationId").IsUnique();
            e.HasIndex("MembershipId", "CreatedAt");
            e.ToTable("membership_events", "users");
        });
        b.Entity("WorkoutLogger.WebApi.Site.AccessArea", e => {
            e.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            e.Property<string>("Name").IsRequired().HasColumnType("text");
            e.Property<bool>("IsActive").HasColumnType("boolean");
            e.Property<long>("Version").IsConcurrencyToken().HasColumnType("bigint");
            e.HasKey("Id");
            e.ToTable("access_areas", "users");
        });
        b.Entity("WorkoutLogger.WebApi.Site.AccessCard", e => {
            e.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            e.Property<string>("UserId").IsRequired().HasColumnType("text");
            e.Property<string>("CodeHash").IsRequired().HasColumnType("text");
            e.Property<string>("Label").IsRequired().HasColumnType("text");
            e.Property<bool>("IsActive").HasColumnType("boolean");
            e.HasKey("Id");
            e.HasIndex("CodeHash").IsUnique();
            e.HasIndex("UserId");
            e.ToTable("access_cards", "users");
        });
        b.Entity("WorkoutLogger.WebApi.Site.ClubPresence", e => {
            e.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            e.Property<string>("UserId").IsRequired().HasColumnType("text");
            e.Property<Guid>("MembershipId").HasColumnType("uuid");
            e.Property<Guid>("AreaId").HasColumnType("uuid");
            e.Property<DateTime>("EnteredAt").HasColumnType("timestamp with time zone");
            e.Property<DateTime?>("ExitedAt").HasColumnType("timestamp with time zone");
            e.Property<string>("EntryActorId").IsRequired().HasColumnType("text");
            e.Property<string?>("ExitActorId").HasColumnType("text");
            e.Property<string?>("ExitNote").HasColumnType("text");
            e.Property<Guid?>("ExitOperationId").HasColumnType("uuid");
            e.Property<long>("Version").IsConcurrencyToken().HasColumnType("bigint");
            e.HasKey("Id");
            e.HasIndex("UserId").IsUnique().HasFilter("\"ExitedAt\" IS NULL");
            e.HasIndex("EnteredAt");
            e.HasIndex("MembershipId");
            e.HasIndex("AreaId");
            e.HasIndex("ExitOperationId").IsUnique();
            e.ToTable("club_presence", "users");
        });
        b.Entity("WorkoutLogger.WebApi.Site.ClubMembership", e => e.HasOne("WorkoutLogger.WebApi.Site.MembershipPlan", null).WithMany().HasForeignKey("PlanId").OnDelete(DeleteBehavior.Restrict).IsRequired());
        b.Entity("WorkoutLogger.WebApi.Site.MembershipEvent", e => e.HasOne("WorkoutLogger.WebApi.Site.ClubMembership", null).WithMany().HasForeignKey("MembershipId").OnDelete(DeleteBehavior.Restrict).IsRequired());
        b.Entity("WorkoutLogger.WebApi.Site.ClubPresence", e => e.HasOne("WorkoutLogger.WebApi.Site.ClubMembership", null).WithMany().HasForeignKey("MembershipId").OnDelete(DeleteBehavior.Restrict).IsRequired());
        b.Entity("WorkoutLogger.WebApi.Site.ClubPresence", e => e.HasOne("WorkoutLogger.WebApi.Site.AccessArea", null).WithMany().HasForeignKey("AreaId").OnDelete(DeleteBehavior.Restrict).IsRequired());
    }
}

