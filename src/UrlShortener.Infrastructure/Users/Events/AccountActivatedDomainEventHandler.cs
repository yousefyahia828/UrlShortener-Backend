using Microsoft.AspNetCore.SignalR;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.Users.Events;
using UrlShortener.Infrastructure.Hubs;

namespace UrlShortener.Infrastructure.Users.Events;

internal sealed class AccountActivatedDomainEventHandler(
    IHubContext<RegistrationNotificationHub> hubContext)
    : IDomainEventHandler<UserEmailConfirmedDomainEvent>
{
    public async Task Handle(
        UserEmailConfirmedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await hubContext.Clients
            .Group($"registration:{notification.UserId}")
            .SendAsync("AccountActivated", cancellationToken);
    }
}
