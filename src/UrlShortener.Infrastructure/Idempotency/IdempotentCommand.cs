using UrlShortener.Abstractions.Idempotency;

namespace UrlShortener.Infrastructure.Idempotency;

public sealed class IdempotentCommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IdempotencyStatus Status { get; set; }
    public string? Response { get; set; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? CompletedOnUtc { get; set; }
}