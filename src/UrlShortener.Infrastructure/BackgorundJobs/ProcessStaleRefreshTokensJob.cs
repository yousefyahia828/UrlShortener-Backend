using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.Infrastructure.BackgorundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessStaleRefreshTokensJob(
    IApplicationDbContext dbContext,
    ILogger<ProcessStaleRefreshTokensJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime threshold = utcNow.AddDays(-3);

        int count = await dbContext.RefreshTokens
            .Where(t => t.ExpiredOnUtc <= threshold ||
                        (t.ReplacedOnUtc != null && t.ReplacedOnUtc <= threshold) ||
                        (t.RevokedOnUtc != null && t.RevokedOnUtc <= threshold))
            .ExecuteDeleteAsync();

        logger.LogInformation(
            "Processed refresh token cleanup. Removed {Count} tokens.",
            count);
    }
}
