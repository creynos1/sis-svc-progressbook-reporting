namespace ProgressBook.Reporting.Data.Entities
{
    public class Template : ReportEntity
    {
        public Template() : base(ReportEntityType.Template)
        {
        }

        public byte[] BinaryContent { get; set; }
    }
}