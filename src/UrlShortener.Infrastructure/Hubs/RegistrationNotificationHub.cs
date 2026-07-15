using Microsoft.AspNetCore.SignalR;

namespace UrlShortener.Infrastructure.Hubs;

public sealed class RegistrationNotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];

        if (!string.IsNullOrWhiteSpace(userId) &&
            Guid.TryParse(userId, out Guid id))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"registration:{id}");
        }

        await base.OnConnectedAsync();
    }
}
