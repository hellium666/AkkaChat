using Akka.Actor;

namespace AkkaChat.Server.SessionManagement;

public class StatsActor : ReceiveActor
{
    private readonly HashSet<string> _groups = new();
    private readonly HashSet<string> _users = new();

    private int _sessionCount;
    private int _messageCount;

    public StatsActor()
    {
        Receive<AddSession>(m =>
        {
            _sessionCount++;
            return _users.Add(m.Login);
        });
        Receive<RemoveSession>(_ => _sessionCount--);
        Receive<JoinGroup>(m => _groups.Add(m.GroupName));
        Receive<ChatMessage>(_ => _messageCount++);
        Receive<GetStats>(_ => Sender.Tell(BuildStats()));
    }

    private ChatStats BuildStats()
    {
        return new ChatStats(_users.Count, _sessionCount, _groups.Count, _messageCount);
    }
}