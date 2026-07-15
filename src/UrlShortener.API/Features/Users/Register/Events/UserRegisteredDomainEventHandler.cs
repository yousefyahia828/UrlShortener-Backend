using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Register.Events;

internal sealed class UserRegisteredDomainEventHandler(
    IEmailVerificationService emailVerificationService)
    : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(
        UserRegisteredDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return emailVerificationService.SendAccountActivationLink(
            new AccountActivationRequest(
                notification.UserId,
                notification.Email,
                notification.FirstName),
            cancellationToken); ;
    }
}
