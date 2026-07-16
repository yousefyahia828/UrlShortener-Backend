using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.ShortenUrls.Events;
using UrlShortener.Domain.Users;

namespace UrlShortener.Domain.ShortenUrls;

public sealed class ShortenUrl : Entity<Guid>, IAuditableEntity
{
    private ShortenUrl()
    {
        // EF Core
    }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string LongUrl { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public bool Enabled { get; private set; } = true;
    public DateTime CreatedOnUtc { get; private set; }
    public string? Description { get; private set; }

    public static ShortenUrl Create(
        Guid id,
        Guid userId,
        string longUrl,
        string code,
        string? description)
    {
        var shortenUrl = new ShortenUrl
        {
            Id = id,
            UserId = userId,
            LongUrl = longUrl,
            Code = code,
            Enabled = true,
            Description = description,
            CreatedOnUtc = DateTime.UtcNow
        };

        shortenUrl.Raise(new ShortenUrlCreatedDomainEvent(shortenUrl.Id, userId, code));

        return shortenUrl;
    }

    public void Disable()
    {
        if (!Enabled) return;

        Enabled = false;

        Raise(new ShortenUrlDisabledDomainEvent(Id));
    }

    public void Enable()
    {
        if (Enabled) return;

        Enabled = true;

        Raise(new ShortenUrlEnabledDomainEvent(Id));
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }
}