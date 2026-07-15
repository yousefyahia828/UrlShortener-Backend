using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmailChange.Events;

internal sealed class NotifyNewEmailOwnerHandler(
    IEmailVerificationService emailVerificationService)
    : IDomainEventHandler<UserEmailChangedDomainEvent>
{
    public Task Handle(
        UserEmailChangedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return emailVerificationService.SendNewEmailNotification(
            new NewEmailNotificationRequest(
                notification.NewEmail,
                notification.FirstName),
            cancellationToken);
    }
}