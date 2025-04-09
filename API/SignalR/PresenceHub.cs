using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;
[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User is null)
        {
            throw new HubException("Cannot get current user claim");
        }
        var isUserOnline = await tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        if(isUserOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUsername());
        var currentOnlineUsers = await tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentOnlineUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User is null)
        {
            throw new HubException("Cannot get current user claim");
        }
        var isUserOffline = await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        if(isUserOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsername());
        await base.OnDisconnectedAsync(exception);
    }
}