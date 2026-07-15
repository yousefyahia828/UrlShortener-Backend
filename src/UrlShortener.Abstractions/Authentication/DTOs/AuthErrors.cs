using Josephan.CQRS;

namespace UrlShortener.Abstractions.Authentication.DTOs;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "Auth.InvalidCredentials", "Invalid email or password");

    public static readonly Error InvalidToken = Error.Failure(
        "Auth.InvalidToken", "The provided token is invalid");

    public static readonly Error InvalidPassword = Error.Unauthorized(
        "Auth.InvalidPassword", "The provided password is invalid");

    public static Error EmailNotConfirmed(DateTime createdOnUtc)
    {
        DateTime deletionOnUtc = createdOnUtc.AddDays(3);

        return Error.Unauthorized(
            "User.EmailNotConfirmed",
            $"Your email address has not been confirmed yet. " +
            $"This account was created on {createdOnUtc:yyyy-MM-dd HH:mm} UTC and will be permanently deleted on {deletionOnUtc:yyyy-MM-dd HH:mm} UTC if you do not confirm your email before then.");
    }
}
