using Josephan.CQRS;
using Microsoft.AspNetCore.SignalR;
using UrlShortener.Domain.Users.Events;
using UrlShortener.Infrastructure.Hubs;

namespace UrlShortener.Infrastructure.Users.Events;

internal sealed class UserEmailChangedDomainEventHandler(
    IHubContext<EmailNotificationHub> hubContext)
    : INotificationHandler<UserEmailChangedDomainEvent>
{
    public async Task Handle(
        UserEmailChangedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await hubContext.Clients.Users(notification.UserId.ToString())
                 .SendAsync("PendingEmailConfirmed", cancellationToken);
    }
}
