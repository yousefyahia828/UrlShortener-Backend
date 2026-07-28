using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Users;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.API.Features.Users.Identity.Email.CancelEmailChange;

internal sealed class CancelEmailChangeCommandHandler(
    ApplicationDbContext context)
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
            .BindAsync(u => u.CancelChangeEmailRequest());
    }
}