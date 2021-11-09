namespace ProgressBook.Reporting.Data.Configuration
{
    using System.Data.Entity.ModelConfiguration;
    using ProgressBook.Reporting.Data.Entities;

    public class JobSchedulingConfiguration : EntityTypeConfiguration<JobScheduling>
    {
        public JobSchedulingConfiguration()
        {
            // Primary Key
            HasKey(t => t.JobSchedulingId);

            // Properties
            Property(t => t.JobName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.JobDescription)
                .HasMaxLength(250);

            Property(t => t.Parameters)
                .IsRequired();

            Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            Property(t => t.FileName)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("tblJobScheduling", "dbo");
            Property(t => t.JobSchedulingId).HasColumnName("JobSchedulingId");
            Property(t => t.JobTypeId).HasColumnName("JobTypeId");
            Property(t => t.JobName).HasColumnName("JobName");
            Property(t => t.JobDescription).HasColumnName("JobDescription");
            Property(t => t.JobStatusId).HasColumnName("JobStatusId");
            Property(t => t.ReferenceId).HasColumnName("ReferenceId");
            Property(t => t.JobDeliveryId).HasColumnName("JobDeliveryId");
            Property(t => t.DateStart).HasColumnName("DateStart");
            Property(t => t.DateStop).HasColumnName("DateStop");
            Property(t => t.Parameters).HasColumnName("Parameters");
            Property(t => t.SchoolId).HasColumnName("SchoolId");
            Property(t => t.DateAdded).HasColumnName("DateAdded");
            Property(t => t.DateModified).HasColumnName("DateModified");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.SessionId).HasColumnName("SessionId");
            Property(t => t.RowVersion).HasColumnName("rv");
            Property(t => t.FileName).HasColumnName("FileName");
        }
    }
}