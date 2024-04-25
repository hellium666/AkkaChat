namespace AkkaChat.Server.SessionManagement;

public interface ISessionsManager
{
    void OnConnected(string login, string session);
    void OnDisconnected(string login, string session);
    void AddUserToGroup(string login, string groupName);
    void RemoveUserFromGroup(string login, string groupName);
    IEnumerable<string> GetUserSessions(string login);
    IEnumerable<string> GetUserGroups(string login);

    void OnMessage(ChatMessage message);

    ChatStats GetStats();
}