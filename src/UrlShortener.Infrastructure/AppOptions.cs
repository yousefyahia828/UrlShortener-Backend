namespace UrlShortener.Infrastructure;

public sealed class AppOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string FrontendUrl { get; init; } = string.Empty;
}
