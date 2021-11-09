namespace ProgressBook.Reporting.Data.Configuration
{
    using System.Data.Entity.ModelConfiguration;
    using ProgressBook.Reporting.Data.Entities;

    public class JobSchedulingResultConfiguration : EntityTypeConfiguration<JobSchedulingResult>
    {
        public JobSchedulingResultConfiguration()
        {
            // Primary Key
            HasKey(t => t.ReportResultId);

            // Properties
            Property(t => t.Filename)
                .HasMaxLength(500);

            Property(t => t.FilePath)
                .HasMaxLength(4);

            // Table & Column Mappings
            ToTable("tblReportResult", "dbo");
            Property(t => t.ReportResultId).HasColumnName("ReportResultId");
            Property(t => t.ReportId).HasColumnName("ReportId");
            Property(t => t.JobSchedulingId).HasColumnName("JobSchedulingId");
            Property(t => t.Filename).HasColumnName("Filename");
            Property(t => t.FileSize).HasColumnName("FileSize");
            Property(t => t.IsRetained).HasColumnName("IsRetained");
            Property(t => t.FilePath).HasColumnName("FilePath");

            // Relationships
            HasRequired(t => t.JobScheduling)
                .WithMany(t => t.Results)
                .HasForeignKey(d => d.JobSchedulingId);
        }
    }
}