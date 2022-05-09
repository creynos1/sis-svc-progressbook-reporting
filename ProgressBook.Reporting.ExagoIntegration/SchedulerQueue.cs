
using System;
using System.IO;
using System.Collections.Generic;
using WebReports.Api.Scheduler;

namespace ProgressBook.Reporting.ExagoIntegration
{
    public class SchedulerQueue
    {
        private const string QUEUE_DIRECTORY = @"C:\Program Files\Exago\ExagoScheduler\working";
        private const int FlushTime = 1;  // hours; Flush is called from Exago web app, so we don't have the flush time to pass in (which is part of scheduler service config)
        private static string LogFn = null;

        static SchedulerQueue()
        {
            LogFn = String.Format(@"{0}\log.txt", QUEUE_DIRECTORY);
        }
        // called when a specific scheduler service starts; service name is in format MachineName:Port
        public static void Start(string serviceName)
        {
            Log("Start: " + serviceName);
            var logInfo = String.Empty;
            using (var jobEntityService = new JobEntityService())
            {
                logInfo = jobEntityService.UpdateRunningJobsOnStartup();
            }

            Log(logInfo);
        }
        // returns array of jobs for scheduler manager
        public static string[] GetJobList(string viewLevel, string companyId, string userId)
        {
            Log("GetJobList");
            List<string> jobXmlList = new List<string>();
            using (var jobEntityService = new JobEntityService())
            {
                foreach (var job in jobEntityService.GetAllQueueApiJobs())
                {
                    if (job.IsUserViewable(viewLevel, companyId, userId))
                    {
                        jobXmlList.Add(job.JobListXml); // just job info and schedule for efficiency (doesn't include report or config)
                    }
                }
            }

            return jobXmlList.ToArray();
        }
        // returns next job to execute
        public static string GetNextExecuteJob(string serviceName)
        {
            // we need to flush occasionally to remove completed or deleted jobs; we can do this in a method that is hit like here, or start a thread that does it occasionally
            ProcessFlush(FlushTime);
            using (var jobEntityService = new JobEntityService())
            {
                var job = jobEntityService.GetNextExecuteJob();
                if (job == null)
                {
                    return null;
                }
                jobEntityService.UpdateStatus(job.JobId.ToString(), (byte)JobStatus.Running);
                Log("Set job to execute status: " + job.ScheduleName);
                return jobEntityService.GetJobData(job.JobId.ToString());
            }
        }
        // called when a job is added or updated; check job.Status for result since job may be finished with an execution
        public static void SaveJob(string jobXml)
        {
            Log("SaveJob");
            QueueApiJob job = QueueApi.GetJob(jobXml);
            using (var jobEntityService = new JobEntityService())
            {
                if (job.Status == JobStatus.Removed)
                {
                    jobEntityService.DeleteSchedule(job.JobId);
                }
                else
                {
                    jobEntityService.SaveSchedule(job);
                }
            }
        }
        // returns entire XML package for a specific job id
        public static string GetJobData(string jobId)
        {
            Log("GetJobData");
            var jobData = String.Empty;
            using (var jobEntityService = new JobEntityService())
            {
                jobData = jobEntityService.GetJobData(jobId);
            }
            return jobData;
        }
        // deletes any jobs that contains a specific report id (soft delete); triggered by deleting a report in the Exago UI
        public static void DeleteReport(string reportId)
        {
            Log("DeleteReport: " + reportId);
            using (var jobEntityService = new JobEntityService())
            {
                foreach (var job in jobEntityService.GetAllQueueApiJobs())
                {
                    if (job != null && job.ReportId == reportId)
                    {
                        job.SetDelete();
                        var schedule = jobEntityService.GetByJobId(job.JobId);
                        jobEntityService.UpdateStatus(schedule.JobId.ToString(), (byte)job.Status);
                    }
                }
            }
        }
        // renames any reports within jobs that contains a specific report id; triggered by renaming a report in the Exago UI
        public static void RenameReport(string reportId, string reportName)
        {
            Log("RenameReport: " + reportId + " " + reportName);
            var reportGuid = new Guid(reportId);
            using (var jobEntityService = new JobEntityService())
            {
                foreach (var schedule in jobEntityService.GetAllJobEntities())
                {
                    if (schedule.ReportId == reportGuid)
                    {
                        jobEntityService.RenameReport(reportName, schedule);
                    }
                }
            }
        }
        // updates report contents within any jobs that contain the specific report id; triggered by saving a report in the Exago UI
        public static void UpdateReport(string reportId, string reportXml)
        {
            Log("UpdateReport: " + reportId);

            var reportGuid = new Guid(reportId);
            using (var jobEntityService = new JobEntityService())
            {
                foreach (var schedule in jobEntityService.GetAllJobEntities())
                {
                    if (schedule != null && schedule.ReportId == reportGuid)
                    {
                        jobEntityService.UpdateReport(reportXml, schedule);
                    }
                }
            }
        }
        // deletes any jobs that are marked as deleted or completed; triggered by click of flush button from Exago scheduler manager
        public static void Flush(string viewLevel, string companyId, string userId)
        {
            Log("Flush");
            ProcessFlush(0, viewLevel, companyId, userId);
        }
        private static void ProcessFlush(int flushTime, string viewLevel = null, string companyId = null, string userId = null)
        {
            using (var jobEntityService = new JobEntityService())
            {
                foreach (var job in jobEntityService.GetAllQueueApiJobs())
                {
                    job.SetFlush(flushTime, viewLevel, companyId, userId);
                    if (job.Status == JobStatus.Removed)
                    {
                        jobEntityService.DeleteSchedule(job.JobId);
                        Log("Delete job: " + job.ScheduleName);
                    }
                }
            }
        }
        private static void Log(string info)
        {
            try
            {
                File.AppendAllText(LogFn, DateTime.Now.ToString("MM/dd/yyyy hh:mmtt "));
                File.AppendAllText(LogFn, info);
                File.AppendAllText(LogFn, Environment.NewLine);
            }
            catch { /* do nothing */ }
        }
    }

}
