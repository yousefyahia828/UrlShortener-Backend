using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace UrlShortener.Infrastructure.Identity.Password;

public sealed class ResetPasswordTokenLinkFactory(IOptions<AppOptions> options)
{
    private readonly AppOptions _options = options.Value;

    public string CreatePasswordResetLink(string token)
    {
        // I can't use LinkGenerator or HttpContext because this is done in the background and http context is null
        string uri = _options.BaseUrl + "/api/auth/password-reset-link";

        return QueryHelpers.AddQueryString(uri, "token", token);
    }
}
