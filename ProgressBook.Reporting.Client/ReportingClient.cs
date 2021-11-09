namespace ProgressBook.Reporting.Client
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ProgressBook.Reporting.Client.Models;
    using ProgressBook.Reporting.SharedModels;
    using ProgressBook.Shared.Utilities.Threading.Tasks;
    using SoftwareAnswers.ProgressBook.IdentityServer.Client.CentralAdmin.Handlers;

    public interface IReportingClient : IDisposable
    {
        Task<ReportResult> ExecuteReport(ReportInformation reportInformation);
        Task<ReportUrl> GetReportExecutionUrl(ReportUrlModel reportInformation);
        Task<ReportUrl> GetAdHocReportDesignerUrl(UserContext userContext);
    }

    public class ReportingClient : BaseApiClient, IReportingClient
    {
        private readonly HttpClient _httpClient;

        public ReportingClient(TokenMessageHandlerBase handler) : base(handler, new ExagoUri())
        {
            _httpClient = GetHttpClient();
        }

        public async Task<ReportResult> ExecuteReport(ReportInformation reportInformation)
        {
            var client = ClientSetup();
            var response = await client.PostAsJsonAsync("Report/ExecuteReport", reportInformation);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = GetFileName(reportInformation);
            var contentType = GetContentType(reportInformation.ExportType);

            return new ReportResult
                   {
                       Content = bytes,
                       Name = fileName,
                       ContentType = contentType
                   };
        }

        public async Task<ReportUrl> GetReportExecutionUrl(ReportUrlModel reportInformation)
        {
            var client = ClientSetup();
            var response = await client.PostAsJsonAsync("Report/ReportExecutionUrl", reportInformation);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return await response.Content.ReadAsAsync<ReportUrl>();
        }

        public async Task<ReportUrl> GetAdHocReportDesignerUrl(UserContext userContext)
        {
            var client = ClientSetup();
            var response = await client.PostAsJsonAsync("Report/AdHocReportDesignerUrl", userContext);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return await response.Content.ReadAsAsync<ReportUrl>();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private string GetContentType(ExportType exportType)
        {
            switch (exportType)
            {
                case ExportType.Csv:
                    return "text/csv";

                case ExportType.Excel:
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                case ExportType.Html:
                    return "text/html";

                case ExportType.Pdf:
                    return "application/pdf";

                case ExportType.Rtf:
                case ExportType.Word:
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                default:
                    throw new ArgumentOutOfRangeException(nameof(exportType), exportType, "Unsupported exportType");
            }
        }

        private static string GetFileName(ReportInformation reportInformation)
        {
            return $"{reportInformation.ReportName.Split('\\').Last()}.{GetFileExtension(reportInformation.ExportType)}";
        }

        private static string GetFileExtension(ExportType exportType)
        {
            switch (exportType)
            {
                case ExportType.Csv:
                    return "csv";

                case ExportType.Excel:
                    return "xlsx";

                case ExportType.Html:
                    return "html";

                case ExportType.Pdf:
                    return "pdf";

                case ExportType.Rtf:
                case ExportType.Word:
                    return "docx";

                default:
                    throw new ArgumentOutOfRangeException(nameof(exportType), exportType, "Unsupported exportType");
            }
        }

        private HttpClient ClientSetup()
        {
            return _httpClient;
        }
    }

    public static class ReportingClientExtensions
    {
        public static ReportResult ExecuteReportSync(this IReportingClient client, ReportInformation reportInformation)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return AsyncHelper.RunSync(() => client.ExecuteReport(reportInformation));
        }

        public static ReportUrl GetReportExecutionUrlSync(this IReportingClient client, ReportUrlModel reportInformation)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return AsyncHelper.RunSync(() => client.GetReportExecutionUrl(reportInformation));
        }

        public static ReportUrl GetAdHocReportDesignerUrlSync(this IReportingClient client, UserContext userContext)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return AsyncHelper.RunSync(() => client.GetAdHocReportDesignerUrl(userContext));
        }
    }
}