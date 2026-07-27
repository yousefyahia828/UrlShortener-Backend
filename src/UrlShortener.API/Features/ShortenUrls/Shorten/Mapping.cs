namespace UrlShortener.API.Features.ShortenUrls.Shorten;

public static class Mapping
{
    public static ShortenUrlCommand MapToCommand(
        this Endpoint.ShortenUrlRequest request,
        Guid userId,
        Guid idempotencyKey)
    => new(idempotencyKey, userId, request.LongUrl, request.Description);
}
