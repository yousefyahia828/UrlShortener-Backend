using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Events;

public sealed record UserProfileUpdatedDomainEvent(Guid UserId) : IDomainEvent;

public sealed record ProfileImageUpdatedDomainEvent(
    Guid UserId,
    string Email,
    string FirstName) : IDomainEvent;