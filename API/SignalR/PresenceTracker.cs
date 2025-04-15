namespace API.SignalR;

/// <summary>
/// Tracks user presence and connection status
/// </summary>
public class PresenceTracker
{
    /// <summary>
    /// Dictionary to store online users and their connection IDs
    /// </summary>
    private static readonly Dictionary<string, List<string>> OnlineUsers = new();
    
    /// <summary>
    /// Adds a user connection and returns whether the user is newly online
    /// </summary>
    /// <param name="username">The username of the connecting user</param>
    /// <param name="connectionId">The connection ID of the user</param>
    /// <returns>True if the user is newly online, false if they were already online</returns>
    public Task<bool> UserConnected(string username, string connectionId)
    {
        var isUserOnline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.TryGetValue(username, out var user))
            {
                user.Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, [connectionId]);
                isUserOnline = true;
            }
        }

        return Task.FromResult(isUserOnline);
    }
    
    /// <summary>
    /// Removes a user connection and returns whether the user is now offline
    /// </summary>
    /// <param name="username">The username of the disconnecting user</param>
    /// <param name="connectionId">The connection ID of the user</param>
    /// <returns>True if the user is now offline, false if they still have other connections</returns>
    public Task<bool> UserDisconnected(string username, string connectionId)
    {
        var isUserDisconnected = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.TryGetValue(username, out var user))
            {
                return Task.FromResult(isUserDisconnected);
            }

            user.Remove(connectionId);
            if (user.Count == 0)
            {
                OnlineUsers.Remove(username);
                isUserDisconnected = true;
            }
        }

        return Task.FromResult(isUserDisconnected);
    }
    
    /// <summary>
    /// Gets a list of all currently online users
    /// </summary>
    /// <returns>An array of usernames of online users</returns>
    public Task<string[]> GetOnlineUsers()
    {
        string[] users;
        lock (OnlineUsers)
        {
            users = OnlineUsers.OrderBy(k => k.Key)
                .Select(k => k.Key)
                .ToArray();
        }
        return Task.FromResult(users);
    }
    
    /// <summary>
    /// Gets all connection IDs for a specific user
    /// </summary>
    /// <param name="username">The username to get connections for</param>
    /// <returns>A list of connection IDs for the user</returns>
    public static Task<List<string>> GetConnectionsForUser(string username)
    {
        List<string> connectionsId;
        if (OnlineUsers.TryGetValue(username, out var connections))
        {
            lock (OnlineUsers)
            {
                connectionsId = [..connections];
            }
        }
        else
        {
            connectionsId = [];
        }
        return Task.FromResult(connectionsId);
    }
}