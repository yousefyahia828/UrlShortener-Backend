using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Tokens;

public sealed class PasswordResetToken : Entity<Guid>
{
    public Guid UserId { get; init; }

    public string TokenHash { get; init; }

    public DateTime CreatedOnUtc { get; init; }

    public DateTime ExpiresOnUtc { get; init; }

    public DateTime? UsedOnUtc { get; private set; }

    public bool IsUsed => UsedOnUtc is not null;

    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;

    public User User { get; private set; }

    public void MarkAsUsed()
    {
        UsedOnUtc = DateTime.UtcNow;
    }
}
