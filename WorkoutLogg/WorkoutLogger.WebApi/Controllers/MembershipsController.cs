using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Infrastructure.Database;
using WorkoutLogger.WebApi.Site;

namespace WorkoutLogger.WebApi.Controllers;

public sealed record SaveMembershipPlan([Required] MembershipTerms Terms, bool IsActive, [Range(0, long.MaxValue)] long Version);
public sealed record IssueMembership(Guid Id, Guid PlanId, [Required] string UserId, DateTime StartsAt, [Required, StringLength(500)] string Note);
public sealed record MembershipCommand(Guid OperationId, long Version, [Range(0, 365)] int Days, [Required, StringLength(500)] string Note);

[ApiController, Authorize]
public sealed class MembershipsController(SiteDbContext db, UsersDbContext users) : ControllerBase
{
    private string Actor => User.FindFirst("userid")!.Value;
    [HttpGet("api/memberships/areas")]
    public async Task<IActionResult> Areas(CancellationToken ct) => Ok(await db.AccessAreas.AsNoTracking()
        .Select(x => new { x.Id, x.Name, x.IsActive }).ToListAsync(ct));

    [HttpGet("api/memberships/plans")]
    public async Task<IActionResult> Plans(CancellationToken ct) => Ok((await db.MembershipPlans.AsNoTracking()
        .Where(x => x.IsActive).ToListAsync(ct)).Select(PlanView));

    [Authorize(Policy = "GymAdmin"), HttpGet("api/admin/memberships/plans")]
    public async Task<IActionResult> AdminPlans(CancellationToken ct) => Ok((await db.MembershipPlans.AsNoTracking().ToListAsync(ct)).Select(PlanView));

    [Authorize(Policy = "GymAdmin"), HttpPut("api/admin/memberships/plans/{id:guid}")]
    public async Task<IActionResult> SavePlan(Guid id, SaveMembershipPlan request, CancellationToken ct)
    {
        if (id == Guid.Empty) return BadRequest();
        if (!request.Terms.AllAreas && await db.AccessAreas.CountAsync(x => request.Terms.AreaIds.Contains(x.Id) && x.IsActive, ct) != request.Terms.AreaIds.Distinct().Count())
            return BadRequest(new { message = "Выберите действующие услуги клуба." });
        var plan = await db.MembershipPlans.FindAsync([id], ct);
        if ((plan?.Version ?? 0) != request.Version) return Changed();
        if (plan is null) { plan = new() { Id = id, Version = 0 }; db.Add(plan); }
        plan.TermsJson = JsonSerializer.Serialize(request.Terms);
        plan.IsActive = request.IsActive; plan.Version++;
        return await Save(ct);
    }

    [HttpGet("api/memberships/mine")]
    public async Task<IActionResult> Mine(CancellationToken ct) => Ok((await db.Memberships.AsNoTracking()
        .Where(x => x.UserId == Actor).OrderByDescending(x => x.StartsAt).ToListAsync(ct)).Select(x => PassView(x, null)));

    [Authorize(Policy = "GymAdmin"), HttpGet("api/admin/memberships")]
    public async Task<IActionResult> All(string? userId, int page = 1, CancellationToken ct = default)
    {
        if (page < 1 || page > 100000) return BadRequest();
        var passes = await db.Memberships.AsNoTracking().Where(x => userId == null || x.UserId == userId)
            .OrderByDescending(x => x.StartsAt).ThenBy(x => x.Id).Skip((page - 1) * 50).Take(50).ToListAsync(ct);
        var ids = passes.Select(x => x.UserId).Distinct().ToArray();
        var names = await users.Users.AsNoTracking().Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.UserName, ct);
        return Ok(passes.Select(x => PassView(x, names.GetValueOrDefault(x.UserId))));
    }

    [Authorize(Policy = "GymAdmin"), HttpPost("api/admin/memberships")]
    public async Task<IActionResult> Issue(IssueMembership request, CancellationToken ct)
    {
        if (request.Id == Guid.Empty || request.StartsAt.Kind != DateTimeKind.Utc || request.StartsAt < DateTime.UtcNow.AddDays(-1)
            || request.StartsAt > DateTime.UtcNow.AddYears(1) || string.IsNullOrWhiteSpace(request.Note))
            return BadRequest(new { message = "Укажите начало действия и основание выдачи. Дата должна быть не раньше вчера и не дальше года." });
        if (await db.Memberships.AnyAsync(x => x.Id == request.Id, ct)) return Conflict(new { message = "Абонемент с этим номером уже выдан. Обновите список." });
        var plan = await db.MembershipPlans.SingleOrDefaultAsync(x => x.Id == request.PlanId && x.IsActive, ct);
        if (plan is null || !await users.Users.AnyAsync(x => x.Id == request.UserId, ct)) return NotFound();
        var terms = MembershipRules.Terms(plan.TermsJson);
        var pass = new ClubMembership { Id = request.Id, PlanId = plan.Id, UserId = request.UserId,
            TermsJson = plan.TermsJson, StartsAt = request.StartsAt,
            ExpiresAt = terms.DurationMonths > 0 ? request.StartsAt.AddMonths(terms.DurationMonths) : request.StartsAt.AddDays(terms.DurationDays) };
        db.Add(pass); AddEvent(pass.Id, request.Id, "issued", request.Note, DateTime.UtcNow);
        return await Save(ct);
    }

    [HttpGet("api/memberships/{id:guid}/history")]
    public async Task<IActionResult> History(Guid id, CancellationToken ct, [FromServices] IAuthorizationService authorization)
    {
        var pass = await db.Memberships.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        if (pass is null) return NotFound();
        if (pass.UserId != Actor && !(await authorization.AuthorizeAsync(User, HttpContext, "GymAdmin")).Succeeded) return NotFound();
        // Internal staff notes are visible only in the administrative history.
        var events = await db.MembershipEvents.AsNoTracking().Where(x => x.MembershipId == id).OrderByDescending(x => x.CreatedAt).Take(200).ToListAsync(ct);
        var admin = (await authorization.AuthorizeAsync(User, HttpContext, "GymAdmin")).Succeeded;
        return Ok(events.Select(x => new { x.Id, x.Kind, x.CreatedAt, Note = admin ? x.Note : null }));
    }

    [Authorize(Policy = "GymAdmin"), HttpPost("api/admin/memberships/{id:guid}/{actionName}")]
    public async Task<IActionResult> Apply(Guid id, string actionName, MembershipCommand request, CancellationToken ct)
    {
        if (actionName is not ("freeze" or "cancel") || request.OperationId == Guid.Empty || string.IsNullOrWhiteSpace(request.Note))
            return BadRequest(new { message = "Укажите действие и комментарий." });
        var pass = await db.Memberships.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (pass is null) return NotFound();
        var existing = await db.MembershipEvents.AsNoTracking().SingleOrDefaultAsync(x => x.MembershipId == id && x.OperationId == request.OperationId, ct);
        if (existing is not null) return existing.Kind == actionName ? Ok(new { alreadyApplied = true }) : Changed();
        if (pass.Version != request.Version) return Changed();
        var now = DateTime.UtcNow;
        if (actionName == "freeze")
        {
            var error = MembershipRules.FreezeError(pass, now, request.Days);
            if (error is not null) return BadRequest(new { message = error });
            pass.FrozenUntil = now.AddDays(request.Days); pass.ExpiresAt = pass.ExpiresAt.AddDays(request.Days);
            pass.UsedFreezeDays += request.Days;
        }
        else
        {
            if (pass.IsCancelled) return BadRequest(new { message = "Абонемент уже отменён." });
            pass.IsCancelled = true;
        }
        pass.Version++;
        AddEvent(id, request.OperationId, actionName, request.Note, now);
        // A versioned update and its audit event are committed in the same EF transaction.
        return await Save(ct);
    }

    private void AddEvent(Guid id, Guid operation, string kind, string note, DateTime now) => db.Add(new MembershipEvent
        { MembershipId = id, OperationId = operation, Kind = kind, Note = note, ActorId = Actor, CreatedAt = now });
    private IActionResult Changed() => Conflict(new { message = "Данные уже изменены. Обновите список и повторите действие." });
    private async Task<IActionResult> Save(CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct); return Ok(new { saved = true }); }
        catch (DbUpdateConcurrencyException) { return Changed(); }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" }) { return Changed(); }
    }
    private static object PlanView(MembershipPlan p) => new { p.Id, p.Version, p.IsActive, terms = MembershipRules.Terms(p.TermsJson) };
    private static object PassView(ClubMembership p, string? name) => new { p.Id, p.UserId, name, p.PlanId, p.Version,
        terms = MembershipRules.Terms(p.TermsJson), p.StartsAt, p.ExpiresAt, p.FrozenUntil, p.UsedVisits, p.UsedFreezeDays,
        freezeDaysRemaining = MembershipRules.FreezeBudget(p) - p.UsedFreezeDays, status = MembershipRules.Status(p, DateTime.UtcNow) };
}
