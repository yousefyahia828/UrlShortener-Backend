using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.API.Common.Extensions;

public static class WebApplicationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
    }
}
