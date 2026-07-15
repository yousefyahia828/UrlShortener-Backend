using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.Infrastructure.BackgorundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessStaleOutboxMessagesJob(
    ApplicationDbContext dbContext,
    ILogger<ProcessStaleOutboxMessagesJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        int count = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc != null &&
                        m.ProcessedOnUtc <= DateTime.UtcNow.AddDays(-3))
            .ExecuteDeleteAsync();

        logger.LogInformation(
            "Processed outbox cleanup. Removed {Count} messages",
            count);
    }
}
