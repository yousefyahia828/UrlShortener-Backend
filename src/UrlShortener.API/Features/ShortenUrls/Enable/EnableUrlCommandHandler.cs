using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.API.Features.ShortenUrls.Common;
using UrlShortener.Domain.ShortenUrls;

namespace UrlShortener.API.Features.ShortenUrls.Enable;

internal sealed class EnableUrlCommandHandler(
    IApplicationDbContext context,
    IMemoryCache cache)
    : ICommandHandler<EnableUrlCommand>
{
    public async Task<Result> Handle(
        EnableUrlCommand command,
        CancellationToken cancellationToken)
    {
        return await Result
            .From(context.ShortenUrls
                .Where(t => t.UserId == command.UserId)
                .Where(t => t.Id == command.UrlId)
                .FirstOrDefaultAsync(cancellationToken))
            .EnsureNotNullAsync(UrlErrors.NotFound)
            .EnsureAsync(url => !url.Enabled, UrlErrors.AlreadyEnabled)
            .TapAsync(url => url.Enable())
            .TapAsync(url => cache.Set(
                $"shorten:{url.Code}",
                new CachableUrl(url.LongUrl, url.Enabled),
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                }))
            .TapAsync(_ => context.SaveChangesAsync(cancellationToken));
    }
}