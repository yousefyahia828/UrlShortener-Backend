using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Authentication.DTOs;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Password.ConfirmPasswordReset;

internal sealed class ConfirmPasswordResetCommandHandler(
    IPasswordResetService passwordResetService,
    IApplicationDbContext context,
    IPasswordHasher passwordHasher)
    : ICommandHandler<ConfirmPasswordResetCommand>
{
    public async Task<Result> Handle(
        ConfirmPasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        var tokenHash = passwordResetService.HashToken(command.Token);

        var token = await context.PasswordResetTokens
            .Include(t => t.User)
            .Where(t => t.TokenHash == tokenHash)
            .FirstOrDefaultAsync(cancellationToken);

        if (token is null ||
            token.ExpiresOnUtc < DateTime.UtcNow ||
            token.UsedOnUtc is not null ||
            token.User is null ||
            !token.User.EmailConfirmed)
        {
            return AuthErrors.InvalidToken;
        }

        var user = token.User;

        if (passwordHasher.Verify(command.NewPassword, user.PasswordHash))
        {
            return UserErrors.SamePassword;
        }

        var newPasswordHash = passwordHasher.HashPassword(command.NewPassword);

        user.ResetPassword(newPasswordHash);

        token.MarkAsUsed();

        // Revoke All Active Session
        await context.RefreshTokens
            .Where(t => t.UserId == user.Id)
            .Where(t => t.ExpiredOnUtc > DateTime.UtcNow)
            .ExecuteDeleteAsync(cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}