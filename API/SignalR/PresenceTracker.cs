namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = new();
    
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