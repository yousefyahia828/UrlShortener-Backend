namespace UrlShortener.Abstractions.Idempotency;

public interface IIdempotencyService
{
    Task<bool> TryClaimAsync(Guid commandId, string commandName, CancellationToken cancellationToken);

    Task CompleteAsync(Guid commandId, object response, CancellationToken cancellationToken);

    Task<IdempotencyResult<TResponse>> WaitForResponseAsync<TResponse>(
        Guid commandId,
        CancellationToken cancellationToken);

    Task<IdempotencyResult<TResponse>> GetResponseAsync<TResponse>(Guid commandId);
    Task ReleaseClaimAsync(Guid commandId, CancellationToken cancellationToken);
}