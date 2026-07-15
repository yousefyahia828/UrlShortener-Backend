namespace UrlShortener.Abstractions.Authentication.DTOs;

public sealed record AuthResponse
{
    public required string RefreshToken { get; init; }
    public required DateTime ExpiresOnUtc { get; init; }
    public required AuthPayload Payload { get; init; }
}