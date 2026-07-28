using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailConfirmation;

internal sealed class RequestEmailConfirmationCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<RequestEmailConfirmationCommand>
{
    public async Task<Result> Handle(
        RequestEmailConfirmationCommand command,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is not null && !user.EmailConfirmed)
        {
            user.RequestEmailVerification();
        }

        return Unit.Value;
    }
}