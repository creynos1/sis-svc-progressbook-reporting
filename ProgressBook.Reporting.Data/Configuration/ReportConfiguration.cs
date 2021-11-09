namespace ProgressBook.Reporting.Data.Configuration
{
    using System.Data.Entity.ModelConfiguration;
    using ProgressBook.Reporting.Data.Entities;

    public class ReportConfiguration : EntityTypeConfiguration<Report>
    {
        public ReportConfiguration()
        {
            Map<Report>(t => t.Requires("EntityType").HasValue((byte)ReportEntityType.Report));

            // ignored properties
            Ignore(t => t.DataObjects);
        }
    }
}