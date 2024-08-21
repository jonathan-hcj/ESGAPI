using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;

namespace ESGAPI
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            ) : base(options, logger, encoder, clock)
        {
        }

        // in large part lifted from https://anuraj.dev/blog/implementing-basic-authentication-in-minimal-webapi/
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? userName = ConfigurationManager.AppSetting["Security:UserName"];
            string? password = ConfigurationManager.AppSetting["Security:Password"];

            /* not going to test if its from swagger, do not do this in production */
            if (Request.Path.Value!= null && Request.Path.Value.StartsWith("/swagger"))
            {

                var claims = new[] { new Claim("name", userName), new Claim(ClaimTypes.Role, "Admin") };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }
            else
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Basic ".Length).Trim();
                
                    var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var credentials = credentialstring.Split(':');

                    if (credentials[0] == userName && credentials[1] == password)
                    {
                        var claims = new[] { new Claim("name", credentials[0]), new Claim(ClaimTypes.Role, "Admin") };
                        var identity = new ClaimsIdentity(claims, "Basic");
                        var claimsPrincipal = new ClaimsPrincipal(identity);
                        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                    }

                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
                }
                else
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
                }
            }
        }

    }
}
