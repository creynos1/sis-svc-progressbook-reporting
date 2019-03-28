namespace ProgressBook.Reporting.Client.Models
{
    using System;

    public class AdHocReport
    {
        public Guid ReportEntityId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsInternal { get; set; }
        public bool NestedDisplay { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedBy { get; set; }

        public UserReportStats UserReportStats { get; set; }
    }
}