namespace UrlShortener.Abstractions.Authentication.DTOs;

public sealed record AuthPayload
{
    public required string AccessToken { get; init; }
    public required DateTime ExpiresOnUtc { get; init; }
}
