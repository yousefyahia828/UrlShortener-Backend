namespace UrlShortener.Abstractions.Messaging.Users;

public sealed record PasswordResetRequest(Guid UserId, string Email, string FirstName);