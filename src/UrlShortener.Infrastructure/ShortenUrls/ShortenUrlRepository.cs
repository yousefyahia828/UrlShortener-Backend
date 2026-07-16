using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.ShortenUrls;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.Infrastructure.ShortenUrls;

internal sealed class ShortenUrlRepository(
    ApplicationDbContext context) :
    IShortenUrlRepository
{
    public async Task<bool> TryInsertAsync(ShortenUrl shortenUrl, CancellationToken cancellationToken = default)
    {
        int affectedRows = await context.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO shorten.shorten_urls (id, user_id, code, long_url, created_on_utc, description)
            VALUES ({0}, {1}, {2}, {3}, {4}, {5})
            ON CONFLICT (code) DO NOTHING
            """,
            [
                shortenUrl.Id,
                shortenUrl.UserId,
                shortenUrl.Code,
                shortenUrl.LongUrl,
                shortenUrl.CreatedOnUtc,
                shortenUrl.Description!
            ],
            cancellationToken);

        return affectedRows > 0; // Actually 1 or zero on failure
    }
}
