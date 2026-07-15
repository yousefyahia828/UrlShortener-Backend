using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace UrlShortener.Infrastructure.Hubs;

[Authorize]
public sealed class EmailNotificationHub : Hub
{
}
