using System.Reflection;
using UrlShortener.API.Common.Abstractions.Behaviors;
using UrlShortener.Infrastructure;

namespace UrlShortener.API;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        services.AddCQRS(options =>
        {
            options.AddHandlersFromAssemblies(
                assembly,
                InfrastructureAssemblyReference.Assembly);

            options.AddCommandBehavior(typeof(CommandLoggingBehavior<,>));
            options.AddQueryBehavior(typeof(QueryLoggingBehavior<,>));
            options.AddCommandBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}
