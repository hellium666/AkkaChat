using AkkaChat.Server.SessionManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AkkaChat.Server.Hubs;

[Authorize(AuthenticationSchemes = LoginApiKeyAuthenticationHandler.DefaultSchemeName)]
public class ChatHub : Hub<IChatHubClient>
{
    private readonly ISessionsManager _sessionsManager;

    public ChatHub(ISessionsManager sessionsManager)
    {
        _sessionsManager = sessionsManager;
    }

    public async Task SendMessage(string groupName, string message)
    {
        if (string.IsNullOrWhiteSpace(Context.UserIdentifier))
            return;
        
        var msg = new ChatMessage(groupName, DateTimeOffset.Now, Context.UserIdentifier, message);
        _sessionsManager.OnMessage(msg);
        await Clients.Group(groupName).OnMessage(msg);
    }
    
    public async Task JoinGroup(string groupName)
    {
        var login = Context.UserIdentifier;
        if (string.IsNullOrWhiteSpace(login))
            return;
        
        _sessionsManager.AddUserToGroup(login, groupName);
        var sessions = _sessionsManager.GetUserSessions(login);
        await Clients.User(login).OnJoinedGroup(groupName);
        foreach (var session in sessions)
        {
            await Groups.AddToGroupAsync(session, groupName);
        }
    }
    
    public async Task LeaveGroup(string groupName)
    {
        var login = Context.UserIdentifier;
        if (string.IsNullOrWhiteSpace(login))
            return;
        
        _sessionsManager.RemoveUserFromGroup(login, groupName);
        var sessions = _sessionsManager.GetUserSessions(login);
        foreach (var session in sessions)
        {
            await Groups.RemoveFromGroupAsync(session, groupName);
        }
        
        await Clients.User(login).OnLeftGroup(groupName);
    }

    public override async Task OnConnectedAsync()
    {
        var login = Context.UserIdentifier;
        if (!string.IsNullOrWhiteSpace(login))
        {
            var connectionId = Context.ConnectionId;
            _sessionsManager.OnConnected(login, connectionId);
            var groups = _sessionsManager.GetUserGroups(login);
            var client = Clients.Client(connectionId);
            foreach (var group in groups)
            {
                await client.OnJoinedGroup(group);
                await Groups.AddToGroupAsync(connectionId, group);
            }
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var login = Context.UserIdentifier;
        if (!string.IsNullOrWhiteSpace(login))
            _sessionsManager.OnDisconnected(login, Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}