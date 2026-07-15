namespace UrlShortener.API.Common.Abstractions.Presentation;

public static class AuthHelpers
{
    public const string RefreshTokenCookieName = "refresh_token";

    public static void SetRefreshCookie(HttpContext httpContext, string refreshToken, DateTime expiresOnUtc)
    {
        httpContext.Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresOnUtc,
                Path = "/",
            });
    }

    public static void RemoveRefreshCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
            });
    }
}
