using Josephan.CQRS;

namespace UrlShortener.API.Features.ShortenUrls.Delete;

public sealed record DeleteUrlCommand(Guid UserId, Guid UrlId) : ICommand;
