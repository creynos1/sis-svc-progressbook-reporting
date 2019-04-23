﻿
using System.Drawing;

namespace ProgressBook.Reporting.ExagoScheduler.Setup
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml;
    using ProgressBook.Reporting.Data;
    using ProgressBook.Reporting.ExagoIntegration;
    using ProgressBook.Reporting.ExagoScheduler.Common;

    internal static class Program
    {
        private static readonly string ResourcesNamespace = $"{typeof(Program).Assembly.GetName().Name}.Resources";
        private const string XML_CONFIG_FILE_NAME = "eWebReportsScheduler.xml";
        private const string CONFIG_EXE_FILE_NAME = "ProgressBook.Reporting.ExagoScheduler.Config.exe";
        private const string DEFAULT_SMTP_FROM_NAME = "ProgressBook Ad Hoc Reports Scheduler";
        private const string ENABLE_FTP_SESSION_LOGGING = "false";
        private const string NUMBER_OF_LOG_DAYS_HISTORY_TO_MAINTAIN = "10";
        private const string FTP_SESSION_LOG_PATH = @"C:\temp\logs";
        private const string DEFAULT_LOGGING = "on";
        private const string DEFAULT_FLUSH_TIME = "0";

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ResolveEmbeddedAssemblies();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RunSetup();
        }

        private static void RunSetup()
        {
#if DEBUG
            var filePath = "C:\\Program Files\\Exago\\ExagoScheduler\\";
            var serviceName = "eWebReportsScheduler_ExagoScheduler";
            var startupLocation = Point.Empty;
            var startupPoistion = FormStartPosition.CenterScreen;
#else
            var setupForm = new CompleteSetup.ExagoSchedulerSetup();
            Application.Run(setupForm);

            var fieldInfo = setupForm.GetType().GetField("filePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var filePath = fieldInfo.GetValue(setupForm) as string;

            if (filePath == null)
                return;

            fieldInfo = setupForm.GetType().GetField("serviceName", BindingFlags.Instance | BindingFlags.NonPublic);
            var serviceName = fieldInfo.GetValue(setupForm) as string;

            var startupLocation = setupForm.Location;
            var startupPoistion = FormStartPosition.Manual;
#endif
            try
            {
                var xmlConfigFilePath = Path.Combine(filePath, XML_CONFIG_FILE_NAME);
                if (!File.Exists(xmlConfigFilePath))
                {
                    throw new FileNotFoundException(string.Format("Configuration file not found:\n{0}", xmlConfigFilePath));
                }

                DeployIntegrationAssemblies();
                DeployConfigExe(filePath);
                SetDefaultValues(xmlConfigFilePath);

                Application.Run(new ConfigForm(filePath, serviceName)
                {
                    Location = startupLocation,
                    StartPosition = startupPoistion,
                    Icon = Properties.Resources.GearIcon
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private static void DeployIntegrationAssemblies()
        {
            var integrationPath = @"C:\ProgramData\Software Answers\Ad Hoc Reports\Integration";

            if (!Directory.Exists(integrationPath))
            {
                Directory.CreateDirectory(integrationPath);
            }

            var assemblyNames = new[]
                                {
                                    "EntityFramework.dll",
                                    "EntityFramework.SqlServer.dll",
                                    typeof(ExagoConfigurationHelper).Assembly.GetName().Name + ".dll",
                                    typeof(ReportEntityDbContext).Assembly.GetName().Name + ".dll",
                                    "ProgressBook.Reporting.Client.dll",
                                    "ProgressBook.Reporting.SharedModels.dll",
                                    "ProgressBook.Shared.Utilities.Data.dll",
                                    "ProgressBook.Shared.Utilities.dll",
                                    "WinSCP.exe",
                                    "WinSCPnet.dll"
                                };

            foreach (var assemblyName in assemblyNames)
            {
                var dll = GetEmbeddedAssembly(assemblyName);
                File.WriteAllBytes(Path.Combine(integrationPath, assemblyName), dll);
            }
        }

        private static void DeployConfigExe(string filePath)
        {
            var exe = GetEmbeddedAssembly(CONFIG_EXE_FILE_NAME);
            File.WriteAllBytes(Path.Combine(filePath, CONFIG_EXE_FILE_NAME), exe);
        }

        private static void SetDefaultValues(string configFile)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(configFile);
            xml.SelectSingleNode("//smtp_from_name").InnerText = DEFAULT_SMTP_FROM_NAME;
            xml.SelectSingleNode("//logging").InnerText = DEFAULT_LOGGING;
            xml.SelectSingleNode("//flush_time").InnerText = DEFAULT_FLUSH_TIME;

            xml = AddElement(xml, "enable_ftp_session_logging", ENABLE_FTP_SESSION_LOGGING);
            xml = AddElement(xml, "number_of_log_days_history_to_maintain", NUMBER_OF_LOG_DAYS_HISTORY_TO_MAINTAIN);
            xml = AddElement(xml, "ftp_session_log_path", FTP_SESSION_LOG_PATH);

            xml.Save(configFile);
        }

        private static XmlDocument AddElement(XmlDocument xml, string elementName, string val)
        {
            var rootNode = xml.SelectSingleNode("//eWebReportScheduler");

            if (xml.SelectSingleNode("//" + elementName) == null)
            {
                var newElement = xml.CreateElement(elementName);
                newElement.InnerText = val;
                rootNode.AppendChild(newElement);
            }

            return xml;
        }

        private static void ResolveEmbeddedAssemblies()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var resourceName = string.Format("{0}.{1}.dll", ResourcesNamespace, new AssemblyName(args.Name).Name);
                
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    var assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }

        private static byte[] GetEmbeddedAssembly(string name)
        {
            var resourceName = string.Format("{0}.{1}", ResourcesNamespace, name);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                var assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);

                return assemblyData;
            }
        }
    }
}