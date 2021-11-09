namespace ProgressBook.Reporting.Data.Configuration
{
    using System.Data.Entity.ModelConfiguration;
    using ProgressBook.Reporting.Data.Entities;

    public class ThemeConfiguration : EntityTypeConfiguration<Theme>
    {
        public ThemeConfiguration()
        {
            Map<Theme>(t => t.Requires("EntityType").HasValue((byte)ReportEntityType.Theme));
        }
    }
}