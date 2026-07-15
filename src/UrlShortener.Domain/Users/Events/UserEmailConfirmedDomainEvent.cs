using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Events;

public sealed record UserEmailConfirmedDomainEvent(Guid UserId, string Email, string FirstName) : IDomainEvent;
