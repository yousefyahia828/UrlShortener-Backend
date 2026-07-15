using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Authentication.DTOs;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Password.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IApplicationDbContext context,
    IRefreshTokenProvider refreshTokenProvider,
    IPasswordHasher passwordHasher)
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(
        ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        var isCurrentPasswordValid =
            passwordHasher.Verify(command.CurrentPassword, user.PasswordHash);

        if (!isCurrentPasswordValid)
        {
            return AuthErrors.InvalidPassword;
        }

        var isSamePassword =
            passwordHasher.Verify(command.NewPassword, user.PasswordHash);

        if (isSamePassword)
        {
            return UserErrors.SamePassword;
        }

        var newPasswordHash = passwordHasher.HashPassword(command.NewPassword);

        var result = user.ResetPassword(newPasswordHash);

        if (result.IsSuccess)
        {
            if (command.RevokeAllSessions)
            {
                // Revoke All Active Session except current session
                await context.RefreshTokens
                    .Where(t => t.TokenHash != refreshTokenProvider.HashToken(command.RefreshToken!))
                    .Where(t => t.UserId == user.Id)
                    .Where(t => t.ExpiredOnUtc > DateTime.UtcNow)
                    .ExecuteDeleteAsync(cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
