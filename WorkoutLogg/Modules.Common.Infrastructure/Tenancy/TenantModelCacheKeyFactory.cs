using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Modules.Common.Infrastructure.Tenancy;

public interface ITenantDbContext
{
    string Schema { get; }
}

public sealed class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime) =>
        (context.GetType(), (context as ITenantDbContext)?.Schema, designTime);
}
