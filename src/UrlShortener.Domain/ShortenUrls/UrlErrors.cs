using Josephan.CQRS;

namespace UrlShortener.Domain.ShortenUrls;

public static class UrlErrors
{
    public static readonly Error FailedToShorten = Error.Failure(
        "Url.FailedToShorten", "Could not create a shorten url after retries");

    public static readonly Error NotFound = Error.NotFound(
        "Url.NotFound",
        "No URL was found for the given code.");

    public static readonly Error Disabled = Error.NotFound(
        "Url.Disabled",
        "Url is disabled buy its owner");

    public static readonly Error AlreadyEnabled = Error.Conflict(
        "Url.AlreadyEnabled",
        "Url is enabled already");

    public static readonly Error AlreadyDisabled = Error.Conflict(
        "Url.AlreadyDisabled",
        "Url is disabled already");
}
