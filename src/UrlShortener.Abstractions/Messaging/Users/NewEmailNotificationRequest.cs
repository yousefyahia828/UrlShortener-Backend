namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record NewEmailNotificationRequest(
    string NewEmailAddress,
    string FirstName);