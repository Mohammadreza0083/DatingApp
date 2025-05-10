using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

/// <summary>
/// SignalR hub for managing user presence and online status
/// </summary>
[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    /// <summary>
    /// Handles client connection to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        // Validate user context
        if (Context.User is null)
        {
            throw new HubException("Cannot get current user claim");
        }

        // Track user connection and notify others
        var isUserOnline = await tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        if (isUserOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUsername());

        // Send current online users to the caller
        var currentOnlineUsers = await tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentOnlineUsers);
    }

    /// <summary>
    /// Handles client disconnection from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Validate user context
        if (Context.User is null)
        {
            throw new HubException("Cannot get current user claim");
        }

        // Track user disconnection and notify others
        var isUserOffline = await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        if (isUserOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsername());

        await base.OnDisconnectedAsync(exception);
    }
}