namespace UrlShortener.Abstractions.Idempotency;

public sealed record IdempotencyResult<TResponse>
{
    public bool Exists { get; init; }
    public TResponse? Response { get; init; }
}
