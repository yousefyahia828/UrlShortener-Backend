using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UrlShortener.Domain.Primitives;

namespace UrlShortener.Infrastructure.Database;

internal sealed class AuditInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is not null)
        {
            var entities = context.ChangeTracker
                .Entries<IAuditableEntity>()
                .Where(e => e.State == EntityState.Added)
                .ToArray();

            var utcNow = DateTime.UtcNow;
            var propertyName = nameof(IAuditableEntity.CreatedOnUtc);

            foreach (var entity in entities)
            {
                entity.Property(propertyName).CurrentValue = utcNow;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
