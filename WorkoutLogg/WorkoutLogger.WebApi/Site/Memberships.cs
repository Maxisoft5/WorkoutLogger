using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace WorkoutLogger.WebApi.Site;

public sealed class MembershipTerms : IValidatableObject
{
    [Required, StringLength(80)] public string Name { get; set; } = "";
    [StringLength(1500)] public string Description { get; set; } = "";
    [Range(0, 10000000)] public decimal Price { get; set; }
    [Required, RegularExpression("^[A-Z]{3}$")] public string Currency { get; set; } = "RUB";
    [Range(1, 1095)] public int DurationDays { get; set; } = 30;
    [Range(0, 36)] public int DurationMonths { get; set; } // When nonzero, calendar months replace DurationDays.
    [Range(1, 10000)] public int? VisitLimit { get; set; } = 8;
    [Range(1, 100)] public int DailyVisitLimit { get; set; } = 1;
    [Range(0, 365)] public int FreezeDays { get; set; }
    [Range(0, 12)] public int FreezeMonths { get; set; }
    public bool AllAreas { get; set; } = true;
    [Required, MaxLength(50)] public Guid[] AreaIds { get; set; } = [];
    [Range(1, 127)] public int Weekdays { get; set; } = 127; // Sunday = bit 0.
    [Required, StringLength(100)] public string TimeZone { get; set; } = "Europe/Moscow";
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(Name)) yield return new("Укажите название тарифа.", [nameof(Name)]);
        if (!AllAreas && (AreaIds is null || AreaIds.Length == 0)) yield return new("Выберите хотя бы одну услугу.", [nameof(AreaIds)]);
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(TimeZone ?? "", out _))
            yield return new("Неизвестный часовой пояс.", [nameof(TimeZone)]);
    }
}

public sealed class MembershipPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TermsJson { get; set; } = "{}";
    public bool IsActive { get; set; } = true;
    public long Version { get; set; } = 1;
}

public sealed class ClubMembership
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlanId { get; set; }
    public string UserId { get; set; } = "";
    public string TermsJson { get; set; } = "{}";
    public DateTime StartsAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? FrozenUntil { get; set; }
    public int UsedFreezeDays { get; set; }
    public int UsedVisits { get; set; }
    public bool IsCancelled { get; set; }
    public long Version { get; set; } = 1;
}

public sealed class MembershipEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MembershipId { get; set; }
    public Guid OperationId { get; set; }
    public string Kind { get; set; } = "";
    public string ActorId { get; set; } = "";
    public string Note { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public static class MembershipRules
{
    public static MembershipTerms Terms(string json) => JsonSerializer.Deserialize<MembershipTerms>(json)!;
    public static int FreezeBudget(ClubMembership pass) { var t = Terms(pass.TermsJson); return t.FreezeDays + (int)(pass.StartsAt.AddMonths(t.FreezeMonths) - pass.StartsAt).TotalDays; }
    public static bool Allows(ClubMembership pass, Guid areaId) { var t = Terms(pass.TermsJson); return t.AllAreas || t.AreaIds.Contains(areaId); }
    public static string Status(ClubMembership pass, DateTime now) => pass.IsCancelled ? "cancelled"
        : now < pass.StartsAt ? "scheduled" : now >= pass.ExpiresAt ? "expired"
        : pass.FrozenUntil > now ? "frozen"
        : Terms(pass.TermsJson).VisitLimit is int limit && pass.UsedVisits >= limit ? "exhausted" : "active";
    public static string? VisitError(ClubMembership pass, DateTime now, int visitsToday)
    {
        if (Status(pass, now) != "active") return "Абонемент не активен или посещения закончились.";
        var terms = Terms(pass.TermsJson);
        var local = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById(terms.TimeZone));
        if ((terms.Weekdays & (1 << (int)local.DayOfWeek)) == 0) return "Тариф не действует в этот день недели.";
        return visitsToday >= terms.DailyVisitLimit ? "Дневной лимит посещений исчерпан." : null;
    }
    public static string? FreezeError(ClubMembership pass, DateTime now, int days)
    {
        if (Status(pass, now) != "active") return "Заморозить можно только активный абонемент.";
        return days < 1 || days > FreezeBudget(pass) - pass.UsedFreezeDays
            ? "Недостаточно доступных дней заморозки." : null;
    }
}

public static class MembershipModel
{
    public static void Configure(ModelBuilder model)
    {
        model.Entity<MembershipPlan>(e => { e.ToTable("membership_plans"); e.HasKey(x => x.Id);
            e.Property(x => x.TermsJson).HasColumnType("jsonb"); e.Property(x => x.Version).IsConcurrencyToken(); });
        model.Entity<ClubMembership>(e => { e.ToTable("club_memberships"); e.HasKey(x => x.Id);
            e.Property(x => x.TermsJson).HasColumnType("jsonb"); e.Property(x => x.Version).IsConcurrencyToken();
            e.HasIndex(x => x.UserId); e.HasOne<MembershipPlan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict); });
        model.Entity<MembershipEvent>(e => { e.ToTable("membership_events"); e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.MembershipId, x.OperationId }).IsUnique();
            e.HasIndex(x => new { x.MembershipId, x.CreatedAt });
            e.HasOne<ClubMembership>().WithMany().HasForeignKey(x => x.MembershipId).OnDelete(DeleteBehavior.Restrict); });
    }
}
