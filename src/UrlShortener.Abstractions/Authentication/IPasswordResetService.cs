using UrlShortener.Abstractions.Messaging.Users;

namespace UrlShortener.Abstractions.Authentication;

public interface IPasswordResetService
{
    public string HashToken(string rawToken);

    public Task SendPasswordResetEmail(
        PasswordResetRequest request,
        CancellationToken cancellationToken = default);

    public Task SendPasswordChangedNotification(
       PasswordChangedNotificationRequest request,
       CancellationToken cancellationToken = default);
}
