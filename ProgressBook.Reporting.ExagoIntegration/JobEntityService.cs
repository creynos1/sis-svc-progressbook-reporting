using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ProgressBook.Reporting.Data.Entities;
using ProgressBook.Reporting.Data.Repositories;
using WebReports.Api.Scheduler;

namespace ProgressBook.Reporting.ExagoIntegration
{
    public class JobEntityService : IDisposable
    {
        private readonly JobEntityDataService _jobEntityDataService;
        public JobEntityService()
        {
            _jobEntityDataService = new JobEntityDataService();
        }
        public List<QueueApiJob> GetAllQueueApiJobs()
        {
            var jobs = new List<QueueApiJob>();
            foreach (var schedule in _jobEntityDataService.GetAllJobEntities())
            {
                var jobData = _jobEntityDataService.GetJobData(schedule.JobId.ToString());
                var job = QueueApi.GetJob(jobData);
                jobs.Add(job);
            }
            return jobs;
        }
        public string UpdateRunningJobsOnStartup()
        {
            var logInfo = String.Empty;
            // change 'stuck' running jobs to ready, which probably was due to service going down before job was complete
            foreach (var job in _jobEntityDataService.GetAllJobEntities())
            {
                if (job.Status == (byte)JobStatus.Running)
                {
                    _jobEntityDataService.UpdateStatus(job.JobId.ToString(), (byte)JobStatus.Ready);
                    logInfo += $"Incomplete job status changed to ready: {job.ScheduleName}{System.Environment.NewLine}";
                }
            }
            return logInfo;
        }
        public void RenameReport(string reportName, JobEntity job)
        {
            XElement storedSchedule = XDocument.Parse(job.ScheduleXml).Element("schedule");
            storedSchedule.Element("report_name").Value = reportName;
            _jobEntityDataService.UpdateReportName(job.JobId.ToString(), reportName, storedSchedule.ToString(SaveOptions.DisableFormatting));
        }

        public void UpdateReport(string reportXml, JobEntity job)
        {
            XElement storedReport = XDocument.Parse(job.ReportXml).Element("report");
            XElement updatedReport = XDocument.Parse(reportXml).Element("report");
            // maintain user selected filter values if the filters still exist in the source report
            foreach (var currentFilter in storedReport.Descendants("filter"))
            {
                foreach (var updatedFilter in updatedReport.Descendants("filter"))
                {
                    if (currentFilter.Element("filter_name").Value == updatedFilter.Element("filter_name").Value)
                    {
                        updatedFilter.Element("values").ReplaceWith(currentFilter.Element("values"));
                    }
                }
            }
            _jobEntityDataService.UpdateReportXml(job.JobId.ToString(), updatedReport.ToString(SaveOptions.DisableFormatting));
        }
        public JobEntity GetNextExecuteJob()
        {
            DateTime executeJobDate = DateTime.Now;
            var filteredEntities = _jobEntityDataService.GetAllJobEntities()
                                                        .Where(j => j.Status == (byte)JobStatus.Ready
                                                            && j.NextExecuteDate <= executeJobDate)
                                                        .OrderByDescending(j => j.NextExecuteDate);
            if (filteredEntities.Any())
            {
                return filteredEntities.First();
            }
            return null;
        }
        public void UpdateStatus(string jobId, byte status)
        {
            _jobEntityDataService.UpdateStatus(jobId, status);
        }
        public string GetJobData(string jobId)
        {
            return _jobEntityDataService.GetJobData(jobId);
        }
        public JobEntity GetByJobId(string jobId)
        {
            return _jobEntityDataService.GetByJobId(jobId);
        }
        public List<JobEntity> GetAllJobEntities()
        {
            return _jobEntityDataService.GetAllJobEntities();
        }
        public void DeleteSchedule(string jobId)
        {
            _jobEntityDataService.DeleteSchedule(jobId);
        }
        public void SaveSchedule(QueueApiJob job)
        {
            var lastExecuteDate = GetLastExecuteDateFromXml(job.Xml);

            _jobEntityDataService.SaveSchedule(job, lastExecuteDate);
        }
        private DateTime GetLastExecuteDateFromXml(string jobXml)
        {
            XElement stored = XDocument.Parse(jobXml).Element("webreports");
            XElement storedJobInfo = stored.Element("jobinfo");
            var lastExecuteDateVal = storedJobInfo.Element("LastExecuteDate").Value;
            return DateTime.ParseExact(lastExecuteDateVal, "yyyy-MM-ddTHH:mm:ss", null);
        }
        public void Dispose()
        {
            _jobEntityDataService.Dispose();
        }
    }
}