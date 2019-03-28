namespace ProgressBook.Reporting.Data.Configuration
{
    using System.Data.Entity.ModelConfiguration;
    using ProgressBook.Reporting.Data.Entities;

    public class FolderConfiguration : EntityTypeConfiguration<Folder>
    {
        public FolderConfiguration()
        {
            Map<Folder>(t => t.Requires("EntityType").HasValue((byte)ReportEntityType.Folder));
        }
    }
}