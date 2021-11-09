namespace ProgressBook.Reporting.Data.Repositories
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using ProgressBook.Reporting.Data.Entities;
    using ProgressBook.Shared.Utilities.Data;

    public class SisReportViewInfoRepository : AdoRepository<ReportViewInfo>
    {
        public SisReportViewInfoRepository()
            : base(new DbCommandExecutor(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString))
        {
        }

        public List<ReportViewInfo> GetSisEntities()
        {
            var query = @"
                SELECT
	                ViewName,
	                SchemaName,
	                [Exago_EntityName] AS EntityName,
	                [Exago_Category] AS Category,
	                [Exago_KeyColumns] AS KeyColumns,
	                [Exago_TenantColumns] AS TenantColumns
                FROM (
	                SELECT
		                vw.name AS ViewName,
		                sch.name AS SchemaName,
		                ep.name AS EpName,
		                ep.value AS EpValue
	                FROM sys.views vw
	                INNER JOIN sys.schemas sch ON vw.schema_id = sch.schema_id
	                INNER JOIN sys.extended_properties ep ON ep.major_id = vw.object_id
                ) x
                PIVOT
                (
	                MIN(EpValue) FOR EpName IN ([Exago_EntityName],[Exago_Category],[Exago_KeyColumns],[Exago_TenantColumns])
                ) pvt
                WHERE
	                [Exago_EntityName] IS NOT NULL";

            return ReadAllRecords(query, CommandType.Text).ToList();
        }
    }
}