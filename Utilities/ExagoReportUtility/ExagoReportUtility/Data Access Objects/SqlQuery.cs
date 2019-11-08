using ExagoReportUtility.Models;
using System.Text;

namespace ExagoReportUtility.DataAccessObjects
{
  public class SqlQuery
    {
        public ExagoReportHelper ExagoReportHelper { get; set; }
        public SqlQuery(ExagoReportHelper exagoReportHelper)
        {
            this.ExagoReportHelper = exagoReportHelper;
        }

        public string GetReportSqlQuery()
        {
            var reportTitles = new StringBuilder();
            for(int i =0; i <= ExagoReportHelper.ReportTitles.Count - 1; i++)
            {
                reportTitles.Append("'").Append(ExagoReportHelper.ReportTitles[i].DatabaseTitle)
                    .Append("' ");
                reportTitles.Append((i == ExagoReportHelper.ReportTitles.Count - 1 ? "" : "," ));
            }
            return $@"SELECT reportEntityId, name, description, content
                        FROM coreReports.ReportEntity
                            WHERE name IN({reportTitles})
                                AND userid IS NULL
                                AND districtId IS NULL";
        }
    }
}
