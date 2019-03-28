namespace ProgressBook.Reporting.Data.Entities
{
    public class JobSchedulingResult
    {
        public int ReportResultId { get; set; }
        public int ReportId { get; set; }
        public int JobSchedulingId { get; set; }
        public string Filename { get; set; }
        public int FileSize { get; set; }
        public bool IsRetained { get; set; }
        public string FilePath { get; set; }
        public JobScheduling JobScheduling { get; set; }
    }
}
