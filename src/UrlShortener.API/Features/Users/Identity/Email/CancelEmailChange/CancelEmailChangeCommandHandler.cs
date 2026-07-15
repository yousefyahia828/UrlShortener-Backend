using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Email.CancelEmailChange;

internal sealed class CancelEmailChangeCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<CancelEmailChangeCommand>
{
    public async Task<Result> Handle(
        CancelEmailChangeCommand command,
        CancellationToken cancellationToken)
    {
        return await Result
            .From(context.Users.FirstOrDefaultAsync(
                u => u.Id == command.UserId,
                cancellationToken))
            .EnsureNotNullAsync(UserErrors.NotFound)
            .BindAsync(u => u.CancelChangeEmailRequest())
            .TapAsync(() => context.SaveChangesAsync(cancellationToken));
    }
}