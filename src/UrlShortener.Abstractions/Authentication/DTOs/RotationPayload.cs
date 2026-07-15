namespace UrlShortener.Abstractions.Authentication.DTOs;

public class RotationPayload
{
    public required string RefreshToken { get; init; }
    public required DateTime RefreshTokenExpiration { get; init; }
}