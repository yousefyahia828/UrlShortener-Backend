using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Events;

public sealed record UserPasswordHasBeenResetDomainEvent(
    Guid UserId,
    string Email,
    string FirstName) : IDomainEvent;
