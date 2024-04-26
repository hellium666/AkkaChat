using Akka.Actor;
using Akka.Hosting;

namespace AkkaChat.Server.SessionManagement;

public class SessionManagerActor : ReceiveActor
{
    private readonly IActorRef _stats;
    private readonly IActorRef _groupsRegion;

    public SessionManagerActor(IReadOnlyActorRegistry registry)
    {
        _stats = registry.Get<StatsActor>();
        _groupsRegion = registry.Get<UserGroupsActor>();

        Receive<AddSession>(ForwardSession);
        Receive<RemoveSession>(ForwardSession);
        Receive<GetSessions>(ForwardToUser);

        Receive<JoinGroup>(ForwardGroup);
        Receive<LeaveGroup>(ForwardGroup);
        Receive<GetGroups>(_groupsRegion.Forward);

        Receive<ChatMessage>(_stats.Forward);
    }

    private void ForwardSession(IWithLogin m)
    {
        var user = GetOrCreateUserActor(m.Login);
        user.Forward(m);
        _stats.Forward(m);
    }

    private void ForwardGroup(IWithLogin m)
    {
        _groupsRegion.Forward(m);
        _stats.Forward(m);
    }

    private static void ForwardToUser(IWithLogin m)
    {
        var user = GetOrCreateUserActor(m.Login);
        user.Forward(m);
    }
    
    private static IActorRef GetOrCreateUserActor(string login)
    {
        var child = Context.Child(login);
        return child.Equals(ActorRefs.Nobody) 
            ? Context.ActorOf<UserSessionsActor>(login) 
            : child;
    }
}