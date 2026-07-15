using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailChange.Events;

// Send Confrimation link to new email
internal sealed class SendEmailChangeConfirmationHandler(
    IEmailVerificationService emailVerificationService)
    : IDomainEventHandler<UserEmailChangeRequestedDomainEvent>
{
    public Task Handle(
        UserEmailChangeRequestedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return emailVerificationService.SendEmailChangeConfirmation(
            new ChangeEmailConfirmationRequest(
                notification.UserId,
                notification.PendingEmail,
                notification.FirstName),
            cancellationToken);
    }
}
