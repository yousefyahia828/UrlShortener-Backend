using Josephan.CQRS;

namespace UrlShortener.Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "User.NotFound",
        "The user was not found.");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "User.EmailNotUnique",
        "The email address is already in use.");

    public static readonly Error EmailAlreadyConfirmed = Error.Conflict(
        "User.EmailAlreadyConfirmed",
        "The email address has already been confirmed.");

    public static readonly Error SameName = Error.Conflict(
        "User.SameName",
        "The new name must be different from the current name");

    public static readonly Error SameEmail = Error.Conflict(
        "User.SameEmail",
        "The new email address must be different from the current email address.");

    public static readonly Error NoPendingEmailChange = Error.Conflict(
        "User.NoPendingEmailChange",
        "There is no pending email change to confirm.");

    public static readonly Error SamePassword = Error.Conflict(
        "User.SamePassword",
        "The new password must be different from the current password.");

    public static readonly Error EmptyProfileImage = Error.Failure(
        "User.ProfileImage", "Profile image is empty.");

    public static readonly Error InvalidProfileImageExtension = Error.Failure(
        "User.ProfileImage", "Profile image extension is not allowed.");

    public static readonly Error InvalidProfileImageContentType = Error.Failure(
        "User.ProfileImage", "Profile image content type is not allowed.");

    public static Error LargeProfileImage(int sizeInMegaBytes) => Error.Failure(
    "User.LargeProfileImage",
    $"Image size must not exceed {sizeInMegaBytes} MB.");
}