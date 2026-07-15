using UrlShortener.Abstractions.Messaging.Users;

namespace UrlShortener.Abstractions.Infrastructure;

public interface INotificationService
{
    Task SendProfileImageChangedNotification(
        ProfileImageChangedNotificationRequest request,
        CancellationToken cancellationToken = default);
}