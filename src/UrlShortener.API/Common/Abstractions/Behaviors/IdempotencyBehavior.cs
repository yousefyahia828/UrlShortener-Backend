using UrlShortener.Abstractions.Idempotency;

namespace UrlShortener.API.Common.Abstractions.Behaviors;

internal sealed class IdempotencyBehavior<TRequest, TResponse>
    : ICommandPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IBaseIdempotent
    where TResponse : Result
{
    private readonly IIdempotencyService _idempotencyService;

    public IdempotencyBehavior(IIdempotencyService idempotencyService)
    {
        _idempotencyService = idempotencyService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        var commandId = request.CommandId;
        var commandName = typeof(TRequest).Name;

        // First check for an already-completed result
        var cached = await _idempotencyService.GetResponseAsync<TResponse>(commandId);
        if (cached.Exists)
        {
            return cached.Response!;
        }

        // Check if it's new command or being processed
        var claimed = await _idempotencyService.TryClaimAsync(commandId, commandName, cancellationToken);

        if (!claimed)
        {
            // Someone else claimed it first — either it's still running
            // or finished between our check above and now. Wait it out.
            var waited = await _idempotencyService.WaitForResponseAsync<TResponse>(commandId, cancellationToken);
            return waited.Response!;
        }

        try
        {
            var response = await next(request, cancellationToken);
            await _idempotencyService.CompleteAsync(commandId, response, cancellationToken);
            return response;
        }
        catch
        {
            // Release the claim so it isn't stuck in Processing forever,
            // and so any waiters fail fast instead of timing out.
            await _idempotencyService.ReleaseClaimAsync(commandId, cancellationToken);
            throw; // Never catch the exception object to preserve stack trace
        }
    }
}