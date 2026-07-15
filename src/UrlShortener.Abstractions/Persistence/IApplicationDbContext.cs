using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.ShortenUrls;
using UrlShortener.Domain.Users;
using UrlShortener.Domain.Users.Tokens;

namespace UrlShortener.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<ShortenUrl> ShortenUrls { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
