using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Authentication.DTOs;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.Infrastructure.Authentication.RefreshToken;

internal sealed class RefreshTokenProvider(
    ApplicationDbContext context,
    IOptions<RefreshTokenOptions> options) : IRefreshTokenProvider
{
    private readonly RefreshTokenOptions _options = options.Value;

    public async Task<(string Token, DateTime ExpiresOnUtc)> GenerateAsync(Guid userId)
    {
        var rawToken = GetRawToken();
        var hash = HashToken(rawToken);

        var token = new Domain.Users.Tokens.RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            TokenHash = hash,
            CreatedOnUtc = DateTime.UtcNow,
            ExpiredOnUtc = DateTime.UtcNow.AddDays(_options.LifetimeInDays)
        };

        await context.RefreshTokens.AddAsync(token);
        await context.SaveChangesAsync();

        return (rawToken, token.ExpiredOnUtc);
    }

    public async Task<RotationResponse?> RotateAsync(string token)
    {
        var refreshToken = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == HashToken(token));

        if (refreshToken is null || !refreshToken.IsActive) return null;

        var newRawToken = GetRawToken();
        var newTokenHash = HashToken(newRawToken);

        var newRefreshToken = new Domain.Users.Tokens.RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = refreshToken.UserId,
            TokenHash = newTokenHash,
            CreatedOnUtc = DateTime.UtcNow,
            ExpiredOnUtc = DateTime.UtcNow.AddDays(_options.LifetimeInDays)
        };

        refreshToken.ReplacedByTokenId = newRefreshToken.Id;

        await context.RefreshTokens.AddAsync(newRefreshToken);
        await context.SaveChangesAsync();

        return new RotationResponse
        {
            User = refreshToken.User,
            Payload = new RotationPayload
            {
                RefreshToken = newRawToken,
                RefreshTokenExpiration = newRefreshToken.ExpiredOnUtc
            }
        };
    }

    public async Task<Result> RevokeAsync(string token)
    {
        var refreshToken = await context.RefreshTokens
           .FirstOrDefaultAsync(t => t.TokenHash == HashToken(token));

        if (refreshToken is null || !refreshToken.IsActive)
        {
            return AuthErrors.InvalidToken;
        }

        refreshToken.Revoke();

        await context.SaveChangesAsync();

        return Unit.Value;
    }

    public string HashToken(string rawToken)
        => Base64Url.EncodeToString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

    private string GetRawToken()
        => Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(_options.Length));
}