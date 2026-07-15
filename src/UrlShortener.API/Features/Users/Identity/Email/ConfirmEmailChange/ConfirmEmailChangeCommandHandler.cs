using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Authentication.DTOs;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmailChange;

internal sealed class ConfirmEmailChangeCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<ConfirmEmailChangeCommand>
{
    public async Task<Result> Handle(
        ConfirmEmailChangeCommand command,
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

        var result = user.ConfirmEmailChange();

        context.EmailVerificationTokens.Remove(token);

        if (result.IsSuccess)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}