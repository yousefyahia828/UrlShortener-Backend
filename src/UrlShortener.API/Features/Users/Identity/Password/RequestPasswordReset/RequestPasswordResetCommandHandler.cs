using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.API.Features.Users.Identity.Password.RequestPasswordReset;

internal sealed class RequestPasswordResetCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<RequestPasswordResetCommand>
{
    public async Task<Result> Handle(
        RequestPasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(u => u.Email == command.Email)
            .Where(u => u.EmailConfirmed)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is not null)
        {
            // Delete any active tokens if any so only one is valid
            await context.EmailVerificationTokens
                .Where(t => t.UserId == user.Id)
                .Where(t => t.ExpiresOnUtc <= DateTime.UtcNow)
                .ExecuteDeleteAsync(cancellationToken);

            user.RequestPasswordChange();
            await context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}