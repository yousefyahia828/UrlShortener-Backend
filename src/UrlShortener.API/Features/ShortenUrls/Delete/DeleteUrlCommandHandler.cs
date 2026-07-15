using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.ShortenUrls;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.ShortenUrls.Delete;

internal sealed class DeleteUrlCommandHandler(
    IApplicationDbContext context,
    IMemoryCache cache) : ICommandHandler<DeleteUrlCommand>
{
    public async Task<Result> Handle(
        DeleteUrlCommand command,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            return UserErrors.NotFound;
        }

        return await Result
            .From(context.ShortenUrls
                .FirstOrDefaultAsync(u => u.Id == command.UrlId, cancellationToken))
            .EnsureNotNullAsync(UrlErrors.NotFound)
            .TapAsync(url => context.ShortenUrls.Remove(url))
            .TapAsync(url => cache.Remove($"shorten:{url.Code}"))
            .TapAsync(_ => context.SaveChangesAsync(cancellationToken));
    }
}