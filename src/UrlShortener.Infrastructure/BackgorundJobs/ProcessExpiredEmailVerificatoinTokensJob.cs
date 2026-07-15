using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.Infrastructure.BackgorundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessExpiredEmailVerificatoinTokensJob(
    IApplicationDbContext dbContext,
    ILogger<ProcessExpiredEmailVerificatoinTokensJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        int count = await dbContext.EmailVerificationTokens
          .Where(t => t.ExpiresOnUtc < DateTime.UtcNow)
          .ExecuteDeleteAsync();

        logger.LogInformation(
            "Processed email verification token cleanup. Removed {Count} expired tokens.",
            count);
    }
}
