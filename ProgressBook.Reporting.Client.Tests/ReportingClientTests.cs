using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressBook.Reporting.SharedModels;
using SoftwareAnswers.ProgressBook.IdentityServer.Client.CentralAdmin.Handlers;
using SoftwareAnswers.ProgressBook.IdentityServer.Client.CentralAdmin.Handlers.OAuth.ClientCredentials;
using Xunit;

namespace ProgressBook.Reporting.Client.Tests
{
    public class ReportingClientTests 
    {
        [Fact]
        public async void GetReportAsFile_DownloadPDF()
        {
            using (var reportingClient = new ReportingClient(new SharedSingleTokenMessageHandler(new TokenMessageHandlerOptions
            {
                ClientIdentifier = "ProgressBook.ReportingClient",
                Scopes = "reporting"
            })))
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("StartDate", "04/17/2017");
                parameters.Add("StopDate", "04/17/2017");

                var values = new ReportInformation
                {
                    ExportType = ExportType.Pdf,
                    ReportName = "Student\\Student Count Report District Detail",
                    UserContext = new UserContext
                    {
                        DistrictId = new Guid("06D82352-5EB6-42BC-86AB-97A60170AC7E"), // green
                        UserId = new Guid("C7A93BF6-4367-411D-A144-A69D009E6CFB"),
                        DistrictIrn = "050559",
                        AuthorizationPlaceId = new Guid("06D82352-5EB6-42BC-86AB-97A60170AC7E")
                    },
                    TemplateName = string.Empty,
                    FileDownloadToken = string.Empty,
                    Parameters = parameters
                };


                var reports = await reportingClient.ExecuteReport(values);

                // assert
                Assert.NotEmpty(reports.Content);

                // download file
                File.WriteAllBytes(@"C:\Temp\" + reports.Name, reports.Content);
            }
        }

    }
}
