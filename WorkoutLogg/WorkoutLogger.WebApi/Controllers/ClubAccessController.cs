using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Infrastructure.Database;
using WorkoutLogger.WebApi.Site;

namespace WorkoutLogger.WebApi.Controllers;

public sealed record SaveArea([Required, StringLength(80)] string Name, bool IsActive, long Version);
public sealed record RegisterCard([Required] string UserId, [Required, StringLength(128, MinimumLength = 4)] string Code, [Required, StringLength(80)] string Label);
public sealed record ScanCard(Guid OperationId, [Required, StringLength(128, MinimumLength = 4)] string Code, Guid AreaId, bool Exit);
public sealed record ClosePresence(long Version, [Required, StringLength(500)] string Note);

[ApiController, Authorize(Policy = "GymAdmin")]
public sealed class ClubAccessController(SiteDbContext db, UsersDbContext users) : ControllerBase
{
    private string Actor => User.FindFirst("userid")!.Value;
    [HttpGet("api/admin/access/areas")]
    public async Task<IActionResult> Areas(CancellationToken ct) => Ok(await db.AccessAreas.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct));
    [HttpPut("api/admin/access/areas/{id:guid}")]
    public async Task<IActionResult> SaveArea(Guid id, SaveArea request, CancellationToken ct)
    {
        if (id == Guid.Empty || string.IsNullOrWhiteSpace(request.Name)) return BadRequest();
        var area = await db.AccessAreas.FindAsync([id], ct);
        if ((area?.Version ?? 0) != request.Version) return Conflict(new { message = "Услуга уже изменена. Обновите страницу." });
        if (area is null) { area = new() { Id = id, Version = 0 }; db.Add(area); }
        area.Name = request.Name.Trim(); area.IsActive = request.IsActive; area.Version++;
        return await Save(ct);
    }
    [HttpGet("api/admin/access/cards")]
    public async Task<IActionResult> Cards([Required] string userId, CancellationToken ct) => Ok(await db.AccessCards.AsNoTracking()
        .Where(x => x.UserId == userId).Select(x => new { x.Id, x.UserId, x.Label, x.IsActive }).ToListAsync(ct));
    [HttpPost("api/admin/access/cards")]
    public async Task<IActionResult> Register(RegisterCard request, CancellationToken ct)
    {
        if (request.Code.Trim().Length < 4 || string.IsNullOrWhiteSpace(request.Label)) return BadRequest();
        if (!await users.Users.AnyAsync(x => x.Id == request.UserId, ct)) return NotFound();
        db.Add(new AccessCard { UserId = request.UserId, Label = request.Label.Trim(), CodeHash = ClubAccessModel.HashCode(request.Code) });
        return await Save(ct);
    }
    [HttpPost("api/admin/access/cards/{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken ct)
    {
        var card = await db.AccessCards.FindAsync([id], ct);
        if (card is null) return NotFound();
        card.IsActive = false;
        return await Save(ct);
    }
    [HttpGet("api/admin/access/presence")]
    public async Task<IActionResult> Presence(CancellationToken ct)
    {
        var inside = await db.Presences.AsNoTracking().Where(x => x.ExitedAt == null).OrderBy(x => x.EnteredAt).ToListAsync(ct);
        var ids = inside.Select(x => x.UserId).Distinct().ToArray();
        var names = await users.Users.AsNoTracking().Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.UserName, ct);
        var areas = await db.AccessAreas.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        return Ok(new { count = inside.Count, people = inside.Select(x => new { x.Id, x.UserId, name = names.GetValueOrDefault(x.UserId),
            x.AreaId, areaName = areas.GetValueOrDefault(x.AreaId), x.EnteredAt, x.Version }) });
    }
    [HttpPost("api/admin/access/scan")]
    public async Task<IActionResult> Scan(ScanCard request, CancellationToken ct)
    {
        if (request.OperationId == Guid.Empty) return BadRequest();
        var hash = ClubAccessModel.HashCode(request.Code);
        var card = await db.AccessCards.AsNoTracking().SingleOrDefaultAsync(x => x.CodeHash == hash && x.IsActive, ct);
        if (card is null) return BadRequest(new { message = "Карта не зарегистрирована или заблокирована." });
        var present = await db.Presences.SingleOrDefaultAsync(x => x.UserId == card.UserId && x.ExitedAt == null, ct);
        if (request.Exit)
        {
            var previousExit = await db.Presences.AsNoTracking().SingleOrDefaultAsync(x => x.ExitOperationId == request.OperationId, ct);
            if (previousExit is not null) return previousExit.UserId == card.UserId
                ? Ok(new { message = "Этот выход уже зарегистрирован." }) : Conflict(new { message = "Номер операции уже использован." });
            if (present is null) return Ok(new { message = "Участник уже вне клуба." });
            present.ExitedAt = DateTime.UtcNow; present.ExitActorId = Actor; present.ExitNote = "Выход по карте"; present.ExitOperationId = request.OperationId; present.Version++;
            return await Save(ct, "Выход зарегистрирован.");
        }
        var previousEntry = await db.Presences.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.OperationId, ct);
        if (previousEntry is not null) return previousEntry.UserId == card.UserId && previousEntry.AreaId == request.AreaId
            ? Ok(new { message = "Этот вход уже зарегистрирован." }) : Conflict(new { message = "Номер операции уже использован." });
        if (present is not null) return BadRequest(new { message = "Участник уже внутри. Повторное посещение не списано." });
        if (!await db.AccessAreas.AnyAsync(x => x.Id == request.AreaId && x.IsActive, ct)) return BadRequest(new { message = "Выберите действующую услугу." });
        var now = DateTime.UtcNow;
        var passes = await db.Memberships.Where(x => x.UserId == card.UserId && !x.IsCancelled && x.StartsAt <= now && x.ExpiresAt > now)
            .OrderBy(x => x.ExpiresAt).ThenBy(x => x.Id).ToListAsync(ct);
        ClubMembership? selected = null;
        foreach (var pass in passes.Where(x => MembershipRules.Allows(x, request.AreaId)))
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(MembershipRules.Terms(pass.TermsJson).TimeZone);
            var today = TimeZoneInfo.ConvertTimeFromUtc(now, zone).Date;
            var recent = await db.MembershipEvents.Where(x => x.MembershipId == pass.Id && x.Kind == "visit" && x.CreatedAt >= now.AddDays(-2)).Select(x => x.CreatedAt).ToListAsync(ct);
            if (MembershipRules.VisitError(pass, now, recent.Count(x => TimeZoneInfo.ConvertTimeFromUtc(x, zone).Date == today)) is null) { selected = pass; break; }
        }
        if (selected is null) return BadRequest(new { message = "Нет доступного абонемента для этой услуги: проверьте срок, заморозку, дни недели и остаток посещений." });
        selected.UsedVisits++; selected.Version++;
        db.Add(new MembershipEvent { MembershipId = selected.Id, OperationId = request.OperationId, Kind = "visit", ActorId = Actor,
            Note = "Вход по карте", CreatedAt = now });
        db.Add(new ClubPresence { Id = request.OperationId, MembershipId = selected.Id, UserId = card.UserId, AreaId = request.AreaId,
            EnteredAt = now, EntryActorId = Actor });
        return await Save(ct, "Вход разрешён. Посещение зарегистрировано.");
    }
    [HttpPost("api/admin/access/presence/{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, ClosePresence request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Note)) return BadRequest();
        var present = await db.Presences.FindAsync([id], ct);
        if (present is null) return NotFound();
        if (present.ExitedAt is not null || present.Version != request.Version) return Conflict(new { message = "Запись уже изменена." });
        present.ExitedAt = DateTime.UtcNow; present.ExitActorId = Actor; present.ExitNote = request.Note; present.Version++;
        return await Save(ct);
    }
    private async Task<IActionResult> Save(CancellationToken ct, string message = "Сохранено")
    {
        try { await db.SaveChangesAsync(ct); return Ok(new { message }); }
        catch (DbUpdateConcurrencyException) { return Conflict(new { message = "Данные изменились. Обновите список." }); }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        { return Conflict(new { message = "Карта или операция уже зарегистрирована. Обновите список." }); }
    }
}
