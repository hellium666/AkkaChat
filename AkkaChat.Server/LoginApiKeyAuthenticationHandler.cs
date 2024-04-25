using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AkkaChat.Server;

public class LoginApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string HeaderName => Authentication.HeaderName;
}

public class LoginApiKeyAuthenticationHandler : AuthenticationHandler<LoginApiKeyAuthenticationOptions>
{
    public const string DefaultSchemeName = "LoginApiKey";
    
    private static readonly Task<AuthenticateResult> NoResult = Task.FromResult(AuthenticateResult.NoResult());
    private static readonly Task<AuthenticateResult> FailResult = Task.FromResult(AuthenticateResult.Fail("Invalid login"));

    public LoginApiKeyAuthenticationHandler(IOptionsMonitor<LoginApiKeyAuthenticationOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(Options.HeaderName, out var login)) 
            return NoResult;

        if (string.IsNullOrWhiteSpace(login))
            return FailResult;

        var claims = new[] { new Claim(ClaimTypes.Name, login) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public static class LoginApiKeyAuthenticationExtension
{
    

    public static void AddLoginApiKey(this AuthenticationBuilder builder)
    {

        builder.AddScheme<LoginApiKeyAuthenticationOptions, LoginApiKeyAuthenticationHandler>(
            LoginApiKeyAuthenticationHandler.DefaultSchemeName,
            _ => { });
    }
}