namespace ProgressBook.Reporting.ExagoIntegration.VendorExtract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VendorExtractManager
    {
        private readonly Guid _districtId;

        public VendorExtractManager(Guid userId, Guid districtId)
        {
            _districtId = districtId;
            _vendorContext = new VendorExtractContext();
            _vendorBuilder = new VendorExtractBuilder();
        }

        //private ReportEntityDbContext _reportContext { get; set; }
        private VendorExtractContext _vendorContext { get; }
        //private ReportEntityManager _reportManager { get; set; }
        private VendorExtractBuilder _vendorBuilder { get; }

        public List<DistrictVendorFtpInfo> GetAll()
        {
            return _vendorContext.VendorExtractReportConfigs
                .Where(x => x.DistrictId == _districtId)
                .Select(_vendorBuilder.GetModel)
                .ToList();
        }

        public DistrictVendorFtpInfo GetDistrictVendorFtpInfo(Guid reportEntityId)
        {
            var entity = _vendorContext.VendorExtractReportConfigs
                .FirstOrDefault(x => x.ReportEntityId == reportEntityId && x.DistrictId == _districtId);
            return entity != null ? _vendorBuilder.GetModel(entity) : null;
        }

        public void SaveDistrictVendorFtpInfo(DistrictVendorFtpInfo vendorInfo)
        {
            if (vendorInfo.VendorExtractReportConfigId == Guid.Empty)
            {
                _vendorContext.VendorExtractReportConfigs.Add(_vendorBuilder.GetEntity(vendorInfo));
            }
            else
            {
                var entity = _vendorContext.VendorExtractReportConfigs.FirstOrDefault(x => x.VendorExtractReportConfigId == vendorInfo.VendorExtractReportConfigId);
                entity.DistrictId = vendorInfo.DistrictId;
                entity.ReportEntityId = vendorInfo.ReportEntityId;
                entity.Config = _vendorBuilder.GetEntity(vendorInfo).Config;
            }
            
            _vendorContext.SaveChanges();
        }

        public void DeleteDistrictVendorFtpInfo(Guid id)
        {
            var entity = _vendorContext.VendorExtractReportConfigs
                .FirstOrDefault(x => x.VendorExtractReportConfigId == id);
            if (entity == null) return;

            _vendorContext.VendorExtractReportConfigs.Remove(entity);
            _vendorContext.SaveChanges();
        }

        public DistrictVendorFtpInfo GetById(Guid id)
        {
            var entity = _vendorContext.VendorExtractReportConfigs
                .FirstOrDefault(x => x.VendorExtractReportConfigId == id);

            return entity != null ? _vendorBuilder.GetModel(entity) : null;
        }
    }
}