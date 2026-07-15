namespace UrlShortener.Domain.ShortenUrls;

public interface IShortenUrlRepository
{
    Task<bool> TryInsertAsync(ShortenUrl shortenUrl, CancellationToken cancellationToken = default);
}
