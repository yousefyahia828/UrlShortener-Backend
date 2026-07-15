using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.Infrastructure.BackgorundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessUnconfirmedPendingEmailsJob(
    IApplicationDbContext dbContext,
    ILogger<ProcessUnconfirmedPendingEmailsJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        int count = await dbContext.Users
            .Where(u => u.PendingEmail != null &&
                        u.EmailPendedOnUtc != null &&
                        u.EmailPendedOnUtc <= DateTime.UtcNow.AddDays(-1))
            .ExecuteUpdateAsync(builer =>
            {
                builer
                    .SetProperty(
                        x => x.PendingEmail,
                        _ => null)
                    .SetProperty(
                        x => x.EmailPendedOnUtc,
                        _ => null);
            });


        logger.LogInformation(
            "Process unconfirmed pending emails cleanup - Removed {Count} pending email(s)",
            count);
    }
}
