using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.ShortenUrls.Events;

public sealed record ShortenUrlCreatedDomainEvent(Guid ShortenUrlId, Guid UserId, string Code) : IDomainEvent;

public sealed record ShortenUrlDisabledDomainEvent(Guid ShortenUrlId) : IDomainEvent;

public sealed record ShortenUrlEnabledDomainEvent(Guid ShortenUrlId) : IDomainEvent;