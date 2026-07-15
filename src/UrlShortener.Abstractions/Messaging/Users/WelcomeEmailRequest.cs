namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record WelcomeEmailRequest(
    Guid UserId,
    string Email,
    string FirstName);
