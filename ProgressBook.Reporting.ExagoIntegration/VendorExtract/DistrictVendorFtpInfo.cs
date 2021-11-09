namespace ProgressBook.Reporting.ExagoIntegration.VendorExtract
{
    using System;

    public class DistrictVendorFtpInfo
    {
        public Guid VendorExtractReportConfigId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid ReportEntityId { get; set; }

        public string ReportPath { get; set; }
        public string ReportName { get; set; }
        public FtpConnectionInfo FtpConnectionInfo { get; set; }
    }
}