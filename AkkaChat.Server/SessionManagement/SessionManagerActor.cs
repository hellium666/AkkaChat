using Akka.Actor;

namespace AkkaChat.Server.SessionManagement;

public class SessionManagerActor : ReceiveActor
{
    private readonly HashSet<string> _groups = new();

    private int _userCount;
    private int _sessionCount;
    private int _messageCount;
    public SessionManagerActor()
    {
        Receive<AddSession>(m =>
        {
            _sessionCount++;
            ForwardToUser(m);
        });

        Receive<RemoveSession>(m =>
        {
            _sessionCount--;
            ForwardToUser(m);
        });

        Receive<GetSessions>(ForwardToUser);

        Receive<JoinGroup>(m =>
        {
            _groups.Add(m.GroupName);
            ForwardToUser(m);
        });

        Receive<LeaveGroup>(ForwardToUser);
        Receive<GetGroups>(ForwardToUser);

        Receive<ChatMessage>(_ => _messageCount++);
        Receive<GetStats>(_ => Sender.Tell(BuildStats()));
    }

    private ChatStats BuildStats()
    {
        return new ChatStats(_userCount, _sessionCount, _groups.Count, _messageCount);
    }

    private void ForwardToUser(IWithLogin m)
    {
        var user = GetOrCreateUserActor(m);
        user.Forward(m);
    }

    private IActorRef GetOrCreateUserActor(IWithLogin m)
    {
        var child = Context.Child(m.Login);
        if (!child.Equals(ActorRefs.Nobody)) 
            return child;

        _userCount++;
        return Context.ActorOf<UserActor>(m.Login);
    }
}