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

        public static string OnReportExecuteStart(SessionInfo sessionInfo)
        {
            TrackManualReportExecution(sessionInfo);
            ChangeDataSourceConnectionString(sessionInfo, "QuickReports", "StudentInformation");
            ChangeDistrictDataSource(sessionInfo);
            return null;
        }

        public static string OnScheduledReportComplete(SessionInfo sessionInfo, WebReports.Api.Scheduler.SchedulerJob schedulerJob)
        {
            if (sessionInfo.Report.ExecuteDataRowCount == 0)
            {
                WriteErrorFile(sessionInfo.ReportSchedulerService.SchedulerJob.ReportName, NoDataStatusMsg);
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
                string fileName;
                switch (sessionInfo.ReportSchedulerService.SchedulerJob.ExecuteResult)
                {
                    case wrExecuteReturnValue.Success:
                        fileName = MoveFileToRepository(sessionInfo);
                        break;
                    case wrExecuteReturnValue.NothingQualified:
                        fileName = WriteErrorFile(sessionInfo.ReportSchedulerService.SchedulerJob.ReportName, NoDataStatusMsg);
                        break;
                    default:
                        fileName = WriteErrorFile(sessionInfo.ReportSchedulerService.SchedulerJob.ReportName, sessionInfo.ReportSchedulerService.SchedulerJob.ExecuteResult.ToString());
                        break;
                }

                UpdateJobScheduleTable(sessionInfo, fileName);
            }

            return true;
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
                    return false;
            }

            return true;
        }

        private static void UpdateJobScheduleTable(SessionInfo sessionInfo, string fileName)
        {
            var job = sessionInfo.ReportSchedulerService.SchedulerJob;

            var user = GetUserFromTenantParameter(sessionInfo.UserId);
            var jobScheduling = new JobScheduling
            {
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
                JobDeliveryId = JobDeliveryPickup,
                JobTypeId = JobTypeReport,
                JobName = job.JobInfo.Name,
                JobStatusId = MapExagoReturnValueToJobStatus(job.ExecuteResult),
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

            try
            {
                var dbContext = new JobSchedulingDbContext("StudentInformation");
                dbContext.JobSchedulings.Add(jobScheduling);
                dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.Message.Contains("See the inner exception for details"))
                {
                    sessionInfo.WriteLog(string.Format("Encountered exception while trying to save JobScheduling data: {0}", ex.InnerException.Message));
                    throw ex.InnerException;
                }

                throw;
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("The following entity validation errors occured:");
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityError.ValidationErrors)
                    {
                        sb.AppendFormat("{0}.{1}: {2}\r\n",
                                        entityError.Entry.Entity,
                                        validationError.PropertyName,
                                        validationError.ErrorMessage);
                    }
                }

                sessionInfo.WriteLog(string.Format("Encountered exception while trying to save JobScheduling data: {0}", sb.ToString()));
                throw new Exception(sb.ToString());
            }
        }

        private static string MoveFileToRepository(SessionInfo sessionInfo)
        {
            string output_directory = GetOutputRepository();
            string newFilenameTemplate = "AdHocReport_{0}_{1}{2}";

            var scheduler_job = sessionInfo.ReportSchedulerService.SchedulerJob;
            var execution = WebReports.Api.Execute.ExecutionManager.GetExecution(sessionInfo.PageInfo, String.Format(@"{0}\working\temp", Environment.CurrentDirectory), output_directory, scheduler_job.JobId);
            var file_info = new FileInfo(execution.DownloadFn);
            var original_extension = file_info.Extension;

            string outputFilePath = Path.Combine(String.Format(@"{0}\working\export\", Environment.CurrentDirectory), execution.DownloadName);
            var reportUniqueId = Path.GetFileName(execution.DownloadName).Split('_')[0];
            var fileExtension = Path.GetExtension(execution.DownloadName);
            var newFilename = string.Format(newFilenameTemplate, reportUniqueId, DateTime.Now.Ticks, fileExtension);
            string newFilePath = Path.Combine(output_directory, newFilename);
            if(File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }
            File.Move(outputFilePath, newFilePath);
            return newFilePath;
        }

        private static string GetOutputRepository()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Environment.CurrentDirectory, "eWebReportsScheduler.xml"));

            return doc.SelectSingleNode("//report_path").InnerText;
        }

        private static void SendFileViaSftp(DistrictVendorFtpInfo info, string filename, string reportName, SessionInfo sessionInfo)
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
                    knownHostsMgr.SshKnownHosts.Add(new SshKnownHost {Host = sessionKey, Fingerprint = fingerprint});
                    knownHostsMgr.Save();
                }

                session.Open(sessionOptions);


                if (!string.IsNullOrEmpty(info.FtpConnectionInfo.RemoteDirectory) &&
                    !session.FileExists(info.FtpConnectionInfo.RemoteDirectory))
                {
                    session.CreateDirectory(info.FtpConnectionInfo.RemoteDirectory);
                }

                var destFileName = BuildFileName(info, filename, reportName);
                var result = session.PutFiles(filename, destFileName);
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

        private static void SendFileViaFtp(DistrictVendorFtpInfo info, string filename, string reportName, SessionInfo sessionInfo)
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

                var destFileName = BuildFileName(info, filename, reportName);
                var result = session.PutFiles(filename, destFileName);
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

        public static string RemoveBlankFilters(SessionInfo sessionInfo)
        {
            ReportFilterCollection filters = sessionInfo.Report.ExecFilters;
            filters.RemoveAll(x => x.Value.Length == 0);

            // continue execution
            return null;
        }

        private static byte MapExagoReturnValueToJobStatus(wrExecuteReturnValue result)
        {
            byte jobStatus;

            switch(result)
            {
                case wrExecuteReturnValue.Success:
                case wrExecuteReturnValue.NothingQualified:
                    jobStatus = JobStatusComplete;
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

        private static string WriteErrorFile(string reportName, string statusMsg)
        {
            var destFileName = Path.Combine(GetOutputRepository(), string.Format("AdHocReport_NoData_{0}.html", Guid.NewGuid()));

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

            File.WriteAllText(destFileName, sb.ToString());

            return destFileName;
        }

        private class AdHocReportUser
        {
            public Guid UserId { get; set; }
            public Guid DistrictId { get; set; }
        }
    }
}