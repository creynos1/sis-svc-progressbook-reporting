using ExagoReportUtility.Enums;
using ExagoReportUtility.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ExagoReportUtility.Objects
{
    public class OutputHelper
    {
        private string _sqlFileName { get; set; }
        private ExagoReportHelper _exagoReportHelper { get; set; }
        public OutputHelper()
        {

        }

        public OutputHelper(ExagoReportHelper exagoReportHelper)
        {
            this._exagoReportHelper = exagoReportHelper;
        }

        public void GenerateSqlFile(UserSelection userSelection)
        {
            switch (userSelection)
            {
                case (UserSelection.RENAMEDESCRIPTIONS):
                    {
                        GenerateReportDescrtiptionSqlFile();
                        break;
                    }
                case (UserSelection.RENAMEFILTERS):
                    {
                        // Has not been refactored.
                        throw new NotImplementedException();
                    }
                case (UserSelection.RENAMESORTS):
                    {
                        // Has not been refactored.
                        throw new NotImplementedException();
                    }
                case (UserSelection.RENAMETITLES):
                    {
                        GenerateReportTitleSqlFile();
                        break;
                    }
            }
        }

        private void GenerateReportDescrtiptionSqlFile()
        {
            // This does not output with escape Char
            _sqlFileName = "FixReportDescriptions.sql";
            string declaration = "DECLARE @reportId UNIQUEIDENTIFER \r\n BEGIN TRAN";
            using (var streamWriter = new StreamWriter(_sqlFileName, false))
            {
                streamWriter.WriteLine(declaration);
                foreach (var report in _exagoReportHelper.ReportDescriptions)
                {
                    if (_exagoReportHelper.ReportTitleDictionary.TryGetValue(report.ReportTitle.DatabaseTitle, out ReportEntity reportEntity))
                    {
                        streamWriter.WriteLine($"SET @reportId ='{reportEntity.ReportEntityId}' ");
                        streamWriter.WriteLine("UPDATE CoreReports.ReportEntity");
                        streamWriter.WriteLine($" SET Description = '{report.SoftwareAnswersDescription}' ");
                        var xmlDoc = ReplaceDescriptionInXml(ref reportEntity, report.SoftwareAnswersDescription);
                        streamWriter.WriteLine(", Content = '");
                        xmlDoc.Save(streamWriter);
                        streamWriter.WriteLine("', DateModified = GETUTCDATE()");
                        streamWriter.WriteLine("    WHERE reportEntityId = @reportId");
                        streamWriter.WriteLine("    AND userId IS NULL");
                    }
                }
                streamWriter.WriteLine("RollBack");
            }


        }

        private void GenerateReportTitleSqlFile()
        {
            // This does not output with escape Char
            _sqlFileName = "FixReportTitleNames.sql";
            string declaration = "DECLARE @reportId UNIQUEIDENTIFIER \r\n";
            using (var streamWriter = new StreamWriter(_sqlFileName, false))
            {
                streamWriter.WriteLine(declaration);
                foreach (var reportTitle in _exagoReportHelper.ReportTitles)
                {
                    if (_exagoReportHelper.ReportTitleDictionary.TryGetValue(reportTitle.DatabaseTitle, out ReportEntity report))
                    {
                        streamWriter.WriteLine($"SET @reportId = '{report.ReportEntityId}' ");
                        streamWriter.WriteLine("UPDATE CoreReports.ReportEntity");
                        streamWriter.WriteLine($" SET NAME = '{reportTitle.SoftwareAnswersReportTitle}' ");
                        if (reportTitle.HasIncorrectTitleOnReport)
                        {
                            var xmlDoc = ReplaceTitleInXml(ref report, reportTitle);
                            streamWriter.WriteLine(", Content = '");
                            xmlDoc.Save(streamWriter);
                            streamWriter.Write("'");
                        }
                        streamWriter.WriteLine(", DateModified = GETUTCDATE()");
                        streamWriter.WriteLine("    WHERE reportEntityId = @reportId");
                        streamWriter.WriteLine("    AND userId IS NULL");
                    }
                }
            }
        }

        private XmlDocument ReplaceTitleInXml(ref ReportEntity report, ReportTitle reportTitle)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(report.Content);
            XmlNodeList cellNodeList = xmlDocument.GetElementsByTagName("cell_text");
            foreach (XmlNode cellnode in cellNodeList)
            {
                if (cellnode.InnerText == reportTitle.ReportContentToUpdate)
                {
                    cellnode.InnerText = reportTitle.SoftwareAnswersReportTitle;
                }
            }
            return xmlDocument;
        }

        private XmlDocument ReplaceDescriptionInXml(ref ReportEntity report, string reportDescription)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(report.Content);
            var cellNode = xmlDocument.GetElementsByTagName("description")[0];
            cellNode.InnerText = reportDescription;
            return xmlDocument;
        }

        private void AddEscapeCharacters(ref ReportEntity report)
        {
            // This functionality is not working correctly and still needs to be implemented
            if (report.Content.Contains("'"))
            {
                report.Content.Replace("'", "''");
            }
            throw new NotImplementedException();
        }
        public void RemoveXmlDeclarations()
        {
            string[] Allline = File.ReadAllLines(_sqlFileName);
            List<string> NewLine = new List<string>();

            foreach (var textline in Allline)
            {
                if (!textline.StartsWith("<?"))
                {
                    NewLine.Add(textline);
                }
            }

            File.WriteAllLines(_sqlFileName, NewLine.AsEnumerable());
        }
    }
}
