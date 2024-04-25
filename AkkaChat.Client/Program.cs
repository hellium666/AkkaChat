using Microsoft.AspNetCore.SignalR.Client;

namespace AkkaChat.Client;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        string login;

        using (var loginWindow = new LoginForm())
        {
            Application.Run(loginWindow);
            login = loginWindow.Login;
            if (string.IsNullOrEmpty(login))
                return;
        }

        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:20000/hubs/chat", options => options.Headers.Add("X-AkkaChat-Login", login))
            .WithAutomaticReconnect()
            .Build();

        using var mainWindow = new MainForm(login);
        var ctx = new ChatContext(connection, mainWindow);
        Application.Run(mainWindow);
    }
}