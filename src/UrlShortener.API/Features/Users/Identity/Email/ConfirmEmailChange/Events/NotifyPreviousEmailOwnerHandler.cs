using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmailChange.Events;

internal sealed class NotifyPreviousEmailOwnerHandler(
    IEmailVerificationService emailVerificationService)
    : IDomainEventHandler<UserEmailChangedDomainEvent>
{
    public Task Handle(UserEmailChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        return emailVerificationService.SendOldEmailNotification(
            new OldEmailNotificationRequest(
                notification.OldEmail,
                notification.FirstName),
            cancellationToken);
    }
}