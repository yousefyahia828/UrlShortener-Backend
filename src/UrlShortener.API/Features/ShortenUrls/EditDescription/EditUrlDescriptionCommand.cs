using Josephan.CQRS;

namespace UrlShortener.API.Features.ShortenUrls.EditDescription;

public sealed record EditUrlDescriptionCommand(
    Guid UserId,
    Guid UrlId,
    string? Description) : ICommand;
