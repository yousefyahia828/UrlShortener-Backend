namespace UrlShortener.Infrastructure.Notifications;

internal sealed class EmailSettings
{
    public const string ConfigurationSection = "EmailSettings";

    public string SenderEmail { get; init; } = string.Empty;
}
