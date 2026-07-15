using Josephan.CQRS;
using UrlShortener.Abstractions.Authentication.DTOs;

namespace UrlShortener.Abstractions.Authentication;

public interface IRefreshTokenProvider
{
    Task<(string Token, DateTime ExpiresOnUtc)> GenerateAsync(Guid userId);
    Task<RotationResponse?> RotateAsync(string token);
    Task<Result> RevokeAsync(string token);
    string HashToken(string rawToken);
}
