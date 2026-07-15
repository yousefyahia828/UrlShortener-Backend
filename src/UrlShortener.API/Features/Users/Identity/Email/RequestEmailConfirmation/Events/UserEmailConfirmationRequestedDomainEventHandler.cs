using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailConfirmation.Events;

internal sealed class UserEmailConfirmationRequestedDomainEventHandler(
    IEmailVerificationService emailVerificationService)
    : IDomainEventHandler<UserEmailConfirmationRequestedDomainEvent>
{
    public Task Handle(
        UserEmailConfirmationRequestedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return emailVerificationService.SendAccountActivationLink(
            new AccountActivationRequest(
                notification.UserId,
                notification.Email,
                notification.FirstName),
            cancellationToken);
    }
}
