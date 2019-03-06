namespace ProgressBook.Reporting.ExagoIntegration
{
    using System.IO;
    using System.Text;
    using System.Xml;
    using ProgressBook.Reporting.Data.Repositories;
    using ProgressBook.Reporting.Data;

    public interface IExagoConfigurationFileFactory
    {
        string GetConfigurationFile();
        void WriteConfigurationFile(string path);
    }

    public class ExagoConfigurationFileFactory : IExagoConfigurationFileFactory
    {
        private readonly IExagoEntitySerializer _exagoEntitySerializer;
        private readonly IExagoSettings _exagoSettings;
        private readonly string _sisEntityDataSourceId;

        public ExagoConfigurationFileFactory(IExagoEntitySerializer exagoEntitySerializer, IExagoSettings exagoSettings, bool useSisReadonlyDataSource = false)
        { 
            _exagoEntitySerializer = exagoEntitySerializer;
            _exagoSettings = exagoSettings;
            _sisEntityDataSourceId = useSisReadonlyDataSource
                    ? exagoSettings.SisReadonlyDataSourceId
                    : exagoSettings.SisDataSourceId;
        }

        public string GetConfigurationFile()
        {
            var doc = CreateCombinedConfigurationFile();
            var stringWriter = new StringWriter();
            doc.Save(stringWriter);
            return stringWriter.ToString();
        }

        public void WriteConfigurationFile(string path)
        {
            var doc = CreateCombinedConfigurationFile();
            doc.Save(path);
        }

        private XmlDocument CreateCombinedConfigurationFile()
        {
            var doc = new XmlDocument();
            using (var stream = GetType().Assembly.GetManifestResourceStream(_exagoSettings.BaseLineConfigFileName))
            {
                doc.Load(stream);
            }
            PopulateDynamicEntities(doc);
            return doc;
        }

        private void PopulateDynamicEntities(XmlDocument doc)
        {
            var frag = doc.CreateDocumentFragment();
            var sb = new StringBuilder();

            var sisReportViewInfoRepository = new SisReportViewInfoRepository();
            var sisReportColumnMetaDataRepository = new SisReportColumnMetaDataRepository();
            var sisViewInfoList = sisReportViewInfoRepository.GetSisEntities();
            var sisViewMetaData = sisReportColumnMetaDataRepository.GetSisEntityMetadata();
            sisViewInfoList.Merge(sisViewMetaData);
            
            foreach (var viewInfo in sisViewInfoList)
            {
                sb.AppendLine(_exagoEntitySerializer.ToXml(viewInfo, _sisEntityDataSourceId));
            }

            if (!_exagoSettings.DisableGradeBookIntegration)
            {
                var gradeBookReportViewInfoRepository = new GradeBookReportViewInfoRepository();
                var gradeBookViewInfoList = gradeBookReportViewInfoRepository.GetGradeBookEntities();

                foreach (var viewInfo in gradeBookViewInfoList)
                {
                    sb.AppendLine(_exagoEntitySerializer.ToXml(viewInfo, _exagoSettings.GradeBookDataSourceId));
                }
            }

            frag.InnerXml = sb.ToString();

            var rootNode = doc.SelectSingleNode("webreports");
            var firstRoleNode = rootNode.SelectSingleNode("role[1]");
            rootNode.InsertBefore(frag, firstRoleNode);
        }
    }
}