using Akka.Actor;
using Akka.Hosting;
using AkkaChat.Server;
using AkkaChat.Server.Hubs;
using AkkaChat.Server.SessionManagement;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, ChatHubUserIdProvider>();
builder.Services.AddAuthentication().AddLoginApiKey();

builder.Services.AddAkka("AkkaChat", b => b.WithActors((system, registry) =>
{
    var userManager = system.ActorOf(Props.Create(() => new SessionManagerActor()), "UserManager");
    registry.TryRegister<SessionManagerActor>(userManager);
}));

var app = builder.Build();

app.MapGet("/", async (IReadOnlyActorRegistry s) =>
{
    var stats = await s.Get<SessionManagerActor>().Ask<ChatStats>(GetStats.Instance);
    return stats.ToString();
});

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapHub<ChatHub>("/hubs/chat"));

app.Run();