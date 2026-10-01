using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WorkoutLogger.WebApi.Site;

namespace Modules.Users.Tests;

public class MembershipTests
{
    private static readonly DateTime Start = new(2026, 9, 27, 10, 0, 0, DateTimeKind.Utc);
    private static ClubMembership Pass(MembershipTerms? terms = null) => new()
    {
        StartsAt = Start, ExpiresAt = Start.AddMonths(9), TermsJson = JsonSerializer.Serialize(terms ?? new MembershipTerms
        { Name = "9 + 3", DurationMonths = 9, FreezeMonths = 3, VisitLimit = null })
    };

    [Test]
    public void FreezeBudgetUsesCalendarMonthsAndRemainingDays()
    {
        var pass = Pass();
        Assert.That(MembershipRules.FreezeBudget(pass), Is.EqualTo(91));
        pass.UsedFreezeDays = 85;
        Assert.That(MembershipRules.FreezeError(pass, Start, 6), Is.Null);
        Assert.That(MembershipRules.FreezeError(pass, Start, 7), Is.Not.Null);
    }
    [Test]
    public void FreezeEndsAtExactBoundaryAndExpiredPassNeverGrantsAccess()
    {
        var pass = Pass(); pass.FrozenUntil = Start.AddDays(5);
        Assert.That(MembershipRules.VisitError(pass, Start, 0), Is.Not.Null);
        Assert.That(MembershipRules.VisitError(pass, pass.FrozenUntil.Value, 0), Is.Null);
        Assert.That(MembershipRules.VisitError(pass, pass.ExpiresAt, 0), Is.Not.Null);
        Assert.That(MembershipRules.FreezeError(pass, pass.ExpiresAt, 1), Is.Not.Null);
    }
    [Test]
    public void LimitedAndUnlimitedPassesBothRespectDailyLimit()
    {
        var limited = Pass(new() { Name = "8", VisitLimit = 8, DailyVisitLimit = 2 });
        limited.UsedVisits = 7;
        Assert.That(MembershipRules.VisitError(limited, Start, 1), Is.Null);
        Assert.That(MembershipRules.VisitError(limited, Start, 2), Is.Not.Null);
        limited.UsedVisits++;
        Assert.That(MembershipRules.VisitError(limited, Start, 0), Is.Not.Null);
        var unlimited = Pass(); unlimited.UsedVisits = 200;
        Assert.That(MembershipRules.VisitError(unlimited, Start, 0), Is.Null);
        Assert.That(MembershipRules.VisitError(unlimited, Start, 1), Is.Not.Null);
    }
    [Test]
    public void CancelledAndFutureMembershipsDenyAccess()
    {
        var pass = Pass();
        Assert.That(MembershipRules.VisitError(pass, Start.AddTicks(-1), 0), Is.Not.Null);
        pass.IsCancelled = true;
        Assert.That(MembershipRules.VisitError(pass, Start, 0), Is.Not.Null);
    }
    [Test]
    public void PoolOnlyCannotGrantGymAccess()
    {
        var pool = Guid.NewGuid(); var gym = Guid.NewGuid();
        var pass = Pass(new() { Name = "Бассейн", AllAreas = false, AreaIds = [pool] });
        Assert.That(MembershipRules.Allows(pass, pool), Is.True);
        Assert.That(MembershipRules.Allows(pass, gym), Is.False);
        Assert.That(MembershipRules.Allows(Pass(), gym), Is.True);
    }
    [Test]
    public void WeekdayRestrictionUsesClubsLocalDate()
    {
        var pass = Pass(new() { Name = "Понедельник", Weekdays = 2, TimeZone = "Europe/Moscow" });
        // Sunday UTC becomes Monday in Moscow.
        Assert.That(MembershipRules.VisitError(pass, new DateTime(2026,9,27,22,0,0,DateTimeKind.Utc), 0), Is.Null);
        Assert.That(MembershipRules.VisitError(pass, Start, 0), Is.Not.Null);
    }
    [Test]
    public void TermsRequireAnAreaOrExplicitAllAccessAndValidTimezone()
    {
        var terms = new MembershipTerms { Name = "Test", AllAreas = false, TimeZone = "Not/AZone" };
        var errors = new List<ValidationResult>();
        Assert.That(Validator.TryValidateObject(terms, new ValidationContext(terms), errors, true), Is.False);
        Assert.That(errors.SelectMany(x => x.MemberNames), Does.Contain("TimeZone").And.Contain("AreaIds"));
    }
}
