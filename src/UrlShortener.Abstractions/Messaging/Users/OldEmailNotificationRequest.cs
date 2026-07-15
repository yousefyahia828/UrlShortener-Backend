namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record OldEmailNotificationRequest(
    string OldEmailAddress,
    string FirstName);
