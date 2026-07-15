using Josephan.CQRS;

namespace UrlShortener.API.Features.ShortenUrls.Disable;

public sealed record DisableUrlCommand(Guid UserId, Guid UrlId) : ICommand;
