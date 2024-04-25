namespace AkkaChat.Server.SessionManagement;

public class ChatStats
{
    public ChatStats(int userCount, int sessionsCount, int groupsCount, int messageCount)
    {
        UserCount = userCount;
        SessionsCount = sessionsCount;
        GroupsCount = groupsCount;
        MessageCount = messageCount;
    }

    public int UserCount { get; }
    public int SessionsCount { get; }
    public int GroupsCount { get; }
    public int MessageCount { get; }

    public override string ToString()
    {
        var entries = new[]
        {
            "AkkaChat stats:",
            $"Groups: {GroupsCount}",
            $"Users: {UserCount}",
            $"Sessions: {SessionsCount}",
            $"MessagesSent: {MessageCount}"
        };
        return string.Join("\r\n", entries);
    }
}