using Microsoft.Owin;
using ProgressBook.Reporting.Web;

[assembly: OwinStartup(typeof(Startup))]

namespace ProgressBook.Reporting.Web
{
    using Owin;
    using SoftwareAnswers.ProgressBook.IdentityServer.Consumer.Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCentralIdentityServerBearerTokenAuthentication(
                new CentralIdentityServerBearerTokenAuthenticationOptions
                {
                    EnableScopeValidation = true,
                    PermittedScopes = new[] {"reporting"}
                });
            app.UseCentralTrustedIssuerAuthorityUrlClaimValidation(
                new CentralTrustedIssuerAuthorityUrlClaimValidationOptions());
        }
    }
}