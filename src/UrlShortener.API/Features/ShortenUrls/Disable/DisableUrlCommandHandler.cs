using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.ShortenUrls;

namespace UrlShortener.API.Features.ShortenUrls.Disable;

internal sealed class DisableUrlCommandHandler(
    IApplicationDbContext context,
    IMemoryCache cache)
    : ICommandHandler<DisableUrlCommand>
{
    public async Task<Result> Handle(
        DisableUrlCommand command,
        CancellationToken cancellationToken)
    {
        var url = await context.ShortenUrls
            .Where(t => t.UserId == command.UserId)
            .Where(t => t.Id == command.UrlId)
            .FirstOrDefaultAsync(cancellationToken);

        return await Result
            .From(url)
            .EnsureNotNull(UrlErrors.NotFound)
            .Ensure(url => url.Enabled, UrlErrors.AlreadyDisabled)
            .Tap(url => url.Disable())
            .Tap(url => cache.Remove($"shorten:{url.Code}"))
            .TapAsync(() => context.SaveChangesAsync(cancellationToken));
    }
}