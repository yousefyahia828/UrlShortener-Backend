using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Identity.Password.ChangePassword.Events;

internal sealed class UserPasswordChangedDomainEventHandler(
    IPasswordResetService passwordResetService)
    : IDomainEventHandler<UserPasswordChangedDomainEvent>
{
    public Task Handle(
        UserPasswordChangedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return passwordResetService.SendPasswordChangedNotification(
            new PasswordChangedNotificationRequest(
                notification.Email,
                notification.FirstName),
            cancellationToken);
    }
}
