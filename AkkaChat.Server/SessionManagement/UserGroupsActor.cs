using System.Collections.Immutable;
using Akka.Actor;

namespace AkkaChat.Server.SessionManagement;

public class UserGroupsActor : ReceiveActor
{
    private ImmutableHashSet<string> _groups = ImmutableHashSet<string>.Empty;
    
    public UserGroupsActor()
    {
        Receive<JoinGroup>(m => _groups = _groups.Add(m.GroupName));
        Receive<LeaveGroup>(m => _groups = _groups.Remove(m.GroupName));
        Receive<GetGroups>(_ => Sender.Tell(new UserGroups(_groups)));
    }
}