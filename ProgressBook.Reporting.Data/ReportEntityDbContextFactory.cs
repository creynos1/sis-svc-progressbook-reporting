namespace ProgressBook.Reporting.Data
{
    using System;

    public class ReportEntityDbContextFactory
    {
        public static IReportEntityDbContext Create()
        {
            return new ReportEntityDbContext("StudentInformation");
        }

        public static IReportEntityDbContext Create(string nameOrConnectionString)
        {
            return new ReportEntityDbContext(nameOrConnectionString);
        }

        public static IReportEntityDbContext Create(Guid districtId)
        {
            return new ReportEntityDbContext("StudentInformation", districtId, null);
        }

        public static IReportEntityDbContext Create(Guid districtId, Guid userId)
        {
            return new ReportEntityDbContext("StudentInformation", districtId, userId);
        }

        public static IReportEntityDbContext Create(string nameOrConnectionString, Guid? districtId, Guid? userId)
        {
            return new ReportEntityDbContext(nameOrConnectionString, districtId, userId);
        }
    }
}