using Josephan.CQRS;
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
        var url = await dbContext.ShortenUrls
            .Where(u => u.UserId == command.UserId)
            .Where(u => u.Id == command.UrlId)
            .FirstOrDefaultAsync(cancellationToken);

        if (url is null)
        {
            return UrlErrors.NotFound;
        }

        url.UpdateDescription(command.Description);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}