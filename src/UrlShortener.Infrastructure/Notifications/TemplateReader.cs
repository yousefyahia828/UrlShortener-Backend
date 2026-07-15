using System.Reflection;

namespace UrlShortener.Infrastructure.Notifications;

internal sealed class TemplateReader
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    public static async Task<string> ReadTemplateAsync(
        string templateName,
        Type resourceMarkerType,
        CancellationToken cancellationToken = default)
    {
        var stream = _assembly.GetManifestResourceStream(
            resourceMarkerType,
            templateName)
            ?? throw new InvalidOperationException(
                $"Could not find resource '{templateName}'.");

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}
