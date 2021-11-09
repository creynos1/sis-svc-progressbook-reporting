namespace ProgressBook.Reporting.Data.Repositories
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using ProgressBook.Reporting.Data.Entities;
    using ProgressBook.Shared.Utilities.Data;

    public class SisReportColumnMetaDataRepository : AdoRepository<ReportColumnMetaData>
    {
        public SisReportColumnMetaDataRepository()
            : base(new DbCommandExecutor(ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString))
        {
        }

        // Get View Columns and Types, convert to to Exago Types
        public Dictionary<string, List<KeyValuePair<string, string>>> GetSisEntityMetadata()
        {
            var query = @"
                SELECT IC.TABLE_NAME as ViewName, IC.TABLE_SCHEMA as SchemaName, IC.COLUMN_NAME as ColumnName, DATA_TYPE as DataType
                FROM INFORMATION_SCHEMA.COLUMNS IC
                inner join INFORMATION_SCHEMA.TABLES IT on IC.TABLE_SCHEMA = IT.TABLE_SCHEMA and IC.TABLE_NAME = IT.TABLE_NAME AND IT.TABLE_TYPE = 'VIEW'
                WHERE IC.TABLE_SCHEMA = 'CoreReports'";
            var list = ReadAllRecords(query, CommandType.Text);
            Dictionary<string, List<KeyValuePair<string, string>>> items = new Dictionary<string, List<KeyValuePair<string, string>>>();
            foreach (var row in list) {
                if (!items.ContainsKey(row.ViewName.ToLower())) {
                    items[row.ViewName.ToLower()] = new List<KeyValuePair<string, string>>();
                }
                items[row.ViewName.ToLower()].Add(new KeyValuePair<string, string>(row.ColumnName, Utils.ExagoDataTypeConvert(row.ColumnName, row.DataType)));
            }
            return items;
        }        
    }
}