using WebReports.Api.Reports;
using WebReports.Api.Scheduler;

namespace ProgressBook.Reporting.ExagoIntegration
{
    using System;
    using System.IO;
    using System.Linq;
    using ProgressBook.Reporting.Client;
    using ProgressBook.Reporting.Data;
    using ProgressBook.Reporting.Data.Entities;
    using ProgressBook.Reporting.Data.Repositories;
    using ProgressBook.Reporting.ExagoIntegration.VendorExtract;
    using WebReports.Api.Common;
    using WinSCP;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using WebReports.Api;

    public class ServerEvents
    {
        private const int AdHocReportReportId = 900;
        private const byte JobTypeReport = 1;
        private const byte JobDeliveryPickup = 1;
        private const byte JobStatusComplete = 6;
        private const byte JobStatusError = 2;
        private const string NoDataStatusMsg = "No data qualified for report";
        private const string FTPErrorMessage = "An error has occured while trying to FTP this report";

        public static string OnReportExecuteStart(SessionInfo sessionInfo)
        {
            try
            {
                TrackManualReportExecution(sessionInfo);
                ChangeDataSourceConnectionString(sessionInfo, "QuickReports", "StudentInformation");
                ChangeDistrictDataSource(sessionInfo);
            }
            catch (Exception ex)
            {
                sessionInfo.WriteLog(string.Format("OnReportExecuteStart Error. {0}", ex.ToString()));
            }

            return null;
        }

        public static void RemoveBlankFilters(SessionInfo sessionInfo)
        {
            sessionInfo.Report.ExecFilters.RemoveAll(x => x.Value.Length == 0);
        }

        public static string OnScheduledReportComplete(SessionInfo sessionInfo, WebReports.Api.Scheduler.SchedulerJob schedulerJob)
        {
            try
            {
                if (sessionInfo.Report.ExecuteDataRowCount == 0)
                {
                    WriteErrorFile(sessionInfo, NoDataStatusMsg);
                }
            }
            catch (Exception ex)
            {
                sessionInfo.WriteLog(string.Format("OnScheduledReportComplete Error. {0}", ex.ToString()));
                WriteErrorFile(sessionInfo, sessionInfo.ReportSchedulerService.SchedulerJob.ExecuteResult.ToString());
            }

            return null;
        }

        private static void TrackManualReportExecution(SessionInfo sessionInfo)
        {
            if (sessionInfo.PageInfo.ReportSchedulerService != null)
            {
                return;
            }
            using (var attributeService = UserReportAttributeServiceFactory.Instance.Create())
            {
                var userId = Guid.Parse(sessionInfo.GetParameter("UserId").Value);
                var districtId = Guid.Parse(sessionInfo.GetParameter("DistrictId").Value);
                var reportEntityId = attributeService.GetReportEntityIdSync(sessionInfo.Report.Name, userId, districtId);

                attributeService.TrackReportExecutionSync(reportEntityId, userId, districtId);
            }
        }

        private static void ChangeDistrictDataSource(SessionInfo sessionInfo)
        {
            var irn = sessionInfo.SetupData.Parameters.GetParameter("DistrictIrn");

            if (string.IsNullOrEmpty(irn?.Value))
            {
                return;
            }

            ChangeDataSourceConnectionString(sessionInfo, $"District_{irn.Value}", "DistrictDatabase");
        }

        private static void ChangeDataSourceConnectionString(SessionInfo sessionInfo,
                                                             string sourceName,
                                                             string destinationName)
        {
            var sourceDataSource = sessionInfo.SetupData.DataSources.GetDataSource(sourceName);

            if (string.IsNullOrEmpty(sourceDataSource?.DataConnStr))
            {
                return;
            }

            var destinationDataSource = sessionInfo.SetupData.DataSources.GetDataSource(destinationName);

            if (destinationDataSource != null)
            {
                destinationDataSource.DataConnStr = sourceDataSource.DataConnStr;
            }
        }

        public static bool OnScheduledReportExecuteSuccess(SessionInfo sessionInfo)
        {
            try
            {
                var vendorExtractCustomOption = sessionInfo.Report.CustomOptionValues.GetCustomOptionValue("Is_Vendor_Extract");
                if (vendorExtractCustomOption == null)
                {
                    return false;
                }
                if (!bool.Parse(vendorExtractCustomOption.Value))
                {
                    return false;
                }

                var status = FTPOutputFile(sessionInfo);

                if (status)
                {
                    switch (sessionInfo.ReportSchedulerService.SchedulerJob.ExecuteResult)
                    {
                        case wrExecuteReturnValue.Success:
                            CopyFileToRepository(sessionInfo);
                            break;
                        case wrExecuteReturnValue.NothingQualified:
                            WriteErrorFile(sessionInfo, NoDataStatusMsg);
                            break;
                        default:
                            WriteErrorFile(sessionInfo, sessionInfo.ReportSchedulerService.SchedulerJob.ExecuteResult.ToString());
                            break;
                    }

                }
                else
                {
                    WriteErrorFile(sessionInfo, FTPErrorMessage);
                }
            }
            catch (Exception ex)
            {
                sessionInfo.WriteLog(string.Format("OnScheduledReportExecuteSuccess Error. {0}", ex.ToString()));
                WriteErrorFile(sessionInfo, FTPErrorMessage);
            }

            return false;
        }

        private static bool FTPOutputFile(SessionInfo sessionInfo)
        {
            var user = GetUserFromTenantParameter(sessionInfo.UserId);
            var dbContext = ReportEntityDbContextFactory.Create(user.DistrictId, user.UserId);
            var reportEntity = new ReportEntityManager(dbContext).GetByPath<Report>(sessionInfo.Report.Name);

            var mgr = new VendorExtractManager(user.UserId, user.DistrictId);
            var ftpInfo = mgr.GetDistrictVendorFtpInfo(reportEntity.Id);

            if (ftpInfo == null)
            {
                sessionInfo.WriteLog(string.Format("Cannot FTP the file - No valid Vendor Extracts configured for DistrictId({0}), UserID({1}), and ReportEntitiyId({2}).", user.DistrictId, user.UserId, reportEntity.Id));

                return false;
            }

            switch (ftpInfo.FtpConnectionInfo.ProtocolType)
            {
                case ProtocolType.Sftp:
                    SendFileViaSftp(ftpInfo, sessionInfo.Report.DownloadFn, reportEntity.Name, sessionInfo);
                    break;
                case ProtocolType.Ftp:
                case ProtocolType.FtpExplicitTls:
                case ProtocolType.FtpImplicitTls:
                    SendFileViaFtp(ftpInfo, sessionInfo.Report.DownloadFn, reportEntity.Name, sessionInfo);
                    break;
                default:
                    sessionInfo.WriteLog(string.Format("Cannot FTP the file - Unsupported protocol type."));
                    return false;
            }

            return true;
        }

        private static void UpdateJobScheduleTable(SessionInfo sessionInfo, string fileName, bool isFTPError = false)
        {
            var job = sessionInfo.ReportSchedulerService.SchedulerJob;
            var jobStatusId = isFTPError ? JobStatusError : MapExagoReturnValueToJobStatus(job.ExecuteResult);
            var user = GetUserFromTenantParameter(sessionInfo.UserId);
            var jobScheduling = new JobScheduling
            {
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
                JobDeliveryId = JobDeliveryPickup,
                JobTypeId = JobTypeReport,
                JobName = job.JobInfo.Name,
                JobStatusId = jobStatusId,
                UserId = user.UserId,
                SchoolId = user.DistrictId,
                Parameters = string.Empty,
                JobDescription = "Scheduled Ad Hoc Report",
                Results = new List<JobSchedulingResult>
                    {
                        new JobSchedulingResult
                        {
                            Filename = Path.GetFileName(fileName),
                            FileSize = (int) GetFileSize(fileName),
                            IsRetained = false,
                            ReportId = AdHocReportReportId
                        }
                }
            };

            var dbContext = new JobSchedulingDbContext("StudentInformation");
            dbContext.JobSchedulings.Add(jobScheduling);
            dbContext.SaveChanges();
        }

        private static void CopyFileToRepository(SessionInfo sessionInfo)
        {
            string output_directory = GetOutputRepository();
            string newFilenameTemplate = "AdHocReport_{0}_{1}{2}";
            var scheduler_job = sessionInfo.ReportSchedulerService.SchedulerJob;
            var execution = WebReports.Api.Execute.ExecutionManager.GetExecution(sessionInfo.PageInfo, String.Format(@"{0}\working\temp", Environment.CurrentDirectory), output_directory, scheduler_job.JobId);
            string outputFilePath = Path.Combine(String.Format(@"{0}\working\export\", Environment.CurrentDirectory), execution.DownloadName);
            var reportUniqueId = Path.GetFileName(execution.DownloadName).Split('_')[0];
            var fileExtension = Path.GetExtension(execution.DownloadName);
            var newFilename = string.Format(newFilenameTemplate, reportUniqueId, DateTime.Now.Ticks, fileExtension);
            string newFilePath = Path.Combine(output_directory, newFilename);
            File.Copy(outputFilePath, newFilePath, true);
            UpdateJobScheduleTable(sessionInfo, newFilePath);
        }

        private static string GetOutputRepository()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Environment.CurrentDirectory, "eWebReportsScheduler.xml"));

            return doc.SelectSingleNode("//report_path").InnerText;
        }

        private static void SendFileViaSftp(DistrictVendorFtpInfo info, string originFileName, string reportName, SessionInfo sessionInfo)
        {
            var sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = info.FtpConnectionInfo.Host,
                UserName = info.FtpConnectionInfo.Username,
                Password = info.FtpConnectionInfo.Password,
                PortNumber = info.FtpConnectionInfo.Port
            };

            var sessionKey = $"{info.FtpConnectionInfo.Host}:{info.FtpConnectionInfo.Port}";
            var knownHostsMgr = new SshKnownHostsManager();
            var knownHost = knownHostsMgr.SshKnownHosts.FirstOrDefault(x => x.Host == sessionKey);
            if (knownHost != null)
            {
                sessionOptions.SshHostKeyFingerprint = knownHost.Fingerprint;
            }

            var destFileName = BuildFileName(info, originFileName, reportName);

            using (var session = new Session())
            {
                IExagoSettings exagoSettings = new ExagoSettings(new ServerPathResolver());

                if (exagoSettings.EnableFtpSessionLog)
                {
                    CleanupFiles(exagoSettings);
                    session.SessionLogPath = GetFileName(exagoSettings);
                }

                if (string.IsNullOrEmpty(sessionOptions.SshHostKeyFingerprint))
                {
                    var fingerprint = session.ScanFingerprint(sessionOptions);
                    sessionOptions.SshHostKeyFingerprint = fingerprint;
                    knownHostsMgr.SshKnownHosts.Add(new SshKnownHost { Host = sessionKey, Fingerprint = fingerprint });
                    knownHostsMgr.Save();
                }

                session.Open(sessionOptions);


                if (!string.IsNullOrEmpty(info.FtpConnectionInfo.RemoteDirectory) &&
                    !session.FileExists(info.FtpConnectionInfo.RemoteDirectory))
                {
                    session.CreateDirectory(info.FtpConnectionInfo.RemoteDirectory);
                }

                var result = session.PutFiles(originFileName, destFileName);
                result.Check();
            }
        }

        private static void CleanupFiles(IExagoSettings exagoSettings)
        {
            var directoryInfo = new DirectoryInfo(exagoSettings.FtpSessionLogPath);
            var files = directoryInfo.GetFiles("ftp-session*.log").Where(p => p.CreationTime < DateTime.Now.AddDays(-exagoSettings.FTPSessionLogDaysHistory));
            foreach (var file in files)
            {
                file.Delete();
            }
        }

        private static string GetFileName(IExagoSettings exagoSettings)
        {
            return string.Format("{0}\\ftp-session-{1}-{2}-{3}.log", exagoSettings.FtpSessionLogPath, DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("yyyy"));
        }

        private static void SendFileViaFtp(DistrictVendorFtpInfo info, string originFileName, string reportName, SessionInfo sessionInfo)
        {
            var sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = info.FtpConnectionInfo.Host,
                UserName = info.FtpConnectionInfo.Username,
                Password = info.FtpConnectionInfo.Password,
                PortNumber = info.FtpConnectionInfo.Port
            };

            if (info.FtpConnectionInfo.ProtocolType == ProtocolType.FtpExplicitTls)
            {
                sessionOptions.FtpSecure = FtpSecure.Explicit;
            }

            if (info.FtpConnectionInfo.ProtocolType == ProtocolType.FtpImplicitTls)
            {
                sessionOptions.FtpSecure = FtpSecure.Implicit;
            }

            var destFileName = BuildFileName(info, originFileName, reportName);

            using (var session = new Session())
            {
                IExagoSettings exagoSettings = new ExagoSettings(new ServerPathResolver());
                if (exagoSettings.EnableFtpSessionLog)
                {
                    CleanupFiles(exagoSettings);
                    session.SessionLogPath = GetFileName(exagoSettings);
                }

                // for explicit or implicit tls ftp, server needs to already trust remote server's certificate
                session.Open(sessionOptions);

                if (!string.IsNullOrEmpty(info.FtpConnectionInfo.RemoteDirectory) &&
                    !session.FileExists(info.FtpConnectionInfo.RemoteDirectory))
                {
                    session.CreateDirectory(info.FtpConnectionInfo.RemoteDirectory);
                }

                var result = session.PutFiles(originFileName, destFileName);
                result.Check();
            }
        }

        private static string BuildFileName(DistrictVendorFtpInfo info, string fileToSend, string reportName)
        {
            var destFile = "";
            if (!string.IsNullOrEmpty(info.FtpConnectionInfo.RemoteDirectory))
            {
                destFile += info.FtpConnectionInfo.RemoteDirectory.TrimEnd('/') + '/';
            }

            if (!string.IsNullOrEmpty(info.FtpConnectionInfo.OutputFileName))
            {
                destFile += info.FtpConnectionInfo.OutputFileName;
            }
            else
            {
                destFile += reportName.Replace(" ", "") + "_" + Path.GetFileName(fileToSend).Replace("_Download", "");
            }

            return destFile;
        }

        private static byte MapExagoReturnValueToJobStatus(wrExecuteReturnValue result)
        {
            byte jobStatus;

            switch (result)
            {
                case wrExecuteReturnValue.Success:
                    jobStatus = JobStatusComplete;
                    break;
                case wrExecuteReturnValue.NothingQualified:
                    jobStatus = JobStatusError;
                    break;
                default:
                    jobStatus = JobStatusError;
                    break;
            }

            return jobStatus;
        }

        private static AdHocReportUser GetUserFromTenantParameter(string value)
        {
            var values = value.Split('_');
            return new AdHocReportUser { UserId = new Guid(values[0]), DistrictId = new Guid(values[1]) };
        }

        private static string GetReportPath()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Environment.CurrentDirectory, "eWebReportsScheduler.xml"));

            return doc.SelectSingleNode("//report_path").InnerText;
        }

        private static long GetFileSize(string fileName)
        {
            return new FileInfo(fileName).Length;
        }

        private static void WriteErrorFile(SessionInfo sessionInfo, string statusMsg)
        {
            var fileName = Path.Combine(GetOutputRepository(), string.Format("AdHocReport_NoData_{0}.html", Guid.NewGuid()));
            string reportName = sessionInfo.ReportSchedulerService.SchedulerJob.ReportName;
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendFormat("<title>Ad Hoc Report - {0}</title>\r\n", reportName);
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine(
                "body { font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif; font-size: 14px; color: #333333; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendFormat("<h3>Ad Hoc Report - {0}</h3>\r\n", reportName);
            sb.AppendFormat("<p>{0}</p>", statusMsg);
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            File.WriteAllText(fileName, sb.ToString());
            bool isFTPError = (statusMsg == FTPErrorMessage ? true : false);
            UpdateJobScheduleTable(sessionInfo, fileName, isFTPError);
        }



        private class AdHocReportUser
        {
            public Guid UserId { get; set; }
            public Guid DistrictId { get; set; }
        }
    }
}