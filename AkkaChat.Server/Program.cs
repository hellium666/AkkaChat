using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Akka.Remote.Hosting;
using AkkaChat.Server;
using AkkaChat.Server.Hubs;
using AkkaChat.Server.SessionManagement;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, ChatHubUserIdProvider>();
builder.Services.AddAuthentication().AddLoginApiKey();

builder.Services.AddAkka("AkkaChat", akka =>
{
    var messageExtractor = new UserMessageExtractor(20);
    akka.WithRemoting("localhost", int.Parse(Environment.GetEnvironmentVariable("AKKA_CHAT_CLUSTER_PORT") ?? "0"))
        .WithClustering(new ClusterOptions
        {
            Roles = new[] { "chat" },
            SeedNodes = new[] { "akka.tcp://AkkaChat@localhost:1701" }
        })
        .WithSingleton<StatsActor>("Stats", Props.Create<StatsActor>(), new ClusterSingletonOptions { Role = "chat" })
        .WithShardRegion<UserGroupsActor>("UserGroups", _ => Props.Create<UserGroupsActor>(), messageExtractor,
            new ShardOptions { ShouldPassivateIdleEntities = false })
        .WithActors((s, r) =>
        {
            var sessions = s.ActorOf(Props.Create<UserSessionsActor>(), "UserSessions");
            r.Register<UserSessionsActor>(sessions);
            
            var manager = s.ActorOf(Props.Create(() => new SessionManagerActor(r)), "SessionManager");
            r.Register<SessionManagerActor>(manager);
        });
});

var app = builder.Build();

app.MapGet("/", async (IReadOnlyActorRegistry s) =>
{
    var stats = await s.Get<StatsActor>().Ask<ChatStats>(GetStats.Instance);
    var state = await s.Get<UserGroupsActor>().Ask<CurrentShardRegionState>(GetShardRegionState.Instance);
    return $"{stats}\r\n{state}";
});

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapHub<ChatHub>("/hubs/chat"));

app.Run();