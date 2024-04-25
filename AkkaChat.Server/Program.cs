using AkkaChat.Server;
using AkkaChat.Server.Hubs;
using AkkaChat.Server.SessionManagement;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, ChatHubUserIdProvider>();
builder.Services.AddAuthentication().AddLoginApiKey();

builder.Services.AddSingleton<ISessionsManager, SessionsManager>();

var app = builder.Build();

app.MapGet("/", (ISessionsManager s) => s.GetStats().ToString());

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapHub<ChatHub>("/hubs/chat"));

app.Run();