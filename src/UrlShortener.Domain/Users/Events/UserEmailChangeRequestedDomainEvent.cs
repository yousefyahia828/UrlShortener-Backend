using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Events;

public sealed record UserEmailChangeRequestedDomainEvent(Guid UserId, string PendingEmail, string FirstName) : IDomainEvent;