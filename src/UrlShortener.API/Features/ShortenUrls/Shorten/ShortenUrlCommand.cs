using UrlShortener.Abstractions.Idempotency;

namespace UrlShortener.API.Features.ShortenUrls.Shorten;

public sealed record ShortenUrlCommand(
    Guid CommandId,
    Guid UserId,
    string LongUrl,
    string? Description) : IdempotentCommand<string>(CommandId);