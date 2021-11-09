using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;


namespace ViewsExagoDependencies
{
    class DatabaseLookup
    {
        public static List<ReportEntity> LookupAllReports()
        {
            using (var conext = new ExagoReportContext())
            {
                return conext.ReportEntities
                    .Where(r => r.DistrictId == null && r.UserId == null && r.Content != null)
                    .Select(r => r)
                    .ToList();
            }
        }

        public static List<ReportEntity> LookupAllFolders()
        {
            using (var conext = new ExagoReportContext())
            {
                return conext.ReportEntities
                    .Where(r => r.DistrictId == null && r.UserId == null && r.Content == null)
                    .Select(r => r)
                    .ToList();
            }
        }
    }
}
