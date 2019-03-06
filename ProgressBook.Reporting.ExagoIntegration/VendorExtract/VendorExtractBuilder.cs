namespace ProgressBook.Reporting.ExagoIntegration.VendorExtract
{
    using System;
    using System.Web.Script.Serialization;

    public class VendorExtractBuilder
    {
        public DistrictVendorFtpInfo GetModel(VendorExtractReportConfig config)
        {
            return new DistrictVendorFtpInfo
            {
                VendorExtractReportConfigId = config.VendorExtractReportConfigId,
                ReportEntityId = config.ReportEntityId,
                DistrictId = config.DistrictId,
                FtpConnectionInfo = Deserialize(config.Config, $"{config.ReportEntityId}_{config.DistrictId}")
            };
        }

        public VendorExtractReportConfig GetEntity(DistrictVendorFtpInfo vendorFtpInfo)
        {
            return new VendorExtractReportConfig
            {
                VendorExtractReportConfigId = vendorFtpInfo.VendorExtractReportConfigId == Guid.Empty ? Guid.NewGuid() : vendorFtpInfo.VendorExtractReportConfigId,
                ReportEntityId = vendorFtpInfo.ReportEntityId,
                DistrictId = vendorFtpInfo.DistrictId,
                Config = Serialize(vendorFtpInfo.FtpConnectionInfo, $"{vendorFtpInfo.ReportEntityId}_{vendorFtpInfo.DistrictId}")
            };
        }

        private static string Serialize(FtpConnectionInfo obj, string key)
        {
            var scriptSerializer = new JavaScriptSerializer();
            scriptSerializer.MaxJsonLength = int.MaxValue;
            var str = scriptSerializer.Serialize(obj);

            return AESThenHMAC.SimpleEncryptWithPassword(str, key);
        }

        private static FtpConnectionInfo Deserialize(string encrypted, string key)
        {
            var json = AESThenHMAC.SimpleDecryptWithPassword(encrypted, key);

            var scriptSerializer = new JavaScriptSerializer();
            scriptSerializer.MaxJsonLength = int.MaxValue;
            return scriptSerializer.Deserialize<FtpConnectionInfo>(json);
        }
    }
}