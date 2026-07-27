using Josephan.CQRS;

namespace UrlShortener.Abstractions.Idempotency;

public interface IBaseIdempotent
{
    public Guid CommandId { get; }
}

public record IdempotentCommand(Guid CommandId) : ICommand, IBaseIdempotent;
public record IdempotentCommand<TResponse>(Guid CommandId) : ICommand<TResponse>, IBaseIdempotent;

