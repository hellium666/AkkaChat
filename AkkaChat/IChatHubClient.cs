namespace AkkaChat;

public interface IChatHubClient
{
    Task OnMessage(ChatMessage message);
    Task OnJoinedGroup(string groupName);
    Task OnLeftGroup(string groupName);
}