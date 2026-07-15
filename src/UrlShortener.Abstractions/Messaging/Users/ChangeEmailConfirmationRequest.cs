namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record ChangeEmailConfirmationRequest(
    Guid UserId,
    string PendingEmail,
    string FirstName);
