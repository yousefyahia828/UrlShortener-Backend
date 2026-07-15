using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailConfirmation;

internal sealed class RequestEmailConfirmationCommandHandler(
    IApplicationDbContext context)
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
            await context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}