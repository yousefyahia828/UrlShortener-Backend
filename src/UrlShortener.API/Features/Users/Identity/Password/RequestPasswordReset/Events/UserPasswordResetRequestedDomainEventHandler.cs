using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.API.Features.Users.Identity.Password.RequestPasswordReset.Events;

internal sealed class UserPasswordResetRequestedDomainEventHandler(
    IPasswordResetService passwordResetService)
    : IDomainEventHandler<UserPasswordResetRequestedDomainEvent>
{
    public Task Handle(
        UserPasswordResetRequestedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return passwordResetService.SendPasswordResetEmail(
            new PasswordResetRequest(
                notification.UserId,
                notification.Email,
                notification.FirstName),
            cancellationToken);
    }
}
