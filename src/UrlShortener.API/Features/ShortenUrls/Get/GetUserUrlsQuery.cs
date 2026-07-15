using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Pagination;

namespace UrlShortener.API.Features.ShortenUrls.Get;

public sealed record GetUserUrlsQuery(
    Guid UserId,
    string BaseUrl,
    int? Page,
    int? PageSize,
    string? Order,
    bool Enabled = true)
    : IQuery<PageResult<ShortenUrlResponse>>;
