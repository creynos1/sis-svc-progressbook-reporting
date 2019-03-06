namespace ProgressBook.Reporting.Client
{
    using ProgressBook.Reporting.Client.Data;
    using ProgressBook.Shared.Security.Factories;

    public class AdHocReportServiceFactory
    {
        public static readonly AdHocReportServiceFactory Instance = new AdHocReportServiceFactory();

        public IAdHocReportService Create()
        {
            var userAuthorizationService = DefaultUserAuthorizationServiceFactory.Instance.Create();
            return new AdHocReportService(userAuthorizationService,
                                          new UserReportAttributeRepository(new AdHocReportsContext()), new AdHocReportsRepository());
        }
    }
}