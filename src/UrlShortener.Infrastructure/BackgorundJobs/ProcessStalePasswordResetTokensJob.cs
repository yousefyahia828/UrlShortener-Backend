using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.Infrastructure.BackgorundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessStalePasswordResetTokensJob(
    IApplicationDbContext dbContext,
    ILogger<ProcessStalePasswordResetTokensJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        int count = await dbContext.PasswordResetTokens
            .Where(t => t.ExpiresOnUtc < DateTime.UtcNow ||
                        t.UsedOnUtc != null)
            .ExecuteDeleteAsync();

        logger.LogInformation(
            "Processed password reset tokens cleanup. Removed {Count} tokens",
            count);
    }
}
