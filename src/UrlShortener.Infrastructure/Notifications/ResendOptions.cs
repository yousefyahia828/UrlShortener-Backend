namespace UrlShortener.Infrastructure.Notifications;

internal sealed record ResendOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string From { get; init; } = string.Empty;
    public string FromName { get; init; } = string.Empty;
}
