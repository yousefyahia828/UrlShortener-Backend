using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.ShortenUrls;

namespace UrlShortener.API.Features.ShortenUrls.Shorten;

internal sealed class ShortenUrlCommandHandler(
    IShortenUrlRepository repository,
    IApplicationDbContext context,
    IMemoryCache cache)
    : ICommandHandler<ShortenUrlCommand, string>
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    private const int MaxRetries = 10;
    private const int CodeLength = 8;


    public async Task<Result<string>> Handle(
        ShortenUrlCommand command,
        CancellationToken cancellationToken)
    {
        for (int i = 0; i < MaxRetries; i++)
        {
            string code = GenerateRandomCode(CodeLength);

            var shortenUrl = ShortenUrl.Create(
                Guid.CreateVersion7(),
                command.UserId,
                command.LongUrl.Trim(),
                code,
                command.Description?.Trim());

            bool inserted = await repository.TryInsertAsync(shortenUrl, cancellationToken);

            if (inserted)
            {
                cache.Set(
                    $"shorten:{shortenUrl.Code}",
                    shortenUrl.LongUrl,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                    });

                await context.SaveChangesAsync(cancellationToken);
                return shortenUrl.Code;
            }
        }

        return UrlErrors.FailedToShorten;
    }

    private static string GenerateRandomCode(int length)
    {
        Span<byte> buffer = stackalloc byte[length];
        RandomNumberGenerator.Fill(buffer);

        var sb = new StringBuilder(length);
        foreach (byte b in buffer)
            sb.Append(Alphabet[b % Alphabet.Length]);

        return sb.ToString();
    }
}