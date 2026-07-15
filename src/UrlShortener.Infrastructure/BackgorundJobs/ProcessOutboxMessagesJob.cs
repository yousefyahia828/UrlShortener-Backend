using Dapper;
using Josephan.CQRS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System.Data;
using UrlShortener.Domain.Primitives;

namespace UrlShortener.Infrastructure.BackgorundJobs;

internal sealed record OutboxResponse(Guid Id, string Type, string Payload, int RetryCount);

internal sealed class ProcessOutboxMessagesJob(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<ProcessOutboxMessagesJob> logger,
    INotificationPublisher publisher)
    : IJob
{
    private static readonly JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public const int MaxRetryCount = 3;
    private const int BatchSize = 20;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);


    public Task Execute(IJobExecutionContext context)
    {
        return ProcessAsync();
    }

    public async Task ProcessAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();

        var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction();

        try
        {
            var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

            if (outboxMessages.Count == 0)
            {
                logger.LogInformation("Outbox processing completed - no messages to process");
                transaction.Commit();
                return;
            }

            foreach (var message in outboxMessages)
            {
                await ProcessMessageAsync(connection, transaction, message);
            }

            logger.LogInformation("Outbox processing completed - {Count} messages", outboxMessages.Count);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Outbox batch failed, rolling back");
            transaction.Rollback();
            throw;
        }
    }

    private async Task ProcessMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxResponse message)
    {
        try
        {
            IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                message.Payload,
                _settings);

            if (domainEvent is null)
            {
                // Format error → mark dead immediately, no point retrying
                await MarkFailedAsync(
                    connection,
                    transaction,
                    message,
                    "Failed to deserialize domain event",
                    isDead: true);

                return;
            }

            await publisher.PublishAsync(
                domainEvent,
                publishingStrategy: NotificationPublishingStrategy.Parallel);

            await MarkProcessedAsync(
                connection,
                transaction,
                message.Id);
        }
        catch (JsonException ex) // Format error → mark dead immediately, no point retrying
        {
            logger.LogError(ex, "Deserialization failed for outbox message {MessageId}", message.Id);

            await MarkFailedAsync(connection, transaction, message, ex.Message, isDead: true);
        }
        catch (BroadcastFailedException ex)
        {
            foreach (var typed in ex.TypedExceptions)
            {
                logger.LogError(typed.Exception, "Exception while processing outbox message {MessageId} from Handler '{HandlerName}'", message.Id, typed.Type.Name);
            }

            var error = ConstructErrorMessage(ex);

            await MarkFailedAsync(connection, transaction, message, error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while processing outbox message {MessageId}", message.Id);

            await MarkFailedAsync(connection, transaction, message, ex.Message);
        }
    }

    private static string ConstructErrorMessage(BroadcastFailedException ex)
    {
        var messages = ex.TypedExceptions.Select(t => $"[{t.Type.Name}]: {t.Exception.Message}");
        return string.Join(", ", messages);
    }

    private static async Task MarkProcessedAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid messageId)
    {
        const string sql = """
            UPDATE outbox.outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error = NULL
            WHERE id = @MessageId;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                ProcessedOnUtc = DateTime.UtcNow,
                MessageId = messageId
            },
            transaction: transaction);
    }

    private static async Task MarkFailedAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxResponse message,
        string error,
        bool isDead = false)
    {
        int retryCount = message.RetryCount + 1;

        bool shouldBeMarkedAsProcessed = isDead || retryCount >= MaxRetryCount;

        DateTime? processedOnUtc = shouldBeMarkedAsProcessed
            ? DateTime.UtcNow
            : null;

        const string sql = """
            UPDATE outbox.outbox_messages
            SET retry_count = @RetryCount,
                error = @Error,
                processed_on_utc = @ProcessedOnUtc
            WHERE id = @MessageId;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                RetryCount = retryCount,
                Error = error,
                ProcessedOnUtc = processedOnUtc,
                MessageId = message.Id
            },
            transaction: transaction);
    }

    private static async Task<IReadOnlyList<OutboxResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        const string sql = """
            SELECT
                id AS "Id",
                type AS "Type",
                payload AS "Payload",
                retry_count AS "RetryCount"
            FROM outbox.outbox_messages
            WHERE
                processed_on_utc IS NULL AND
                retry_count < @MaxRetryCount
            ORDER BY created_on_utc
            LIMIT @BatchSize
            FOR UPDATE SKIP LOCKED
            """;

        var messages = await connection.QueryAsync<OutboxResponse>(
            sql,
            new { MaxRetryCount, BatchSize },
            transaction: transaction);

        return messages.ToList();
    }
}