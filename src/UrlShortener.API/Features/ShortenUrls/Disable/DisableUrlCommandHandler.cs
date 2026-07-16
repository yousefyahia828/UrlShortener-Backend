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
        return await Result
            .From(context.ShortenUrls
                .Where(t => t.UserId == command.UserId)
                .Where(t => t.Id == command.UrlId)
                .FirstOrDefaultAsync(cancellationToken))
            .EnsureNotNullAsync(UrlErrors.NotFound)
            .EnsureAsync(url => url.Enabled, UrlErrors.AlreadyDisabled)
            .TapAsync(url => url.Disable())
            .TapAsync(url => cache.Remove($"shorten:{url.Code}"))
            .TapAsync(_ => context.SaveChangesAsync(cancellationToken));
    }
}