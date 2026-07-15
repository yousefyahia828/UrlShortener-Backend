using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using UrlShortener.Domain.Primitives;

namespace UrlShortener.Infrastructure.Outbox;

internal sealed class ConvertDomainEventsToOutboxMessagesInterceptor
    : SaveChangesInterceptor
{
    private static readonly JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            PersistOutboxMessages(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void PersistOutboxMessages(DbContext context)
    {
        var outboxMessages = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents;

                entity.ClearDomainEvents();

                return events;
            })
            .Select(@event => new OutboxMessage
            {
                Id = Guid.CreateVersion7(),
                Type = @event.GetType().Name,
                Payload = JsonConvert.SerializeObject(@event, Formatting.None, _settings),
                CreatedOnUtc = DateTime.UtcNow,
            })
            .ToArray();

        context.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
