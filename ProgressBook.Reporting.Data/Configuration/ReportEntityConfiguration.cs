namespace ProgressBook.Reporting.Data.Configuration
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using ProgressBook.Reporting.Data.Entities;

    public class ReportEntityConfiguration : EntityTypeConfiguration<ReportEntity>
    {
        public ReportEntityConfiguration()
        {
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Name)
                .HasMaxLength(255);

            // Table & Column Mappings
            ToTable("ReportEntity", "CoreReports");
            Property(t => t.Id).HasColumnName("ReportEntityId");
            Property(t => t.ParentId).HasColumnName("ParentId");
            Property(t => t.DistrictId).HasColumnName("DistrictId");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.Content).HasColumnName("Content");
            Property(t => t.IsLeafNode).HasColumnName("IsLeafNode");
            Property(t => t.IsReadOnly).HasColumnName("IsReadOnly");
            Property(t => t.DateModified).HasColumnName("DateModified");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.IsInternal).HasColumnName("IsInternal");

            Ignore(t => t.Path);
            Ignore(t => t.ReportEntityType);

            HasOptional(t => t.Parent)
                .WithMany(t => t.Children)
                .HasForeignKey(t => t.ParentId);
        }
    }
}