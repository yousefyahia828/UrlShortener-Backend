using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.Infrastructure.BackgorundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessUnconfirmedUsersJob(
    IApplicationDbContext dbContext,
    ILogger<ProcessUnconfirmedUsersJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        int count = await dbContext.Users
            .Where(u => !u.EmailConfirmed)
            .Where(u => u.CreatedOnUtc <= DateTime.UtcNow.AddDays(-3))
            .ExecuteDeleteAsync();

        logger.LogInformation(
            "Unconfirmed users cleanup. Removed {Count} users",
            count);
    }
}
