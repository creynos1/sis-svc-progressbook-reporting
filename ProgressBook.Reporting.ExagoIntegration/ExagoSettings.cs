namespace ProgressBook.Reporting.ExagoIntegration
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Xml;

    public interface IExagoSettings
    {
        string ExagoInstallPath { get; }
        string HostApplicationPath { get; }
        string IntegrationPath { get; }
        string ConfigFileName { get; }
        string ConfigFileFullPath { get; }
        string SchedulerConfigFileFullPath { get; }
        int DbRowLimit { get; }
        string SchedulerHost { get; }
        string BaseLineConfigFileName { get; }
        string ServerEventsTypeName { get; }
        string OneRosterTypeName { get; }
        string IntegrationAssembly { get; }
        string ReportPathTypeName { get; }
        string SisDataSourceId { get; }
        string SisReadonlyDataSourceId { get; }
        string GradeBookDataSourceId { get; }
        string SpecialServicesDataSourceId { get; }
        bool EnableFtpSessionLog { get; }
        int FTPSessionLogDaysHistory { get; }
        string FtpSessionLogPath { get; }
    }

    public class ExagoSettings : IExagoSettings
    {
        private readonly IServerPathResolver _serverPathResolver;
        private const int DEFAULT_FTP_SESSION_LOG_DAYS_HISTORY = 10;

        public ExagoSettings(IServerPathResolver serverPathResolver)
        {
            _serverPathResolver = serverPathResolver;
        }

        public string ExagoInstallPath => _serverPathResolver.MapPath("~/Exago");
        public string HostApplicationPath => _serverPathResolver.MapPath("~/");

        public string IntegrationPath => ConfigurationManager.AppSettings["IntegrationPath"]
                                         ?? @"C:\ProgramData\Software Answers\Ad Hoc Reports\Integration";
        public string ConfigFileName { get; } = "WebReports.xml";

        public string ConfigFileFullPath => Path.Combine(ExagoInstallPath, "Config", ConfigFileName);

        public string SchedulerConfigFileFullPath
        {
            get
            {
                var filePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                filePath = filePath.Substring(0, filePath.Length - 32);
                return Path.Combine(filePath, "eWebReportsScheduler.xml");
            }
        }

        public int DbRowLimit
        {
            get
            {
                var setting = ConfigurationManager.AppSettings["DbRowLimit"];

                int value;
                return !int.TryParse(setting, out value) ? 300000 : value;
            }
        }

        public string SchedulerHost { get; } = ConfigurationManager.AppSettings["SchedulerHost"];

        public string BaseLineConfigFileName { get; } =
            "ProgressBook.Reporting.ExagoIntegration.WebReportsBaselineConfig.xml";

        public string ServerEventsTypeName { get; } = typeof(ServerEvents).FullName;

        public string OneRosterTypeName { get; } = typeof(OneRosterExtractor).FullName;

        public string IntegrationAssembly => Path.Combine(IntegrationPath,
                                                          typeof(ExagoSettings).Assembly.GetName().Name + ".dll");

        public string ReportPathTypeName { get; } = typeof(ReportFolderManagement).FullName;
        public string SisDataSourceId { get; } = "0";
        public string SisReadonlyDataSourceId { get; } = "2";
        public string GradeBookDataSourceId { get; } = "3";
        public string SpecialServicesDataSourceId { get; } = "6";

        public bool EnableFtpSessionLog
        {
            get
            {
                var xmlConfiguration = new XmlDocument();
                xmlConfiguration.Load(SchedulerConfigFileFullPath);

                var setting = xmlConfiguration.SelectSingleNode("//enable_ftp_session_logging").InnerText;

                bool value;
                return !bool.TryParse(setting, out value) ? false : value;
            }
        }

        public int FTPSessionLogDaysHistory
        {
            get
            {
                var xmlConfiguration = new XmlDocument();
                xmlConfiguration.Load(SchedulerConfigFileFullPath);

                var setting = xmlConfiguration.SelectSingleNode("//number_of_log_days_history_to_maintain").InnerText;

                int value;
                return !int.TryParse(setting, out value) ? DEFAULT_FTP_SESSION_LOG_DAYS_HISTORY : value;
            }
        }

        public string FtpSessionLogPath
        {
            get
            {
                var xmlConfiguration = new XmlDocument();
                xmlConfiguration.Load(SchedulerConfigFileFullPath);

                return xmlConfiguration.SelectSingleNode("//ftp_session_log_path").InnerText;
            }
        } 
    }
}