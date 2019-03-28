using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftwareAnswers.ProgressBook.IdentityServer.Client.CentralAdmin.Handlers;
using SoftwareAnswers.ProgressBook.IdentityServer.Client.CentralAdmin.Handlers.OAuth.ClientCredentials;

namespace ProgressBook.Reporting.Client
{
    public interface IReportingClientFactory
    {
        IReportingClient CreateReportingClientWithTokenMessageHandler(TokenMessageHandlerBase handler);
        IReportingClient CreateReportingClientUserSessionMessageHandler();
    }

    public class ReportingClientFactory : IReportingClientFactory
    {
        public static readonly ReportingClientFactory Instance = new ReportingClientFactory();

        public IReportingClient CreateReportingClientWithTokenMessageHandler(TokenMessageHandlerBase handler)
        {
            return new ReportingClient(handler);
        }

        public IReportingClient CreateReportingClientUserSessionMessageHandler()
        {
            return new ReportingClient(
                new UserSessionTokenMessageHandler(new TokenMessageHandlerOptions
                {
                    ClientIdentifier = "ProgressBook.ReportingClient",
                    Scopes = "reporting"
                })
            );
        }
    }
}
