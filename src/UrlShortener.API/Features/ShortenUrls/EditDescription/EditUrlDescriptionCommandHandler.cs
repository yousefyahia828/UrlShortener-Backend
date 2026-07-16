using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.ShortenUrls;

namespace UrlShortener.API.Features.ShortenUrls.EditDescription;

internal sealed class EditUrlDescriptionCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<EditUrlDescriptionCommand>
{
    public async Task<Result> Handle(
        EditUrlDescriptionCommand command,
        CancellationToken cancellationToken)
    {
        return await Result
            .From(dbContext.ShortenUrls
                .Where(u => u.UserId == command.UserId)
                .Where(u => u.Id == command.UrlId)
                .FirstOrDefaultAsync(cancellationToken))
            .EnsureNotNullAsync(UrlErrors.NotFound)
            .TapAsync(url => url.UpdateDescription(command.Description))
            .TapAsync(_ => dbContext.SaveChangesAsync(cancellationToken));
    }
}