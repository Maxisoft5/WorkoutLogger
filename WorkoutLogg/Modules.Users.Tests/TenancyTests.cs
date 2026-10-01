using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Modules.Common.Infrastructure.Tenancy;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Database;
using Modules.Subscriptions.Infrastructure.Database;
using WorkoutLogger.WebApi.Site;
using WorkoutLogger.WebApi.Tenancy;
using Npgsql;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Modules.Common.Infrastructure.Caching;
using Modules.Users.Infrastructure.Workouts;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutLogger.WebApi.Controllers;

namespace Modules.Users.Tests;

public class TenancyTests
{
    [Test]
    public async Task PasswordResetCacheIsSeparateEvenForTheSameEmail()
    {
        using var memory = new MemoryCache(new MemoryCacheOptions());
        var distributed = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var shared = new HybridCacheService(distributed, memory, NullLogger<HybridCacheService>.Instance);
        var a = new TenantCacheService(shared, Context("gym_alpha"));
        var b = new TenantCacheService(shared, Context("gym_beta"));
        await a.SetAsync("reset:same@example.test", "123456");
        Assert.That(await b.GetAsync<string>("reset:same@example.test"), Is.Null);
        await b.SetAsync("reset:same@example.test", "654321");
        await a.RemoveAsync("reset:same@example.test");
        Assert.That(await b.GetAsync<string>("reset:same@example.test"), Is.EqualTo("654321"));
    }

    [Test]
    public void LiveWorkoutEventsDoNotCrossTenantBoundaries()
    {
        var broker = new WorkoutUpdatesBroker();
        var id = Guid.NewGuid();
        var a = broker.Subscribe(id, "gym_alpha");
        var b = broker.Subscribe(id, "gym_beta");
        using (a.Subscription)
        using (b.Subscription)
        {
            broker.Publish(new WorkoutUpdateEvent(id, DateTime.UtcNow, TenantId: "gym_alpha"));
            Assert.That(a.Reader.TryRead(out _), Is.True);
            Assert.That(b.Reader.TryRead(out _), Is.False);
        }
    }
    private static TenantContext Context(string schema)
    {
        var context = new TenantContext();
        context.Set(new TenantDefinition { Id = schema, Schema = schema, Hosts = [$"{schema}.test"] });
        return context;
    }

    [Test]
    public void ModelsAndTokensAreIsolated()
    {
        var alpha = Context("gym_alpha");
        var beta = Context("gym_beta");
        var options = new DbContextOptionsBuilder<UsersDbContext>().UseNpgsql("Host=localhost;Database=unused").Options;
        using var a = new UsersDbContext(options, alpha);
        using var b = new UsersDbContext(options, beta);
        Assert.That(a.Model.FindEntityType(typeof(User))!.GetSchema(), Is.EqualTo("gym_alpha"));
        Assert.That(b.Model.FindEntityType(typeof(User))!.GetSchema(), Is.EqualTo("gym_beta"));
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("tenant_id", "gym_alpha")]));
        Assert.That(alpha.Accepts(principal), Is.True);
        Assert.That(beta.Accepts(principal), Is.False);
        Assert.That(beta.Accepts(new ClaimsPrincipal()), Is.False);
        Assert.Throws<InvalidOperationException>(() => beta.Set(alpha.Current));
    }

    [Test]
    public void CatalogRejectsUnsafeOrAmbiguousRouting()
    {
        Assert.Throws<InvalidOperationException>(() => new TenantCatalog([new() { Id = "a", Schema = "public", Hosts = ["a.test"] }]));
        Assert.Throws<InvalidOperationException>(() => new TenantCatalog([
            new() { Id = "a", Schema = "gym_a", Hosts = ["a.test"] },
            new() { Id = "b", Schema = "gym_b", Hosts = ["A.TEST"] }]));
        var catalog = new TenantCatalog([new() { Id = "a", Schema = "gym_a", Hosts = ["a.test"] }]);
        Assert.That(catalog.FindByHost("A.TEST")?.Id, Is.EqualTo("a"));
        Assert.That(catalog.FindByHost("unknown.test"), Is.Null);
    }

    [Test]
    public void AllMigrationScriptsTargetOnlyTheirGym()
    {
        var tenant = Context("gym_sql_test").Current;
        const string connection = "Host=localhost;Database=unused";
        using var users = new UsersDbContext(TenantServices.Options<UsersDbContext>(connection, tenant, "Users"));
        using var trainers = new TrainersDbContext(TenantServices.Options<TrainersDbContext>(connection, tenant, "Trainers"));
        using var subscriptions = new SubscriptionsDbContext(TenantServices.Options<SubscriptionsDbContext>(connection, tenant, "Subscriptions"));
        using var site = new SiteDbContext(TenantServices.Options<SiteDbContext>(connection, tenant, "Site"));
        foreach (var db in new DbContext[] { users, trainers, subscriptions, site })
        {
            Assert.That(db.Database.HasPendingModelChanges(), Is.False, db.GetType().Name);
            var sql = db.GetService<IMigrator>().GenerateScript();
            Assert.That(sql, Does.Contain("gym_sql_test"));
            foreach (var schema in new[] { "users", "trainers", "subscriptions" })
            {
                Assert.That(sql, Does.Not.Contain($"{schema}."));
                Assert.That(sql, Does.Not.Contain($"\"{schema}\"."));
            }
        }
    }

    [Test, Category("PostgresIntegration")]
    public async Task FreshSchemasMigrateAndKeepIdenticalEmailsSeparate()
    {
        var connectionString = Environment.GetEnvironmentVariable("TENANCY_TEST_DB");
        if (string.IsNullOrEmpty(connectionString)) Assert.Ignore("Set TENANCY_TEST_DB to a local disposable PostgreSQL database.");
        var suffix = Guid.NewGuid().ToString("N");
        var tenants = new[] { Context($"gym_test_a_{suffix}"), Context($"gym_test_b_{suffix}") };
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        try
        {
            foreach (var tenant in tenants)
            {
                await using var users = new UsersDbContext(TenantServices.Options<UsersDbContext>(connectionString!, tenant.Current, "Users"));
                await using var trainers = new TrainersDbContext(TenantServices.Options<TrainersDbContext>(connectionString!, tenant.Current, "Trainers"));
                await using var subscriptions = new SubscriptionsDbContext(TenantServices.Options<SubscriptionsDbContext>(connectionString!, tenant.Current, "Subscriptions"));
                await using var site = new SiteDbContext(TenantServices.Options<SiteDbContext>(connectionString!, tenant.Current, "Site"));
                foreach (var db in new DbContext[] { users, trainers, subscriptions, site })
                {
                    await db.Database.MigrateAsync();
                    await db.Database.MigrateAsync(); // Re-running provisioning must be safe.
                }
                await using var runtime = new UsersDbContext(new DbContextOptionsBuilder<UsersDbContext>().UseNpgsql(connectionString).Options, tenant);
                await using var runtimeTrainers = new TrainersDbContext(new DbContextOptionsBuilder<TrainersDbContext>().UseNpgsql(connectionString).Options, tenant);
                await using var runtimeSubscriptions = new SubscriptionsDbContext(new DbContextOptionsBuilder<SubscriptionsDbContext>().UseNpgsql(connectionString).Options, tenant);
                await using var runtimeSite = new SiteDbContext(new DbContextOptionsBuilder<SiteDbContext>().UseNpgsql(connectionString).Options, tenant);
                foreach (var db in new DbContext[] { runtime, runtimeTrainers, runtimeSubscriptions, runtimeSite })
                    Assert.That(db.Model.GetEntityTypes().Select(e => e.GetSchema()).Distinct(), Is.EqualTo(new[] { tenant.Current.Schema }));
                Assert.That(await runtimeTrainers.TrainerProfiles.CountAsync(), Is.Zero);
                Assert.That(await runtimeSubscriptions.Subscriptions.CountAsync(), Is.Zero);
                runtimeSite.Settings.Add(new SiteSettings { Draft = "{}", Published = "{}", Version = 1 });
                await runtimeSite.SaveChangesAsync();
                runtime.Users.Add(new User { Id = "same-id", Email = "same@example.test", UserName = tenant.Current.Id, NormalizedEmail = "SAME@EXAMPLE.TEST",
                    PhoneNumber = "+79991234567", NormalizedPhoneNumber = "+79991234567" });
                await runtime.SaveChangesAsync();
                await using (var duplicatePhone = new UsersDbContext(new DbContextOptionsBuilder<UsersDbContext>().UseNpgsql(connectionString).Options, tenant))
                {
                    duplicatePhone.Users.Add(new User { Id = "duplicate-phone", UserName = "Duplicate", NormalizedPhoneNumber = "+79991234567" });
                    var duplicate = Assert.ThrowsAsync<DbUpdateException>(() => duplicatePhone.SaveChangesAsync());
                    Assert.That(duplicate!.InnerException, Is.TypeOf<PostgresException>());
                    Assert.That(((PostgresException)duplicate.InnerException!).SqlState, Is.EqualTo("23505"));
                }
                var area = new AccessArea { Name = "Бассейн" };
                var plan = new MembershipPlan { TermsJson = JsonSerializer.Serialize(new MembershipTerms {
                    Name = "Бассейн 9+3", DurationMonths = 9, FreezeMonths = 3, VisitLimit = 5, DailyVisitLimit = 1,
                    AllAreas = false, AreaIds = [area.Id] }) };
                var pass = new ClubMembership { PlanId = plan.Id, UserId = "same-id", TermsJson = plan.TermsJson,
                    StartsAt = DateTime.UtcNow.AddMinutes(-1), ExpiresAt = DateTime.UtcNow.AddMonths(9) };
                runtimeSite.AddRange(area, plan, pass, new AccessCard { UserId = "same-id", Label = "Test bracelet",
                    CodeHash = ClubAccessModel.HashCode("SAME-CARD-IN-BOTH-GYMS") });
                await runtimeSite.SaveChangesAsync();
                var controller = new ClubAccessController(runtimeSite, runtime) {
                    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext {
                        User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("userid", "test-owner")], "test")) } } };
                var entry = new ScanCard(Guid.NewGuid(), "SAME-CARD-IN-BOTH-GYMS", area.Id, false);
                Assert.That(await controller.Scan(entry, default), Is.TypeOf<OkObjectResult>());
                Assert.That(await controller.Scan(entry, default), Is.TypeOf<OkObjectResult>()); // Network retry.
                Assert.That(await controller.Scan(entry with { OperationId = Guid.NewGuid() }, default), Is.TypeOf<BadRequestObjectResult>());
                Assert.That(await runtimeSite.Presences.CountAsync(x => x.ExitedAt == null), Is.EqualTo(1));
                Assert.That((await runtimeSite.Memberships.SingleAsync()).UsedVisits, Is.EqualTo(1));
                Assert.That(await runtimeSite.MembershipEvents.CountAsync(), Is.EqualTo(1));
                var exit = entry with { OperationId = Guid.NewGuid(), Exit = true };
                Assert.That(await controller.Scan(exit, default), Is.TypeOf<OkObjectResult>());
                Assert.That(await controller.Scan(entry with { OperationId = Guid.NewGuid() }, default), Is.TypeOf<BadRequestObjectResult>()); // Daily limit.
                Assert.That(await runtimeSite.Presences.CountAsync(x => x.ExitedAt == null), Is.Zero);
                // A stale administrative edit cannot overwrite a concurrently updated pass.
                await using var competing = new SiteDbContext(new DbContextOptionsBuilder<SiteDbContext>().UseNpgsql(connectionString).Options, tenant);
                var stale = await competing.Memberships.SingleAsync();
                pass.Version++; pass.UsedFreezeDays++;
                await runtimeSite.SaveChangesAsync();
                stale.Version++; stale.IsCancelled = true;
                Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => competing.SaveChangesAsync());
                var updatedTerms = MembershipRules.Terms(pass.TermsJson);
                updatedTerms.DailyVisitLimit = 2;
                pass.TermsJson = JsonSerializer.Serialize(updatedTerms); pass.Version++;
                await runtimeSite.SaveChangesAsync();
                async Task<IActionResult> ConcurrentEntry()
                {
                    await using var independent = new SiteDbContext(new DbContextOptionsBuilder<SiteDbContext>().UseNpgsql(connectionString).Options, tenant);
                    await using var independentUsers = new UsersDbContext(new DbContextOptionsBuilder<UsersDbContext>().UseNpgsql(connectionString).Options, tenant);
                    var gate = new ClubAccessController(independent, independentUsers) { ControllerContext = controller.ControllerContext };
                    return await gate.Scan(entry with { OperationId = Guid.NewGuid() }, default);
                }
                var parallel = await Task.WhenAll(ConcurrentEntry(), ConcurrentEntry());
                Assert.That(parallel.Count(x => x is OkObjectResult), Is.EqualTo(1), "Two readers must admit only once");
                await runtimeSite.Entry(pass).ReloadAsync();
                Assert.That(pass.UsedVisits, Is.EqualTo(2));
                Assert.That(await runtimeSite.Presences.CountAsync(x => x.ExitedAt == null), Is.EqualTo(1));
                // A delayed retry of the old exit must not close the new visit.
                Assert.That(await controller.Scan(exit, default), Is.TypeOf<OkObjectResult>());
                Assert.That(await runtimeSite.Presences.CountAsync(x => x.ExitedAt == null), Is.EqualTo(1));
            }
            foreach (var tenant in tenants)
            {
                await using var runtime = new UsersDbContext(new DbContextOptionsBuilder<UsersDbContext>().UseNpgsql(connectionString).Options, tenant);
                Assert.That((await runtime.Users.SingleAsync()).UserName, Is.EqualTo(tenant.Current.Id));
            }
        }
        finally
        {
            foreach (var tenant in tenants)
            {
                // Only schemas created by this test, with a random suffix, can be removed.
                var schema = tenant.Current.Schema!;
                if (!schema.EndsWith(suffix, StringComparison.Ordinal) || !TenantCatalog.IsValidSchema(schema)) throw new InvalidOperationException();
                await using var drop = new NpgsqlCommand($"DROP SCHEMA IF EXISTS \"{schema}\" CASCADE", connection);
                await drop.ExecuteNonQueryAsync();
            }
        }
    }
}
