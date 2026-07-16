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
        return await Result
            .From(context.EmailVerificationTokens
                .Include(t => t.User)
                .Where(t => t.UserId == command.UserId)
                .Where(t => t.Id == command.TokenId)
                .FirstOrDefaultAsync(cancellationToken))
            .EnsureNotNullAsync(AuthErrors.InvalidToken) // To eliminate null forgiving operators !
            .EnsureAsync(
                t => !t.IsExpired && t.User is not null,
                AuthErrors.InvalidToken)
            .EnsureAsync(
                token => !token.User.EmailConfirmed,
                UserErrors.EmailAlreadyConfirmed)
            .TapAsync(token => token.User.ConfirmEmail())
            .TapAsync(token => context.EmailVerificationTokens.Remove(token))
            .TapAsync(_ => context.SaveChangesAsync(cancellationToken));
    }
}