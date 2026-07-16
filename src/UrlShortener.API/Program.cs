using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using UrlShortener.API;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddServices();

builder.Services.AddEndpoints();

builder.Services.AddCors();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseCors(cfg =>
{
    cfg.AllowAnyHeader()
       .AllowAnyMethod()
       .WithOrigins(
            "http://127.0.0.1:5500",
            "https://localhost:5500",
            "https://url-shortener-frontend-nu-beryl.vercel.app")
       .AllowCredentials()
       .SetPreflightMaxAge(TimeSpan.FromHours(1));
});

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseExceptionHandler();
app.UseRateLimiter();

app.MapEndpoints(app.MapGroup("api"));
app.MapHub<RegistrationNotificationHub>("/hubs/registration");
app.MapHub<EmailNotificationHub>("/hubs/email-notifications");

app.Run();