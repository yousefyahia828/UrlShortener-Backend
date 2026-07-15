namespace UrlShortener.API.Common.Extensions;

public static class HttpContextExtensions
{
    public static string GetBaseUrl(this HttpContext httpContext)
    {
        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    }
}
