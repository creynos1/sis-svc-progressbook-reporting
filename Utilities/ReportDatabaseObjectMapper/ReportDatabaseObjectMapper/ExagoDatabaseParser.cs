using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsExagoDependencies
{
    class ExagoDatabaseParser
    {
        private static readonly string PATH = "ReportDatabaseObjectMap.xlsx";
        private static readonly string TITLE = "Database Object Exago Map";
        private static List<string> HEADERS = new List<string>() { "Database Object Name", "Report Name" };

        private static readonly string ENTITY_NODE = "entity";
        private static readonly string ENTITY_NAME_NODE = "entity_name";
        

        static void Main(string[] args)
        {
            var viewReportDependencies = DatabaseLookup.LookupAllReports()
                .SelectMany(x => ParseExagoContent(x))
                .OrderBy(row => row.databaseObjectName)
                .Select(row => row.ToList());

            ExcelExporter.WriteRows(
                PATH,
                TITLE,
                viewReportDependencies,
                HEADERS);
        }

        private static IEnumerable<ExagoViewRowModel> ParseExagoContent(ReportEntity report)
        {
            return XDocument.Parse(report.Content).Descendants(ENTITY_NODE)
                .Where(entity => entity.Element(ENTITY_NAME_NODE) != null)
                .Select(entity => new ExagoViewRowModel() {
                    databaseObjectName = entity.Element(ENTITY_NAME_NODE).Value,
                    reportName = report.Name
                });
        }
    }
}
