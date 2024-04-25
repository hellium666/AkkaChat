namespace AkkaChat.Server.SessionManagement;

public class SessionsManager : ISessionsManager
{
    private readonly object _lock = new();

    private readonly Dictionary<string, HashSet<string>> _userSessions = new ();
    private readonly Dictionary<string, HashSet<string>> _userGroups= new ();
    private readonly HashSet<string> _groups = new();

    private long _userCount;
    private long _sessionsCount;
    private long _groupCount;
    private long _messagesSent;


    public void OnConnected(string login, string session)
    {
        Interlocked.Increment(ref _sessionsCount);
        lock (_lock)
        {
            var sessions = _userSessions.GetOrAdd(login, ref _userCount);
            sessions.Add(session);
        }
    }

    public void OnDisconnected(string login, string session)
    {
        Interlocked.Decrement(ref _sessionsCount);
        lock (_lock)
        {
            var sessions = _userSessions.GetOrAdd(login);
            sessions.Remove(session);
        }
    }

    public void AddUserToGroup(string login, string groupName)
    {
        lock (_lock)
        {
            var groups = _userGroups.GetOrAdd(login, ref _groupCount);
            groups.Add(groupName);
            if (_groups.Add(groupName))
                Interlocked.Increment(ref _groupCount);
        }
    }
    
    public void RemoveUserFromGroup(string login, string groupName)
    {
        lock (_lock)
        {
            var groups = _userGroups.GetOrAdd(login);
            groups.Remove(groupName);
        }
    }

    public IEnumerable<string> GetUserSessions(string login)
    {
        lock (_lock)
        {
            var copy = _userSessions.GetOrAdd(login).ToArray();
            return copy;
        }
    }
    
    public IEnumerable<string> GetUserGroups(string login)
    {
        lock (_lock)
        {
            var copy = _userGroups.GetOrAdd(login).ToArray();
            return copy;
        }
    }

    public void OnMessage(ChatMessage message)
    {
        Interlocked.Increment(ref _messagesSent);
    }

    public ChatStats GetStats()
    {
        return new ChatStats(
            (int)Interlocked.Read(ref _userCount),
            (int)Interlocked.Read(ref _sessionsCount),
            (int)Interlocked.Read(ref _groupCount),
            (int)Interlocked.Read(ref _messagesSent));
    }
}