using Josephan.CQRS;

namespace UrlShortener.Domain.Primitives;

public interface IDomainEventHandler<TEvent>
    : INotificationHandler<TEvent>
    where TEvent : IDomainEvent;
