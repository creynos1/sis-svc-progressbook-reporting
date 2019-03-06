﻿namespace ProgressBook.Reporting.ExagoScheduler.Common
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;
    using ProgressBook.Reporting.ExagoIntegration;

    public partial class ConfigForm : Form
    {
        private readonly string _filePath;
        private readonly string _serviceName;
        private readonly string _xmlConfigFile;
        private Configuration _exeConfiguration;
        private XmlDocument _xmlConfiguration;

        public ConfigForm(string filePath, string serviceName)
        {
            InitializeComponent();

            _filePath = filePath;
            _xmlConfigFile = Path.Combine(filePath, "eWebReportsScheduler.xml");
            LoadConfiguration();
            _serviceName = serviceName ?? _exeConfiguration.AppSettings.Settings["ServiceName"]?.Value;
            PopulateForm();
        }

        private void LoadConfiguration()
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(Path.Combine(_filePath, "eWebReportsScheduler.exe"));
            _xmlConfiguration = new XmlDocument();
            _xmlConfiguration.Load(_xmlConfigFile);
        }

        private void PopulateForm()
        {
            // populate database settings
            if (_exeConfiguration.ConnectionStrings.ConnectionStrings["StudentInformation"] != null)
            {
                databaseSettings.ConnectionString = _exeConfiguration.ConnectionStrings.ConnectionStrings["StudentInformation"].ConnectionString;
            }

            if (_exeConfiguration.AppSettings.Settings["EnableFtpSessionLog"] != null)
            {
                ftpLoggingSettings.EnableFtpSessionLogging = Convert.ToBoolean(_exeConfiguration.AppSettings.Settings["EnableFtpSessionLog"].Value);
            }

            if (_exeConfiguration.AppSettings.Settings["FtpSessionLogDaysHistory"] != null)
            {
                ftpLoggingSettings.NumberofLogDaysHistoryToMaintain = Convert.ToInt16(_exeConfiguration.AppSettings.Settings["FtpSessionLogDaysHistory"].Value);
            }

            if (_exeConfiguration.AppSettings.Settings["FtpSessionLogPath"] != null)
            {
                ftpLoggingSettings.FtpSessionLogPath = _exeConfiguration.AppSettings.Settings["FtpSessionLogPath"].Value;
            }

            // populate email settings
            emailSettings.ServerName = _xmlConfiguration.SelectSingleNode("//smtp_server").InnerText;
            emailSettings.EnableSsl = bool.Parse(_xmlConfiguration.SelectSingleNode("//smtp_enable_ssl").InnerText);
            emailSettings.UserName = _xmlConfiguration.SelectSingleNode("//smtp_user_id").InnerText;
            emailSettings.Password = _xmlConfiguration.SelectSingleNode("//smtp_password").InnerText;
            emailSettings.FromEmail = _xmlConfiguration.SelectSingleNode("//smtp_from").InnerText;
            emailSettings.FromName = _xmlConfiguration.SelectSingleNode("//smtp_from_name").InnerText;
            emailSettings.EmailAddendum = _xmlConfiguration.SelectSingleNode("//email_addendum").InnerText;

            // populate advanced settings
            advancedSettings.ReportPath = _xmlConfiguration.SelectSingleNode("//report_path").InnerText;
            advancedSettings.ErrorEmail = _xmlConfiguration.SelectSingleNode("//error_report_to").InnerText;
            advancedSettings.DefaultJobTimeout = int.Parse(_xmlConfiguration.SelectSingleNode("//default_job_timeout").InnerText);
            advancedSettings.SleepTime = int.Parse(_xmlConfiguration.SelectSingleNode("//sleep_time").InnerText);
            advancedSettings.SimultaneousJobMax = int.Parse(_xmlConfiguration.SelectSingleNode("//simultaneous_job_max").InnerText);
            advancedSettings.FlushTime = int.Parse(_xmlConfiguration.SelectSingleNode("//flush_time").InnerText);
            advancedSettings.EmailRetryTime = int.Parse(_xmlConfiguration.SelectSingleNode("//email_retry_time").InnerText);
            advancedSettings.StopScheduleOnError = bool.Parse(_xmlConfiguration.SelectSingleNode("//abend_upon_report_error").InnerText);
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dialogResult = MessageBox.Show("Save changes before exiting?",
                "Save Changes?",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation);

            switch (dialogResult)
            {
                case DialogResult.No:
                    e.Cancel = false;
                    return;

                case DialogResult.Cancel:
                    e.Cancel = true;
                    return;

                case DialogResult.Yes:

                    if (!ValidateChildren())
                    {
                        MessageBox.Show("One or more fields are missing required value(s).",
                            "Save Changes?",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        if (!emailSettings.ValidateChildren())
                        {
                            tabs.SelectTab(emailTab);
                        }
                        else if (!databaseSettings.ValidateChildren())
                        {
                            tabs.SelectTab(databaseTab);
                        }
                        else if (!advancedSettings.ValidateChildren())
                        {
                            tabs.SelectTab(advancedTab);
                        }

                        e.Cancel = true;
                        return;
                    }

                    SaveChanges();
                    break;
            }
        }

        private void SaveChanges()
        {
            try
            {
                SaveConfig();

                if (_serviceName != null)
                {
                    ServiceHelper.StopService(_serviceName);
                    SaveServiceAccountSettings();
                    ServiceHelper.StartService(_serviceName);
                }

                MessageBox.Show("Settings updated successfully.", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred while saving settings: {0}", ex.Message),
                    "Save Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SaveConfig()
        {
            // save database settings
            if (_exeConfiguration.ConnectionStrings.ConnectionStrings["StudentInformation"] == null)
            {
                _exeConfiguration.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("StudentInformation", databaseSettings.ConnectionString, "System.Data.SqlClient"));
            }
            else
            {
                _exeConfiguration.ConnectionStrings.ConnectionStrings["StudentInformation"].ConnectionString = databaseSettings.ConnectionString;
                _exeConfiguration.ConnectionStrings.ConnectionStrings["StudentInformation"].ProviderName = "System.Data.SqlClient";
            }

            // save app settings
            if (_exeConfiguration.AppSettings.Settings["ServiceName"] == null)
            {
                _exeConfiguration.AppSettings.Settings.Add("ServiceName", _serviceName);
            }

            if (_exeConfiguration.AppSettings.Settings["EnableFtpSessionLog"] == null)
            {
                _exeConfiguration.AppSettings.Settings.Add("EnableFtpSessionLog", ftpLoggingSettings.EnableFtpSessionLogging.ToString().ToLower());
            }
            else
            {
                _exeConfiguration.AppSettings.Settings["EnableFtpSessionLog"].Value = ftpLoggingSettings.EnableFtpSessionLogging.ToString().ToLower();
            }

            if (_exeConfiguration.AppSettings.Settings["FtpSessionLogDaysHistory"] == null)
            {
                _exeConfiguration.AppSettings.Settings.Add("FtpSessionLogDaysHistory", ftpLoggingSettings.NumberofLogDaysHistoryToMaintain.ToString());
            }
            else
            {
                _exeConfiguration.AppSettings.Settings["FtpSessionLogDaysHistory"].Value = ftpLoggingSettings.NumberofLogDaysHistoryToMaintain.ToString();
            }

            if (_exeConfiguration.AppSettings.Settings["FtpSessionLogPath"] == null)
            {
                _exeConfiguration.AppSettings.Settings.Add("FtpSessionLogPath", ftpLoggingSettings.FtpSessionLogPath);
            }
            else
            {
                _exeConfiguration.AppSettings.Settings["FtpSessionLogPath"].Value = ftpLoggingSettings.FtpSessionLogPath;
            }

            _exeConfiguration.Save();

            // save email settings
            _xmlConfiguration.SelectSingleNode("//smtp_server").InnerText = emailSettings.ServerName;
            _xmlConfiguration.SelectSingleNode("//smtp_enable_ssl").InnerText = emailSettings.EnableSsl.ToString().ToLowerInvariant();
            _xmlConfiguration.SelectSingleNode("//smtp_user_id").InnerText = emailSettings.UserName;
            _xmlConfiguration.SelectSingleNode("//smtp_password").InnerText = emailSettings.Password;
            _xmlConfiguration.SelectSingleNode("//smtp_from").InnerText = emailSettings.FromEmail;
            _xmlConfiguration.SelectSingleNode("//smtp_from_name").InnerText = emailSettings.FromName;
            _xmlConfiguration.SelectSingleNode("//email_addendum").InnerText = emailSettings.EmailAddendum;

            // save advanced settings
            _xmlConfiguration.SelectSingleNode("//report_path").InnerText = advancedSettings.ReportPath;
            _xmlConfiguration.SelectSingleNode("//error_report_to").InnerText = advancedSettings.ErrorEmail;
            _xmlConfiguration.SelectSingleNode("//default_job_timeout").InnerText = advancedSettings.DefaultJobTimeout.ToString();
            _xmlConfiguration.SelectSingleNode("//sleep_time").InnerText = advancedSettings.SleepTime.ToString();
            _xmlConfiguration.SelectSingleNode("//simultaneous_job_max").InnerText = advancedSettings.SimultaneousJobMax.ToString();
            _xmlConfiguration.SelectSingleNode("//flush_time").InnerText = advancedSettings.FlushTime.ToString();
            _xmlConfiguration.SelectSingleNode("//email_retry_time").InnerText = advancedSettings.EmailRetryTime.ToString();
            _xmlConfiguration.SelectSingleNode("//abend_upon_report_error").InnerText = advancedSettings.StopScheduleOnError.ToString().ToLowerInvariant();

            _xmlConfiguration.Save(_xmlConfigFile);
        }

        private void SaveServiceAccountSettings()
        {
            if (!string.IsNullOrEmpty(serviceAccountSettings.ServiceAccount))
            {
                ServiceHelper.ChangeServiceAccount(_serviceName, serviceAccountSettings.ServiceAccount, serviceAccountSettings.ServicePassword);

                if (!LocalSecurityAuthorityWrapper.GetRightsForAccount(serviceAccountSettings.ServiceAccount).Contains(LocalSecurityAuthorityRights.LogonAsService))
                {
                    LocalSecurityAuthorityWrapper.SetRightByAccount(serviceAccountSettings.ServiceAccount, LocalSecurityAuthorityRights.LogonAsService);
                }
            }
        }
    }

    internal class DummyServerPathResolver : IServerPathResolver
    {
        public string MapPath(string path)
        {
            return path;
        }
    }
}