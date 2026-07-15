namespace UrlShortener.Infrastructure.Authentication.RefreshToken;

public sealed record RefreshTokenOptions
{
    public int LifetimeInDays { get; set; }
    public int Length { get; set; }
}
