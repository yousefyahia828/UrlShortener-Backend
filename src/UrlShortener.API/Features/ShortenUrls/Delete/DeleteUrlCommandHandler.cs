using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Domain.ShortenUrls;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.API.Features.ShortenUrls.Delete;

internal sealed class DeleteUrlCommandHandler(
    ApplicationDbContext context,
    IMemoryCache cache) : ICommandHandler<DeleteUrlCommand>
{
    public async Task<Result> Handle(
        DeleteUrlCommand command,
        CancellationToken cancellationToken)
    {
        return await Result
            .From(context.ShortenUrls
                .Where(u => u.UserId == command.UserId)
                .Where(u => u.Id == command.UrlId)
                .FirstOrDefaultAsync(cancellationToken))
            .EnsureNotNullAsync(UrlErrors.NotFound)
            .TapAsync(url => context.ShortenUrls.Remove(url))
            .TapAsync(url => cache.Remove($"shorten:{url.Code}"));
    }
}