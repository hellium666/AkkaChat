using Akka.Cluster.Sharding;

namespace AkkaChat.Server.SessionManagement;

public class UserMessageExtractor : HashCodeMessageExtractor
{
    public UserMessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards)
    {
    }

    public override string? EntityId(object message)
    {
        return message switch
        {
            IWithLogin withLogin => withLogin.Login,
            _ => throw new InvalidOperationException($"Сообщение {message.GetType().Name} не поддерживается.")
        };
    }
}