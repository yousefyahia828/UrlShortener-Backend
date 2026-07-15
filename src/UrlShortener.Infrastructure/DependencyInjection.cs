using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
using System.Text;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.ShortenUrls;
using UrlShortener.Domain.Users;
using UrlShortener.Infrastructure.Authentication;
using UrlShortener.Infrastructure.Authentication.Jwt;
using UrlShortener.Infrastructure.Authentication.RefreshToken;
using UrlShortener.Infrastructure.Database;
using UrlShortener.Infrastructure.Extensions;
using UrlShortener.Infrastructure.Identity.EmailVerification;
using UrlShortener.Infrastructure.Identity.Password;
using UrlShortener.Infrastructure.Notifications;
using UrlShortener.Infrastructure.Outbox;
using UrlShortener.Infrastructure.ShortenUrls;
using UrlShortener.Infrastructure.Storage;
using UrlShortener.Infrastructure.Users;

namespace UrlShortener.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AppOptions>(configuration.GetSection("App"));

        return services
            .AddDatabase(configuration)
            .AddServices(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorization()
            .AddQuartzWithJobs(configuration)
            .AddFluentEmail(configuration);
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
        services.AddSingleton<AuditInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(
                        sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>(),
                        sp.GetRequiredService<AuditInterceptor>())
                   .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Password Hasher
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Jwt
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtProvider, JwtProvider>();

        // Refresh
        services.Configure<RefreshTokenOptions>(configuration.GetSection("Refresh"));
        services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();

        // Url Repo
        services.AddScoped<IShortenUrlRepository, ShortenUrlRepository>();

        // Email
        services.AddScoped<EmailVerificationLinkFactory>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        services.AddScoped<IEmailSender, EmailSender>();

        // Notifications
        services.AddScoped<INotificationService, NotificationService>();

        // Password
        services.AddScoped<ResetPasswordTokenLinkFactory>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();

        services.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            return connection;
        });

        // Local Storage
        services.Configure<ProfileImageOptions>(configuration.GetSection(ProfileImageOptions.SectionName));
        services.AddSingleton<ILocalStorage, LocalStorage>();

        // Current User
        services.AddHttpContextAccessor();
        services.AddTransient<ICurrentUser, CurrentUser>();

        // Cache
        services.AddMemoryCache();

        // SignalR
        services.AddSignalR();

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
              {
                  o.SaveToken = true;
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = configuration["Jwt:Issuer"],
                      ValidAudience = configuration["Jwt:Audience"],
                      ClockSkew = TimeSpan.FromSeconds(5),
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                          configuration["Jwt:SecretKey"]!))
                  };
              });

        return services;
    }

    private static IServiceCollection AddFluentEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddFluentEmail(
                configuration["Email:From"],
                "UrlShortener System")
            .AddSmtpSender(
                host: configuration["Email:Host"],
                port: configuration.GetValue<int>("Email:Port"),
                username: configuration["Email:Username"],
                password: configuration["Email:Password"]);

        return services;
    }
}
