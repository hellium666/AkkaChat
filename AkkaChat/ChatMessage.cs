namespace AkkaChat;

public class ChatMessage
{
    public ChatMessage(string groupName, DateTimeOffset timestamp, string from, string message)
    {
        GroupName = groupName;
        From = from;
        Message = message;
        Timestamp = timestamp;
    }

    public string GroupName { get; }
    public DateTimeOffset Timestamp { get; }
    public string From { get; }
    public string Message { get; }
}