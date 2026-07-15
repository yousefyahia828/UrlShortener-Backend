using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Authentication.DTOs;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.API.Features.Users.Login;

internal sealed class LoginUserCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IRefreshTokenProvider refreshTokenProvider)
    : ICommandHandler<LoginUserCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null ||
            !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            return AuthErrors.InvalidCredentials;
        }

        if (!user.EmailConfirmed)
        {
            return AuthErrors.EmailNotConfirmed(user.CreatedOnUtc);
        }

        var (accessToken, accessTokenExpiration) = jwtProvider.GenerateToken(user);
        var (refreshToken, refreshTokenExpiration) = await refreshTokenProvider.GenerateAsync(user.Id);

        return new AuthResponse
        {
            RefreshToken = refreshToken,
            ExpiresOnUtc = refreshTokenExpiration,
            Payload = new AuthPayload
            {
                AccessToken = accessToken,
                ExpiresOnUtc = accessTokenExpiration
            }
        };
    }
}