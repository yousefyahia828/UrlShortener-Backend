using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.API.Common.Abstractions.Pagination;
using UrlShortener.Domain.ShortenUrls;

namespace UrlShortener.API.Features.ShortenUrls.Get;

internal sealed class GetUserUrlsQueryHandler(
    IApplicationDbContext context)
    : IQueryHandler<GetUserUrlsQuery, PageResult<ShortenUrlResponse>>
{
    public async Task<Result<PageResult<ShortenUrlResponse>>> Handle(
        GetUserUrlsQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<ShortenUrl> shortenUrlsQuery = context.ShortenUrls.AsNoTracking();

        shortenUrlsQuery = shortenUrlsQuery
            .Where(url => url.UserId == query.UserId)
            .Where(url => url.Enabled == query.Enabled);

        shortenUrlsQuery = string.Equals("desc", query.Order, StringComparison.InvariantCultureIgnoreCase)
            ? shortenUrlsQuery.OrderByDescending(url => url.CreatedOnUtc)
            : shortenUrlsQuery.OrderBy(url => url.CreatedOnUtc);

        var shortenUrlsResponseQuery = shortenUrlsQuery
            .Select(url => new ShortenUrlResponse
            {
                Id = url.Id,
                Code = url.Code,
                ShortUrl = string.Concat(query.BaseUrl, "/", url.Code),
                LongUrl = url.LongUrl,
                Description = url.Description ?? "",
                CreatedOnUtc = url.CreatedOnUtc
            });

        return await PageResult<ShortenUrlResponse>.CreateAsync(
            shortenUrlsResponseQuery,
            query.Page,
            query.PageSize);
    }
}