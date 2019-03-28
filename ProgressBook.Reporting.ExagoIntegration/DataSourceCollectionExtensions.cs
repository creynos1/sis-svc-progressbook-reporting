namespace ProgressBook.Reporting.ExagoIntegration
{
    using WebReports.Api.Data;

    public static class DataSourceCollectionExtensions
    {
        public static void EnsureDataSourceExists(this DataSourceCollection dataSourceCollection,
                                                  string dataSourceName,
                                                  string dbType,
                                                  string connectionString)
        {
            var dataSource = dataSourceCollection.GetDataSource(dataSourceName);

            if (dataSource == null)
            {
                dataSource = new DataSource(null)
                             {
                                 DbType = dbType,
                                 Name = dataSourceName
                             };
                dataSourceCollection.Add(dataSource);
            }

            dataSource.DataConnStr = connectionString;
        }
    }
}