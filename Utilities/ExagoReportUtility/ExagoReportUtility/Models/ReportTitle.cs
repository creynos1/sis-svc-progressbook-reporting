
namespace ExagoReportUtility.Models
{
  public class ReportTitle
    {
        public string DatabaseTitle { get; set; }
        public string SoftwareAnswersReportTitle { get; set; }
        public bool HasIncorrectTitleOnReport { get; set; }
        
        // This is a property that is necessary because
        // sometimes the report content title varies from 
        // the database title as well
        public string ReportContentToUpdate { get; set; }


        public ReportTitle(string reportTitle)
        {
            this.DatabaseTitle = reportTitle;
        }

        public ReportTitle()
        {

        }
    }
}
