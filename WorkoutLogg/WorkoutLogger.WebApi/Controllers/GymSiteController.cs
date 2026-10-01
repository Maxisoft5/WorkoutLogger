using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Infrastructure.Tenancy;
using WorkoutLogger.WebApi.Site;
using Modules.Trainers.Infrastructure.Database;

namespace WorkoutLogger.WebApi.Controllers;

[ApiController]
public sealed class GymSiteController(SiteDbContext db, TenantContext tenant, TrainersDbContext trainers) : ControllerBase
{
    [HttpGet("api/site")]
    public async Task<IActionResult> PublicSite(CancellationToken ct)
    {
        var settings = await db.Settings.AsNoTracking().SingleOrDefaultAsync(x => x.Id == 1, ct);
        var brand = settings is null ? new BrandSettings { Name = tenant.Current.Name } : Read(settings.Published);
        return Ok(new { tenantId = tenant.Current.Id, brand });
    }

    [Authorize, HttpGet("api/site/access")]
    public async Task<IActionResult> Access(CancellationToken ct)
    {
        var id = User.FindFirst("userid")?.Value;
        return Ok(new { isAdmin = tenant.Current.OwnerUserIds.Contains(id), isTrainer = await trainers.TrainerProfiles.AnyAsync(x => x.UserId == id && x.IsActive, ct) });
    }

    [Authorize(Policy = "GymAdmin"), HttpGet("api/admin/site")]
    public async Task<IActionResult> GetDraft(CancellationToken ct)
    {
        var settings = await db.Settings.AsNoTracking().SingleOrDefaultAsync(x => x.Id == 1, ct);
        return Ok(new { settings = settings is null ? new BrandSettings { Name = tenant.Current.Name } : Read(settings.Draft), version = settings?.Version ?? 0 });
    }

    [Authorize(Policy = "GymAdmin"), HttpPut("api/admin/site")]
    public async Task<IActionResult> Save(SaveBrandRequest request, CancellationToken ct)
    {
        var settings = await db.Settings.SingleOrDefaultAsync(x => x.Id == 1, ct);
        if ((settings?.Version ?? 0) != request.Version) return Conflict(new { message = "Настройки уже изменены. Обновите страницу." });
        if (settings is null)
        {
            settings = new SiteSettings { Published = JsonSerializer.Serialize(new BrandSettings { Name = tenant.Current.Name }) };
            db.Add(settings);
        }
        settings.Draft = JsonSerializer.Serialize(request.Settings);
        settings.Version++;
        return await Commit(settings.Version, ct);
    }

    [Authorize(Policy = "GymAdmin"), HttpPost("api/admin/site/publish")]
    public async Task<IActionResult> Publish(PublishBrandRequest request, CancellationToken ct)
    {
        var settings = await db.Settings.SingleOrDefaultAsync(x => x.Id == 1, ct);
        if (settings is null || settings.Version != request.Version) return Conflict(new { message = "Сначала сохраните актуальный черновик." });
        settings.Published = settings.Draft;
        settings.Version++;
        return await Commit(settings.Version, ct);
    }

    private async Task<IActionResult> Commit(long version, CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct); return Ok(new { version }); }
        catch (DbUpdateConcurrencyException) { return Conflict(new { message = "Настройки уже изменены. Обновите страницу." }); }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        { return Conflict(new { message = "Настройки уже созданы. Обновите страницу." }); }
    }
    private static BrandSettings Read(string json) => JsonSerializer.Deserialize<BrandSettings>(json) ?? new();
}
