using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Abstractions.Persistence;
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
        var url = await context.ShortenUrls
             .Where(t => t.UserId == command.UserId)
             .Where(t => t.Id == command.UrlId)
             .FirstOrDefaultAsync(cancellationToken);

        return await Result
            .From(url)
            .EnsureNotNull(UrlErrors.NotFound)
            .Ensure(url => !url.Enabled, UrlErrors.AlreadyEnabled)
            .Tap(url => url.Enable())
            .Tap(url => cache.Set(
                $"shorten:{url.Code}",
                url.LongUrl,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                }))
            .TapAsync(() => context.SaveChangesAsync(cancellationToken));
    }
}