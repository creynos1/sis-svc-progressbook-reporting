namespace ProgressBook.Reporting.ExagoIntegration
{
    using System.Configuration;

    public interface IConnectionStrings
    {
        string StudentInformation { get; }
        string DistrictTemplate { get; }
        string QuickReports { get; }
        string PbMaster { get; }
        string IntegrationAssembly { get; }
    }

    public class ConnectionStrings : IConnectionStrings
    {
        private readonly IExagoSettings _exagoSettings;

        public ConnectionStrings(IExagoSettings exagoSettings)
        {
            _exagoSettings = exagoSettings;
        }

        public string StudentInformation { get; } =
            ConfigurationManager.ConnectionStrings["StudentInformation"]?.ConnectionString;

        public string DistrictTemplate { get; } =
            ConfigurationManager.ConnectionStrings["DistrictTemplate"]?.ConnectionString;

        public string QuickReports { get; } =
            ConfigurationManager.ConnectionStrings["QuickReports"]?.ConnectionString;

        public string PbMaster { get; } = ConfigurationManager.ConnectionStrings["PbMasterContext"]?.ConnectionString;

        public string IntegrationAssembly =>
            $@"assembly={_exagoSettings.IntegrationAssembly};class={_exagoSettings.ServerEventsTypeName}";
    }
}