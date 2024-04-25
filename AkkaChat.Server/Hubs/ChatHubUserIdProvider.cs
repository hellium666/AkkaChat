using Microsoft.AspNetCore.SignalR;

namespace AkkaChat.Server.Hubs;

public class ChatHubUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.Identity?.Name;
    }
}