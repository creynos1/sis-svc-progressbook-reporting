namespace ProgressBook.Reporting.Data.Configuration
{
    using System.Data.Entity.ModelConfiguration;
    using ProgressBook.Reporting.Data.Entities;

    public class TemplateConfiguration : EntityTypeConfiguration<Template>
    {
        public TemplateConfiguration()
        {
            Map<Template>(t => t.Requires("EntityType").HasValue((byte)ReportEntityType.Template));

            Property(t => t.BinaryContent).HasColumnName("BinaryContent");
        }
    }
}