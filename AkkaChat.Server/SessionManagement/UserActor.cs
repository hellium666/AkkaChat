using System.Collections.Immutable;
using Akka.Actor;

namespace AkkaChat.Server.SessionManagement;

public class UserActor : ReceiveActor
{
    private ImmutableHashSet<string> _sessions = ImmutableHashSet<string>.Empty;
    private ImmutableHashSet<string> _groups = ImmutableHashSet<string>.Empty;

    public UserActor()
    {
        Receive<AddSession>(m => _sessions = _sessions.Add(m.SessionId));
        Receive<RemoveSession>(m => _sessions = _sessions.Add(m.SessionId));
        Receive<GetSessions>(_ => Sender.Tell(new UserSessions(_sessions)));
        Receive<JoinGroup>(m => _groups = _groups.Add(m.GroupName));
        Receive<LeaveGroup>(m => _groups = _groups.Remove(m.GroupName));
        Receive<GetGroups>(_ => Sender.Tell(new UserGroups(_groups)));
    }
}