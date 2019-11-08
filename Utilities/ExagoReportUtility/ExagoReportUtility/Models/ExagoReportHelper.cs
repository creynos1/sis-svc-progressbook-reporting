using System.Collections.Generic;

namespace ExagoReportUtility.Models
{
    public class ExagoReportHelper
    {
        public List<ReportTitle> ReportTitles { get; set; }
        public List<ReportFilter> ReportFilters { get; set; }
        public List<ReportSort> ReportSorts { get; set; }
        public List<ReportDescription> ReportDescriptions { get; set; }
        public List<ReportEntity> ReportEntityList { get; set; }

        public Dictionary<string, ReportEntity> ReportTitleDictionary { get; set; }


        public ExagoReportHelper()
        {
            this.ReportTitles = new List<ReportTitle>();
            this.ReportFilters = new List<ReportFilter>();
            this.ReportSorts = new List<ReportSort>();
            this.ReportDescriptions = new List<ReportDescription>();
            this.ReportEntityList = new List<ReportEntity>();
            this.ReportTitleDictionary = new Dictionary<string, ReportEntity>();
        }
    }
}
