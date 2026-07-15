namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record ProfileImageChangedNotificationRequest(
    string Email,
    string FirstName);
