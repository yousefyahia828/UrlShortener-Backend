using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Events;

public sealed record UserEmailChangedDomainEvent(
    Guid UserId,
    string OldEmail,
    string NewEmail,
    string FirstName) : IDomainEvent;

public sealed record UserEmailChangeCancelledDomainEvent(
    Guid UserId) : IDomainEvent;


