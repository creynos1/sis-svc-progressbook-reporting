using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressBook.Reporting.Data.Entities
{
    public class JobEntity
    {
        public long Id { get; set; }
        public Guid? JobId { get; set; }
        public DateTime NextExecuteDate { get; set; }
        public DateTime LastExecuteDate { get; set; }
        public byte Status { get; set; }
        public string ScheduleName { get; set; }
        public string ReportName { get; set; }
        public Guid? ReportId { get; set; }
        public string ConfigXml { get; set; }
        public string ReportXml { get; set; }
        public string ScheduleXml { get; set; }
        public string JobInfoXml { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
