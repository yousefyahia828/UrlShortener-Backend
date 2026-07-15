using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Authentication.DTOs;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmail;

internal sealed class ConfirmEmailCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<ConfirmEmailCommand>
{
    public async Task<Result> Handle(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        var token = await context.EmailVerificationTokens
            .Include(t => t.User)
            .Where(t => t.UserId == command.UserId)
            .Where(t => t.Id == command.TokenId)
            .FirstOrDefaultAsync(cancellationToken);

        if (token is null || token.IsExpired || token.User is null)
        {
            return AuthErrors.InvalidToken;
        }

        var user = token.User;

        if (user.EmailConfirmed)
        {
            return UserErrors.EmailAlreadyConfirmed;
        }

        user.ConfirmEmail();

        context.EmailVerificationTokens.Remove(token);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}