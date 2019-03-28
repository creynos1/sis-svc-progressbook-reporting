using System.Collections.Generic;

namespace ProgressBook.Reporting.Data.Entities
{
    public class ReportViewInfo
    {
        public string ViewName { get; set; }
        public string SchemaName { get; set; }
        public string EntityName { get; set; }
        public string Category { get; set; }
        public string KeyColumns { get; set; }
        public string TenantColumns { get; set; }
        public List<KeyValuePair<string, string>> MetaColumns { get; set; }
    }
}