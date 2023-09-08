using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IpDeputyApi.Authentication
{
    public class BotAuthenticationHandler : AuthenticationHandler<BotAuthenticationSchemeOptions>
    {
        public BotAuthenticationHandler(IOptionsMonitor<BotAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
                return Task.FromResult(AuthenticateResult.Fail($"Missing Header For Token: {Options.TokenHeaderName}"));

            var token = Request.Headers[Options.TokenHeaderName];

            if (token != Options.BotToken)
                return Task.FromResult(AuthenticateResult.Fail("Incorrect Bot Token"));

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "Bot"),
            };

            var id = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(id);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
