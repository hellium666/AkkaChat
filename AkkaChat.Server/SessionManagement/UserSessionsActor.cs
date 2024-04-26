using System.Collections.Immutable;
using Akka.Actor;

namespace AkkaChat.Server.SessionManagement;

public class UserSessionsActor : ReceiveActor
{
    private ImmutableHashSet<string> _sessions = ImmutableHashSet<string>.Empty;

    public UserSessionsActor()
    {
        Receive<AddSession>(m => _sessions = _sessions.Add(m.SessionId));
        Receive<RemoveSession>(m => _sessions = _sessions.Add(m.SessionId));
        Receive<GetSessions>(_ => Sender.Tell(new UserSessions(_sessions)));
    }
}