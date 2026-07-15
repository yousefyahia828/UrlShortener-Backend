using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Events;

public sealed record UserPasswordChangedDomainEvent(
    Guid UserId,
    string Email,
    string FirstName) : IDomainEvent;
