namespace UrlShortener.Infrastructure.Authentication.Jwt;

public sealed record JwtOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string SecretKey { get; set; }
    public required int LifetimeInMinutes { get; set; }
}
