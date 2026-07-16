using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.API.Features.ShortenUrls.Common;
using UrlShortener.Domain.ShortenUrls;

namespace UrlShortener.API.Features.ShortenUrls.Redirect;

internal sealed class RedirectToLongUrlQueryHandler(
    IApplicationDbContext context,
    IMemoryCache cache)
    : IQueryHandler<RedirectToLongUrlQuery, string>
{
    public async Task<Result<string>> Handle(
        RedirectToLongUrlQuery query,
        CancellationToken cancellationToken)
    {
        var urlResponse = await cache.GetOrCreateAsync(
            $"shorten:{query.Code}",
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                return context.ShortenUrls
                    .AsNoTracking()
                    .Where(url => url.Code == query.Code)
                    .Select(url => new CachableUrl(url.LongUrl, url.Enabled))
                    .FirstOrDefaultAsync(cancellationToken);
            });

        return urlResponse is null ? UrlErrors.NotFound :
            !urlResponse.Enabled ? UrlErrors.Disabled :
            urlResponse.LongUrl;
    }
}