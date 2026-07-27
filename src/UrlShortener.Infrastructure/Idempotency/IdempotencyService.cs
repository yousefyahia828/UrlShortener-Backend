using Josephan.CQRS.FunctionalResults.NewtonsoftJson;
using Josephan.CQRS.FunctionalResults.SystemTextJson;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json;
using UrlShortener.Abstractions.Idempotency;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.Infrastructure.Idempotency;

internal sealed class IdempotencyService : IIdempotencyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        .AddJosephanFunctionalResults();

    private static readonly TimeSpan PollingInterval = TimeSpan.FromMilliseconds(150);
    private static readonly TimeSpan TimeoutInterval = TimeSpan.FromSeconds(10);

    public IdempotencyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> TryClaimAsync(
        Guid commandId,
        string commandName,
        CancellationToken cancellationToken)
    {
        var command = new IdempotentCommand
        {
            Id = commandId,
            Name = commandName,
            Status = IdempotencyStatus.Processing,
            CreatedOnUtc = DateTime.UtcNow,
        };

        try
        {
            await _dbContext.IdempotentCommands.AddAsync(command, cancellationToken);

            // Must not depend on UoW to save changes
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (IsUniquenessViolation(ex))
        {
            // Roll back (request is being processed by another thread)
            // The caller can wait for thr result to complete
            _dbContext.Entry(command).State = EntityState.Detached;
            return false;
        }
    }

    public async Task CompleteAsync(
        Guid commandId,
        object response,
        CancellationToken cancellationToken)
    {
        // I'm sure command is not null
        var command = await _dbContext.IdempotentCommands
            .FirstAsync(c => c.Id == commandId, cancellationToken);

        command.Status = IdempotencyStatus.Completed;
        command.CompletedOnUtc = DateTime.UtcNow;
        command.Response = JsonSerializer.Serialize(response, _options);
    }

    public async Task<IdempotencyResult<TResponse>> WaitForResponseAsync<TResponse>(
        Guid commandId,
        CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow.Add(TimeoutInterval);

        while (DateTime.UtcNow < deadline)
        {
            var command = await _dbContext.IdempotentCommands
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == commandId, cancellationToken);

            if (command is null)
            {
                // The original claimant failed and released it.
                throw new InvalidOperationException(
                    $"Command '{commandId}' failed during processing.");
            }

            // Command completed
            if (command is { Status: IdempotencyStatus.Completed, Response: not null })
            {
                return new IdempotencyResult<TResponse>
                {
                    Exists = true,
                    Response = JsonSerializer.Deserialize<TResponse>(command.Response, _options)
                };
            }

            // Command still execute
            await Task.Delay(PollingInterval, cancellationToken);
        }

        throw new TimeoutException(
            $"Timed out waiting for command '{commandId}' to complete.");
    }

    public async Task ReleaseClaimAsync(Guid commandId, CancellationToken cancellationToken)
    {
        var command = await _dbContext.IdempotentCommands
            .FirstOrDefaultAsync(c => c.Id == commandId, cancellationToken);

        if (command is not null)
        {
            _dbContext.IdempotentCommands.Remove(command);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }


    public async Task<IdempotencyResult<TResponse>> GetResponseAsync<TResponse>(Guid commandId)
    {
        var command = await _dbContext.IdempotentCommands
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commandId);

        var exists = command is { Status: IdempotencyStatus.Completed, Response: not null };

        return new IdempotencyResult<TResponse>
        {
            Exists = exists,
            Response = exists ? JsonSerializer.Deserialize<TResponse>(command!.Response!, _options) : default
        };
    }

    private static bool IsUniquenessViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException ex &&
               ex.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}