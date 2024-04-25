using Microsoft.AspNetCore.SignalR.Client;

namespace AkkaChat.Client;

public class ChatContext : IChatHubClient
{
    private readonly HubConnection _connection;
    private readonly MainForm _form;

    public ChatContext(HubConnection connection, MainForm form)
    {
        _connection = connection;
        _form = form;

        connection.Closed += OnConnectionClosed;
        connection.Reconnected += OnReconnected;
        connection.Reconnecting += OnReconnecting;
        connection.On<string>(nameof(OnJoinedGroup), OnJoinedGroup);
        connection.On<string>(nameof(OnLeftGroup), OnLeftGroup);
        connection.On<ChatMessage>(nameof(OnMessage), OnMessage);
        
        
        form.Load += OnLoad;
        form.AddGroup += OnFormAddGroup;
        form.SendMessage += OnSendMessage;
    }

    private void OnSendMessage(string groupName, string message)
    {
        _connection.SendAsync("SendMessage", groupName, message);
    }

    private void OnFormAddGroup(string groupName)
    {
        _connection.SendAsync("JoinGroup", groupName);
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        Task.Run(OnLoadAsync);
    }

    private async Task OnLoadAsync()
    {
        try
        {
            await _connection.StartAsync();
            _form.Invoke(() => _form.Connected());
        }
        catch (Exception ex)
        {
            _form.Invoke(() => _form.ConnectionFailed(ex.Message));
        }
        
    }

    private Task OnReconnecting(Exception? arg)
    {
        _form.Invoke(() => _form.Reconnecting());
        return Task.CompletedTask;
    }

    private Task OnReconnected(string? arg)
    {
        _form.Invoke(() => _form.Reconnected());
        return Task.CompletedTask;
    }

    private Task OnConnectionClosed(Exception? arg)
    {
        _form.Invoke(() => _form.OnConnectionClosed());
        return Task.CompletedTask;
    }

    public Task OnMessage(ChatMessage message)
    {
        _form.Invoke(() => _form.OnMessage(message));
        return Task.CompletedTask;
    }

    public Task OnJoinedGroup(string groupName)
    {
        _form.Invoke(() => _form.OnJoinGroup(groupName));
        return Task.CompletedTask;
    }

    public Task OnLeftGroup(string groupName)
    {
        _form.Invoke(() => _form.OnLeftGroup(groupName));
        return Task.CompletedTask;
    }
}