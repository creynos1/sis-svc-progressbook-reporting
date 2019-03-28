using System.Collections.Generic;

namespace ProgressBook.Reporting.SharedModels
{
    public class ReportInformation
    {
        public ReportInformation()
        {
            Parameters = new Dictionary<string, string>();
        }

        public string ReportName { get; set; }

        public string TemplateName { get; set; }

        public ExportType ExportType { get; set; }
        
        public IDictionary<string, string> Parameters { get; set; }

        public string FileDownloadToken { get; set; }

        public UserContext UserContext { get; set; }
    }
}