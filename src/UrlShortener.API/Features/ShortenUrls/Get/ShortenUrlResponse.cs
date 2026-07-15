namespace UrlShortener.API.Features.ShortenUrls.Get;

public sealed record ShortenUrlResponse
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public string ShortUrl { get; init; } = string.Empty;
    public required string LongUrl { get; init; }
    public required string Description { get; init; }
    public required DateTime CreatedOnUtc { get; init; }
}
