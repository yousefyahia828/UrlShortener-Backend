using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmail.Events;

public sealed class UserEmailConfirmedDomainEventHandler(
    IEmailVerificationService emailVerificationService)
    : IDomainEventHandler<UserEmailConfirmedDomainEvent>
{
    public Task Handle(
        UserEmailConfirmedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return emailVerificationService.SendWelcomeEmail(
            new WelcomeEmailRequest(
                notification.UserId,
                notification.Email,
                notification.FirstName),
            cancellationToken);
    }
}
