using Josephan.CQRS;

namespace UrlShortener.API.Features.ShortenUrls.Redirect;

public sealed record RedirectToLongUrlQuery(string Code) : IQuery<string>;
