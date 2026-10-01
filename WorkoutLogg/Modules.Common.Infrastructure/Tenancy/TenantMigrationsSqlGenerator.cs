using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace Modules.Common.Infrastructure.Tenancy;

// Migration snapshots stay in their canonical module schema. Only SQL generation is
// redirected; runtime contexts use TenantModelCacheKeyFactory. Pin and test provider upgrades.
#pragma warning disable EF1001
public sealed class TenantMigrationsSqlGenerator(
    MigrationsSqlGeneratorDependencies dependencies,
    INpgsqlSingletonOptions options,
    ICurrentDbContext current) : NpgsqlMigrationsSqlGenerator(dependencies, options)
{
    public override IReadOnlyList<MigrationCommand> Generate(
        IReadOnlyList<MigrationOperation> operations, IModel? model = null,
        MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
    {
        var schema = new NpgsqlConnectionStringBuilder(current.Context.Database.GetConnectionString()).SearchPath;
        if (schema is null || !TenantCatalog.IsValidSchema(schema))
            throw new InvalidOperationException("Tenant migration requires a validated gym schema as its search path.");
        // Npgsql wraps its own history-table creation in a SqlOperation. Allow only
        // an exact match to the provider-generated script for this context/schema.
        if (model is null && operations is [SqlOperation history] &&
            history.Sql == current.Context.GetService<IHistoryRepository>().GetCreateIfNotExistsScript())
            return base.Generate(operations, model, options);
        var undo = new Stack<Action>();
        try
        {
            foreach (var operation in operations) Remap(operation, schema, undo);
            return base.Generate(operations, model, options);
        }
        finally
        {
            while (undo.TryPop(out var restore)) restore();
        }
    }

    private static void Remap(MigrationOperation operation, string schema, Stack<Action> undo)
    {
        // Raw SQL/schema deletion require explicit tenant-aware review rather than string replacement.
        if (operation is SqlOperation or DropSchemaOperation)
            throw new NotSupportedException("Raw SQL and schema deletion are not supported in tenant migrations.");
        foreach (var property in operation.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length != 0 || !property.CanRead) continue;
            var value = property.GetValue(operation);
            if (property.CanWrite && property.PropertyType == typeof(string) &&
                (property.Name is "Schema" or "PrincipalSchema" or "NewSchema" ||
                 operation is EnsureSchemaOperation && property.Name == "Name"))
            {
                if (value is string original && original is "users" or "trainers" or "subscriptions")
                {
                    property.SetValue(operation, schema);
                    undo.Push(() => property.SetValue(operation, original));
                }
                else if (value is string other && other != schema)
                    throw new NotSupportedException($"Unexpected schema in tenant migration: {other}");
            }
            else if (value is MigrationOperation child) Remap(child, schema, undo);
            else if (value is IEnumerable items && value is not string)
                foreach (var item in items)
                    if (item is MigrationOperation nested) Remap(nested, schema, undo);
        }
    }
}
#pragma warning restore EF1001
