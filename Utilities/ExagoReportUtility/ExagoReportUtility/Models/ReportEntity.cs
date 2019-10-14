

namespace ExagoReportUtility.Models
{
    public class ReportEntity
    {
        public string ReportEntityId { get; set; }
        public string ParentId { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }


        public ReportEntity()
        {

        }
       

        public ReportEntity(string reportEntityId, string parentId, string content)
        {
            ReportEntityId = reportEntityId;
            ParentId = parentId;
            Content = content;

        }
    }
}
