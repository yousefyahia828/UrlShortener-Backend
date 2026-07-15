using Microsoft.Extensions.Hosting;
using Quartz;

namespace UrlShortener.Infrastructure.BackgorundJobs;

internal sealed class RecurringJobScheduler : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public RecurringJobScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await EnsureRecurringJobAsync<ProcessOutboxMessagesJob>(
            scheduler,
            "outbox-job",
            "outbox-trigger",
            TimeSpan.FromSeconds(10),
            workersCount: 3,
            cancellationToken: cancellationToken);
        ;

        await EnsureRecurringJobAsync<ProcessStaleOutboxMessagesJob>(
            scheduler,
            "outbox-cleanup",
            "outbox-cleanup-trigger",
            TimeSpan.FromDays(1),
            cancellationToken: cancellationToken);

        await EnsureRecurringJobAsync<ProcessStaleRefreshTokensJob>(
           scheduler,
           "refresh-token-cleanup",
           "refresh-token-cleanup-trigger",
           TimeSpan.FromDays(1),
           cancellationToken: cancellationToken);

        await EnsureRecurringJobAsync<ProcessStalePasswordResetTokensJob>(
           scheduler,
           "password-reset-token-cleanup",
           "password-reset-token-cleanup-trigger",
           TimeSpan.FromDays(3),
           cancellationToken: cancellationToken);

        await EnsureRecurringJobAsync<ProcessExpiredEmailVerificatoinTokensJob>(
           scheduler,
           "email-verification-token-cleanup",
           "email-verification-token-cleanup-trigger",
           TimeSpan.FromDays(3),
           cancellationToken: cancellationToken);

        await EnsureRecurringJobAsync<ProcessUnconfirmedUsersJob>(
            scheduler,
            "unconfirmed-users-cleanup",
            "unconfirmed-users-cleanup-trigger",
            TimeSpan.FromDays(1),
            cancellationToken: cancellationToken);

        await EnsureRecurringJobAsync<ProcessUnconfirmedPendingEmailsJob>(
            scheduler,
            "unconfirmed-pending-emails-cleaup",
            "unconfirmed-pending-emails-cleaup-trigger",
            TimeSpan.FromDays(1),
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task EnsureRecurringJobAsync<TJob>(
        IScheduler scheduler,
        string jobKeyName,
        string triggerKeyName,
        TimeSpan interval,
        int workersCount = 1,
        CancellationToken cancellationToken = default)
        where TJob : IJob
    {
        for (int i = 0; i < workersCount; i++)
        {
            var jobKey = new JobKey($"{jobKeyName}-{i + 1}");
            var triggerKey = new TriggerKey($"{triggerKeyName}-{i + 1}");

            if (await scheduler.CheckExists(jobKey, cancellationToken))
            {
                return; // Never Update or Replace The Trigger
            }

            // Create Trigger for the 1st time

            var job = JobBuilder.Create<TJob>()
                .WithIdentity(jobKey)
                .StoreDurably()
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(job)
                .WithIdentity(triggerKey)
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(i * 3.33))
                .WithSimpleSchedule(schedulerBuilder => schedulerBuilder
                    .WithInterval(interval)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
    }

}
