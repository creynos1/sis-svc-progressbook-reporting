namespace ProgressBook.Reporting.Data.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class Report : ReportEntity
    {
        public Report() : base(ReportEntityType.Report)
        {
        }

        public IEnumerable<string> DataObjects
        {
            get
            {
                var list = new List<string>();

                if (!string.IsNullOrEmpty(Content))
                {
                    var xdoc = XDocument.Parse(Content);
                    list = xdoc.Descendants().Elements("entity_name").Select(x => x.Value).ToList();
                }

                return list;
            }
        }
    }
}