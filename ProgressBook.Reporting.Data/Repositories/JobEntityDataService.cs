using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ProgressBook.Reporting.Data.Entities;
using WebReports.Api.Scheduler;

namespace ProgressBook.Reporting.Data.Repositories
{
    public class JobEntityDataService : IDisposable
    {
        private readonly JobEntityDbContext _dbContext;
        public JobEntityDataService()
        {
            _dbContext = new JobEntityDbContext();
        }
        public JobEntity GetByJobId(string jobId)
        {
            var currentId = new Guid(jobId);
            JobEntity job = _dbContext.JobEntities.FirstOrDefault(x => (x.JobId == currentId));
            return job;
        }
        public List<JobEntity> GetAllJobEntities()
        {
            return _dbContext.JobEntities.ToList();
        }
        public void UpdateStatus(string jobId, byte status)
        {
            var job = GetByJobId(jobId);
            job.Status = status;
            _dbContext.SaveChanges();
        }
        public void UpdateReportName(string jobId, string reportName, string scheduleXml)
        {
            var job = GetByJobId(jobId);
            job.ReportName = reportName;
            job.ScheduleXml = scheduleXml;
            _dbContext.SaveChanges();
        }
        public void UpdateReportXml(string jobId, string reportXml)
        {
            var job = GetByJobId(jobId);
            job.ReportXml = reportXml;
            _dbContext.SaveChanges();
        }

        public void SaveSchedule(QueueApiJob job, DateTime? lastExecuteDate)
        {
            var schedule = GetByJobId(job.JobId);
            if (schedule == null)
            {
                schedule = new JobEntity { };
                _dbContext.JobEntities.Add(schedule);
            }
            else
            {
                schedule.LastExecuteDate = (DateTime)lastExecuteDate;
            }
            schedule.NextExecuteDate = job.NextExecuteDate;
            schedule.Status = (byte)job.Status;
            schedule.ScheduleName = job.ScheduleName;
            schedule.ReportName = job.ReportName;
            schedule.ReportId = new Guid(job.ReportId);
            schedule.ConfigXml = job.ConfigXml;
            schedule.ReportXml = job.ReportXml;
            schedule.ScheduleXml = job.ScheduleXml;
            schedule.JobInfoXml = job.JobInfoXml;
            _dbContext.SaveChanges();
        }
        public void DeleteSchedule(string jobId)
        {
            var job = GetByJobId(jobId);
            _dbContext.JobEntities.Remove(job);
            _dbContext.SaveChanges();
        }

        public string GetJobData(string jobId)
        {
            var schedule = GetByJobId(jobId);

            XElement jobData = new XElement("webreports",
                XDocument.Parse(schedule.ConfigXml).Element("config"),
                XDocument.Parse(schedule.ReportXml).Element("report"),
                XDocument.Parse(schedule.ScheduleXml).Element("schedule"),
                XDocument.Parse(schedule.JobInfoXml).Element("jobinfo")
            );
            return jobData.ToString(SaveOptions.DisableFormatting);
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
