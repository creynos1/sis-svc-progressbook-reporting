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
        IReportingClient CreateReportingClientWithTokenMessageHandler(CurrentUserSubjectTokenMessageHandler handler);
        IReportingClient CreateReportingClientUserTokenMessageHandler();
    }

    public class ReportingClientFactory : IReportingClientFactory
    {
        public static readonly ReportingClientFactory Instance = new ReportingClientFactory();

        public IReportingClient CreateReportingClientWithTokenMessageHandler(CurrentUserSubjectTokenMessageHandler handler)
        {
            return new ReportingClient(handler);
        }

        public IReportingClient CreateReportingClientUserTokenMessageHandler()
        {
            return new ReportingClient(
                new CurrentUserSubjectTokenMessageHandler(new CurrentUserSubjectTokenMessageHandlerOptions
                {
                    ClientIdentifier = "ProgressBook.ReportingClient",
                    Scopes = "reporting"
                })
            );
        }
    }
}
