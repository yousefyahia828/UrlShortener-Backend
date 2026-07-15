namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record AccountActivationRequest(
    Guid UserId,
    string Email,
    string FirstName);
