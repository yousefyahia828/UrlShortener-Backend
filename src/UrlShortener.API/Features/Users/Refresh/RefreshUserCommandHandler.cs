using Josephan.CQRS;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Authentication.DTOs;

namespace UrlShortener.API.Features.Users.Refresh;

internal sealed class RefreshUserCommandHandler(
    IRefreshTokenProvider refreshTokenProvider,
    IJwtProvider jwtProvider)
    : ICommandHandler<RefreshUserCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(
        RefreshUserCommand command,
        CancellationToken cancellationToken)
    {
        var rotationResponse = await refreshTokenProvider.RotateAsync(command.Token);
        if (rotationResponse is null || rotationResponse.User is null)
        {
            return AuthErrors.InvalidToken;
        }

        var user = rotationResponse.User;
        var (accessToken, accessTokenExpiration) = jwtProvider.GenerateToken(user);

        return new AuthResponse
        {
            RefreshToken = rotationResponse.Payload.RefreshToken,
            ExpiresOnUtc = rotationResponse.Payload.RefreshTokenExpiration,
            Payload = new AuthPayload
            {
                AccessToken = accessToken,
                ExpiresOnUtc = accessTokenExpiration
            }
        };
    }
}