using Josephan.CQRS;

namespace UrlShortener.API.Features.ShortenUrls.Shorten;

public sealed record ShortenUrlCommand(
    Guid UserId,
    string LongUrl,
    string? Description) : ICommand<string>;