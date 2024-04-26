using Akka.Actor;
using Akka.Hosting;
using AkkaChat.Server.SessionManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AkkaChat.Server.Hubs;

[Authorize(AuthenticationSchemes = LoginApiKeyAuthenticationHandler.DefaultSchemeName)]
public class ChatHub : Hub<IChatHubClient>
{
    private readonly IActorRef _sessionsManager;

    public ChatHub(IReadOnlyActorRegistry registry)
    {
        _sessionsManager = registry.Get<SessionManagerActor>();
    }

    public async Task SendMessage(string groupName, string message)
    {
        if (string.IsNullOrWhiteSpace(Context.UserIdentifier))
            return;
        
        var msg = new ChatMessage(groupName, DateTimeOffset.Now, Context.UserIdentifier, message);
        _sessionsManager.Tell(msg);
        await Clients.Group(groupName).OnMessage(msg);
    }
    
    public async Task JoinGroup(string groupName)
    {
        var login = Context.UserIdentifier;
        if (string.IsNullOrWhiteSpace(login))
            return;
        
        _sessionsManager.Tell(new JoinGroup(login, groupName));
        var sessions = await _sessionsManager.Ask<UserSessions>(new GetSessions(login));
        await Clients.User(login).OnJoinedGroup(groupName);
        foreach (var session in sessions.Sessions)
        {
            await Groups.AddToGroupAsync(session, groupName);
        }
    }
    
    public async Task LeaveGroup(string groupName)
    {
        var login = Context.UserIdentifier;
        if (string.IsNullOrWhiteSpace(login))
            return;
        
        _sessionsManager.Tell(new LeaveGroup(login, groupName));
        var sessions = await _sessionsManager.Ask<UserSessions>(new GetSessions(login));
        foreach (var session in sessions.Sessions)
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
            _sessionsManager.Tell(new AddSession(login, connectionId));
            var groups = await _sessionsManager.Ask<UserGroups>(new GetGroups(login));
            var client = Clients.Client(connectionId);
            foreach (var group in groups.Groups)
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
            _sessionsManager.Tell(new RemoveSession(login, Context.ConnectionId));

        return base.OnDisconnectedAsync(exception);
    }
}