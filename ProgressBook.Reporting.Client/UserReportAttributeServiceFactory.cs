namespace ProgressBook.Reporting.Client
{
    using ProgressBook.Reporting.Client.Data;

    public class UserReportAttributeServiceFactory
    {
        public static readonly UserReportAttributeServiceFactory Instance = new UserReportAttributeServiceFactory();

        public IUserReportAttributeService Create()
        {
            return new UserReportAttributeService(new AdHocReportsContext());
        }
    }
}