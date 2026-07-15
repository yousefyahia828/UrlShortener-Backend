using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using UrlShortener.Domain.Users.Tokens;

namespace UrlShortener.Infrastructure.Identity.EmailVerification;

public sealed class EmailVerificationLinkFactory(
    LinkGenerator linkGenerator,
    IOptions<AppOptions> options)
{
    private readonly AppOptions _options = options.Value;

    public const string AccountAtivationRouteName = "ActivateAccount";
    public const string EmailChangeRouteName = "ChangeEmail";

    public string CreateAccountActivationLink(EmailVerificationToken token)
    {
        var path = linkGenerator.GetPathByName(
            AccountAtivationRouteName,
            new { token.UserId, token.Id }) ??
            throw new InvalidOperationException("Could not create account activation link");

        return $"{_options.BaseUrl}{path}";
    }

    public string CreateChangeEmailConfirmationLink(EmailVerificationToken token)
    {
        var path = linkGenerator.GetPathByName(
            EmailChangeRouteName,
            new { token.UserId, token.Id }) ??
            throw new InvalidOperationException("Could not create change email confirmation link");

        return $"{_options.BaseUrl}{path}";
    }
}
