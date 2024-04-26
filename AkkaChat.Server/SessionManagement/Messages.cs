using System.Collections.Immutable;

namespace AkkaChat.Server.SessionManagement;

public interface IWithLogin
{
    string Login { get; }
}

public class AddSession : IWithLogin
{
    public AddSession(string login, string sessionId)
    {
        Login = login;
        SessionId = sessionId;
    }

    public string Login { get; }
    public string SessionId { get; }
}

public class RemoveSession : IWithLogin
{
    public RemoveSession(string login, string sessionId)
    {
        Login = login;
        SessionId = sessionId;
    }

    public string Login { get; }
    public string SessionId { get; }
}

public class GetSessions : IWithLogin
{
    public GetSessions(string login)
    {
        Login = login;
    }

    public string Login { get; }
}

public class UserSessions
{
    public UserSessions(ImmutableHashSet<string> sessions)
    {
        Sessions = sessions;
    }

    public ImmutableHashSet<string> Sessions { get; }
}

public class JoinGroup : IWithLogin
{
    public JoinGroup(string login, string groupName)
    {
        Login = login;
        GroupName = groupName;
    }

    public string Login { get; }
    public string GroupName { get; }
}

public class LeaveGroup : IWithLogin
{
    public LeaveGroup(string login, string groupName)
    {
        Login = login;
        GroupName = groupName;
    }

    public string Login { get; }
    public string GroupName { get; }
}

public class GetGroups : IWithLogin
{
    public GetGroups(string login)
    {
        Login = login;
    }

    public string Login { get; }
}

public class UserGroups
{
    public UserGroups(ImmutableHashSet<string> groups)
    {
        Groups = groups;
    }

    public ImmutableHashSet<string> Groups { get; }
}

public class GetStats
{
    public static readonly GetStats Instance = new();
}
