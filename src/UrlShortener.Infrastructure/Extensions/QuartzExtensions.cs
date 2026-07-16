using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using UrlShortener.Infrastructure.BackgorundJobs;

namespace UrlShortener.Infrastructure.Extensions;

internal static class QuartzExtensions
{
    public static IServiceCollection AddQuartzWithJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
            options.UsePersistentStore(storeOptions =>
            {
                storeOptions.UseProperties = true;

                storeOptions.UsePostgres(config =>
                {
                    config.ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
                    config.ConnectionString = configuration.GetConnectionString("Local")!;

                    config.TablePrefix = "quartz.qrtz_";
                });

                storeOptions.PerformSchemaValidation = false;
                storeOptions.UseNewtonsoftJsonSerializer();
            });

            services.AddHostedService<RecurringJobScheduler>();
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
