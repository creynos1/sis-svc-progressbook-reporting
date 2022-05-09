
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ProgressBook.Reporting.Data.Entities;

namespace ProgressBook.Reporting.Data.Configuration
{
    public class JobEntityConfiguration : EntityTypeConfiguration<JobEntity>
    {
        public JobEntityConfiguration()
        {
            HasKey(t => t.Id);
            // Properties
            Property(t => t.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // Table & Column Mappings
            ToTable("JobEntity", "CoreReports");
        }
    }
}
