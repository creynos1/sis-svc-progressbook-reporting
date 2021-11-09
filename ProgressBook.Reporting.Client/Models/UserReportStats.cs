namespace ProgressBook.Reporting.Client.Models
{
    using System;

    public class UserReportStats
    {
        public int RunCount { get; set; }
        public DateTime? LastRunDate { get; set; }
        public bool IsFavorite { get; set; }
    }
}