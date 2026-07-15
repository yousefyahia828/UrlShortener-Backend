using UrlShortener.Domain.Primitives;

namespace UrlShortener.Domain.Users.Tokens;

public sealed class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; init; }
    public string TokenHash { get; init; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }
    public DateTime ExpiredOnUtc { get; init; }
    public DateTime? RevokedOnUtc { get; set; }
    public DateTime? ReplacedOnUtc { get; set; }
    public Guid? ReplacedByTokenId { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiredOnUtc;
    public bool IsRevoked => RevokedOnUtc is not null;
    public bool IsReplaced => ReplacedByTokenId is not null;
    public bool IsActive => !IsExpired && !IsRevoked;

    public User User { get; private set; } = null!;

    public void Replace(Guid? replacedByTokenId = null)
    {
        ReplacedOnUtc = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }

    public void Revoke()
    {
        RevokedOnUtc = DateTime.UtcNow;
    }
}