using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Abstractions.Persistence;
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
        string? longUrl = await cache.GetOrCreateAsync(
            $"shorten:{query.Code}",
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                return context.ShortenUrls
                    .AsNoTracking()
                    .Where(url => url.Code == query.Code)
                    .Where(url => url.Enabled)
                    .Select(url => url.LongUrl)
                    .FirstOrDefaultAsync(cancellationToken);
            });

        return longUrl is null
            ? UrlErrors.NotFound
            : longUrl;
    }
}