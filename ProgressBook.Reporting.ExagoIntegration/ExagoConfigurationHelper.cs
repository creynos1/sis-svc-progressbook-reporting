namespace ProgressBook.Reporting.ExagoIntegration
{
    using System.IO;
    using System.Xml;
    using ProgressBook.Master.Connections;
    using ProgressBook.Reporting.Data;
    using WebReports.Api;
    using WebReports.Api.Common;

    public interface IExagoConfigurationHelper
    {
        void Configure();
    }

    public class ExagoConfigurationHelper : IExagoConfigurationHelper
    {
        private readonly IConnectionStrings _connectionStrings;
        private readonly IExagoConfigurationFileFactory _exagoConfigurationFileFactory;
        private readonly IExagoSettings _exagoSettings;

        public ExagoConfigurationHelper(IExagoSettings exagoSettings,
                                        IExagoConfigurationFileFactory exagoConfigurationFileFactory,
                                        IConnectionStrings connectionStrings)
        {
            _exagoSettings = exagoSettings;
            _exagoConfigurationFileFactory = exagoConfigurationFileFactory;
            _connectionStrings = connectionStrings;
        }

        public void Configure()
        {
            _exagoConfigurationFileFactory.WriteConfigurationFile(_exagoSettings.ConfigFileFullPath);

#if DEBUG
            //DeployIntegrationFiles();
#endif

            var api = new Api(_exagoSettings.ExagoInstallPath, _exagoSettings.ConfigFileName);
            ConfigureDataSources(api);
            ConfigureMainSettings(api);
            ConfigureCustomOptions(api);
            api.SaveData(true);
            File.Delete(_exagoSettings.ConfigFileFullPath);
        }      

        private void DeployIntegrationFiles()
        {
            string[] files =
            {
                typeof(ExagoConfigurationHelper).Assembly.GetName().Name + ".dll",
                typeof(ReportEntityDbContext).Assembly.GetName().Name + ".dll",
                "EntityFramework.dll",
                "EntityFramework.SqlServer.dll"
            };

            foreach (var filename in files)
            {
                var srcFolder = Path.Combine(_exagoSettings.HostApplicationPath, "bin");
                var dstFolder = _exagoSettings.IntegrationPath;

                File.Copy(
                    Path.Combine(srcFolder, filename),
                    Path.Combine(dstFolder, filename),
                    true);
            }
        }

        private void ConfigureDataSources(Api api)
        {
            api.DataSources.EnsureDataSourceExists("StudentInformation",
                                                   "mssql",
                                                   _connectionStrings.StudentInformation);

            if (!_exagoSettings.DisableGradeBookIntegration)
            {
                api.DataSources.EnsureDataSourceExists("DistrictDatabase",
                                                       "mssql",
                                                       _connectionStrings.DistrictTemplate);

                using (var districtProfileService = new DistrictProfileService(_connectionStrings.PbMaster))
                {
                    foreach (var districtProfile in districtProfileService.GetAllDistricts())
                    {
                        api.DataSources.EnsureDataSourceExists($"District_{districtProfile.Irn}",
                                                               "mssql",
                                                               districtProfile.ConnectionString);
                    }
                }
            }

            var dataSource = api.DataSources.GetDataSource("QuickReports");

            if (dataSource != null && _connectionStrings.QuickReports != null)
            {
                dataSource.DataConnStr = _connectionStrings.QuickReports;
            }

            api.DataSources.EnsureDataSourceExists("IntegrationAssembly",
                                                   "assembly",
                                                   _connectionStrings.IntegrationAssembly);
        }

        private void ConfigureMainSettings(Api api)
        {
            api.General.ReportPath =
                $@"assembly={_exagoSettings.IntegrationAssembly};class={_exagoSettings
                    .ReportPathTypeName}";

            api.General.ScheduleRemotingHost = _exagoSettings.SchedulerHost;
            api.General.DbRowLimit = _exagoSettings.DbRowLimit;
        }

        private void ConfigureCustomOptions(Api api)
        {
            var vendorOption = api.SetupData.CustomOptions.GetCustomOption("Vendor");
            if (vendorOption == null)
            {
                return;
            }
            vendorOption.ListItems.Clear();
            vendorOption.ListItems.Add(new CustomOptionListItem());
        }
    }
}