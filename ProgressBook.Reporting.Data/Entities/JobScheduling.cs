namespace ProgressBook.Reporting.Data.Entities
{
    using System;
    using System.Collections.Generic;

    public class JobScheduling
    {
        public JobScheduling()
        {
            Results = new List<JobSchedulingResult>();
        }

        public int JobSchedulingId { get; set; }
        public byte JobTypeId { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public byte JobStatusId { get; set; }
        public int? ReferenceId { get; set; }
        public byte JobDeliveryId { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateStop { get; set; }
        public string Parameters { get; set; }
        public Guid SchoolId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SessionId { get; set; }
        public byte[] RowVersion { get; set; }
        public string FileName { get; set; }
        public ICollection<JobSchedulingResult> Results { get; set; }
    }
}