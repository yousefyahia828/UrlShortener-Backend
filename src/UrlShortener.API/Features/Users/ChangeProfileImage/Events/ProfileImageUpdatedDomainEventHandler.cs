using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.ChangeProfileImage.Events;

internal sealed class ProfileImageUpdatedDomainEventHandler(
    INotificationService notificationService)
    : IDomainEventHandler<ProfileImageUpdatedDomainEvent>
{
    public async Task Handle(
        ProfileImageUpdatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await notificationService.SendProfileImageChangedNotification(
            new ProfileImageChangedNotificationRequest(
                notification.Email,
                notification.FirstName),
            cancellationToken);
    }
}
