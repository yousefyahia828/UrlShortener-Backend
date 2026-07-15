namespace UrlShortener.Domain.Users.Tokens;

public sealed class EmailVerificationToken
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime ExpiresOnUtc { get; init; }
    public bool IsExpired => DateTime.UtcNow > ExpiresOnUtc;
    public User User { get; private set; } = null!;
}
