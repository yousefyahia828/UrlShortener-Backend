namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record PasswordChangedNotificationRequest(string Email, string FirstName);
