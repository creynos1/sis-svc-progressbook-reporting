namespace ProgressBook.Reporting.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using ProgressBook.Reporting.Client.Models;
    using ProgressBook.Shared.Security.Models;
    using ProgressBook.Shared.Utilities.Data;

    public interface IAdHocReportsRepository
    {
        Task<IEnumerable<AdHocReport>> GetAdHocReports(Guid userId, Guid districtId, IEnumerable<ResourceTree> resources);

        Task<IEnumerable<AdHocReport>> GetVendorExtractReports(Guid userId, Guid districtId, IEnumerable<ResourceTree> resources);
    }

    public class AdHocReportsRepository : IAdHocReportsRepository
    {
        public async Task<IEnumerable<AdHocReport>> GetAdHocReports(Guid userId, Guid districtId, IEnumerable<ResourceTree> resources)
        {
            var dbCommandExecutor =
                new DbCommandExecutor(new ConfigurationConnectionStringProvider("StudentInformation"));

            var commandText = BuildQuery(resources);
            var placeParameter = new SqlParameter("@placeId", SqlDbType.UniqueIdentifier) {Value = districtId};
            var userParameter = new SqlParameter("@userId", SqlDbType.UniqueIdentifier) {Value = userId};
            return await dbCommandExecutor.ExecuteReaderAsync<AdHocReport>(commandText,
                                                                           CommandType.Text,
                                                                           placeParameter,
                                                                           userParameter);
        }

        public async Task<IEnumerable<AdHocReport>> GetVendorExtractReports(Guid userId, Guid districtId, IEnumerable<ResourceTree> resources)
        {
            var dbCommandExecutor = new DbCommandExecutor(new ConfigurationConnectionStringProvider("StudentInformation"));

            var commandText = HasVendorExtractQuery(resources);
            var placeParameter = new SqlParameter("@placeId", SqlDbType.UniqueIdentifier) { Value = districtId };
            var userParameter = new SqlParameter("@userId", SqlDbType.UniqueIdentifier) { Value = userId };
            return await dbCommandExecutor.ExecuteReaderAsync<AdHocReport>(commandText, CommandType.Text, placeParameter, userParameter);
        }

        private string BuildQuery(IEnumerable<ResourceTree> resources)
        {
            var resourceNames = string.Join(",", resources.Select(x => "'" + x.ResourceName + "'").Append("'AdHocReports.ReportFolders.MyReports'").ToArray());

            var query = @"
                    SELECT
                        rpt.ReportEntityId,
                        REPLACE(rpt.[Path], '.wrx', '') AS Name,
                        rpt.DisplayName,
                        rpt.Description
                    FROM [CoreReports].[FlattenedReports] rpt
                    WHERE (rpt.DistrictId = @placeId OR rpt.DistrictId IS NULL)
                    AND (rpt.UserId = @userId OR rpt.UserId IS NULL)
                    AND (rpt.BaseSecurityString IN (" + resourceNames + @"))
                ";

            return query;
        }

        private string HasVendorExtractQuery(IEnumerable<ResourceTree> resources)
        {
            var resourceNames = string.Join(",", resources.Select(x => "'" + x.ResourceName + "'").ToArray());

            var query = @"
                    SELECT
                        rpt.ReportEntityId,
                        'My Reports' AS ResourceName,
                        rpt.[Path] AS Name,
                        REPLACE(rpt.Name, '.wrx', '') AS DisplayName,
                        re.Description,
                        re.DateModified,
                        re.ModifiedBy
                    FROM [CoreReports].ReportEntityInfo rpt
                        LEFT JOIN [CoreReports].ReportEntityInfo fldr ON rpt.ParentId = fldr.ReportEntityId
                        INNER JOIN [CoreReports].ReportEntity re ON rpt.ReportEntityId = re.ReportEntityId
                    WHERE
                        re.EntityType = 1
                    AND re.IsInternal = 0
                    AND fldr.[Path] = 'My Reports'
                    AND (rpt.DistrictId = @placeId OR rpt.DistrictId IS NULL)
                    AND (rpt.UserId = @userId OR rpt.UserId IS NULL)
                    AND CAST(re.Content AS xml).value('(/*//custom_value[./id=''Is_Vendor_Extract'' and ./value=''True''])[1]','VARCHAR(25)') IS NOT NULL
                ";

            if (resources.Any())
            {
                query += @"
                    UNION ALL

                    SELECT * FROM (    
                        SELECT
                            rpt.ReportEntityId,
                            'AdHocReports.ReportFolders.' + REPLACE(REPLACE(fldr.[Path], ' ',''), '\', '.') AS ResourceName,
                            rpt.[Path] AS Name,
                            REPLACE(rpt.Name, '.wrx', '') AS DisplayName,
                            re.Description,
                            re.DateModified,
                            re.ModifiedBy
                        FROM [CoreReports].ReportEntityInfo rpt
                            LEFT JOIN [CoreReports].ReportEntityInfo fldr ON rpt.ParentId = fldr.ReportEntityId
                            INNER JOIN [CoreReports].ReportEntity re ON rpt.ReportEntityId = re.ReportEntityId
                        WHERE
                            re.EntityType = 1
                        AND re.IsInternal = 0
                        AND fldr.[Path] != 'My Reports'
                        AND (rpt.DistrictId = @placeId OR rpt.DistrictId IS NULL)
                        AND (rpt.UserId = @userId OR rpt.UserId IS NULL)
                        AND CAST(re.Content AS xml).value('(/*//custom_value[./id=''Is_Vendor_Extract'' and ./value=''True''])[1]','VARCHAR(25)') IS NOT NULL
                    ) x
                    WHERE x.ResourceName IN (" + resourceNames + @")";
            }

            return query;
        }
    }
}