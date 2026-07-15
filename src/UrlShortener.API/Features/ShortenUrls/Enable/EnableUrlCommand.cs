using Josephan.CQRS;

namespace UrlShortener.API.Features.ShortenUrls.Enable;

public sealed record EnableUrlCommand(Guid UserId, Guid UrlId) : ICommand;