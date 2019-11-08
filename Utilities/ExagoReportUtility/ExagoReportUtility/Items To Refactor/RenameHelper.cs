using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using ExagoReportUtility.DataAccessObjects;


namespace ExagoReportUtility.Objects
{
    /// <summary>
    /// Helper class that does all the work for renaming the Exago filters
    /// </summary>
    class RenameHelper
    {
        private readonly ExagoDataList _exagoDataList;
        private readonly string _sqlFileName;

        public RenameHelper()
        {
            var dataAccess = new DataAccess();
            var reportTable = dataAccess.GetDataTable(ConstructSqlForReportTable());
            var filterTable = dataAccess.GetDataTable(ConstructSQLForFilterTable());
            var sortTable = dataAccess.GetDataTable(ConstructSqlForSortTable());
            _sqlFileName = "FixedExagoDefaultFilterAndSort.sql";
            _exagoDataList = new ExagoDataList();
            _exagoDataList.MapFriendlyFilter(filterTable);
            _exagoDataList.MapFriendlySort(sortTable);
            _exagoDataList.MapReportEntities(reportTable);

        }




        public void GenerateSqlFile()
        {

            using (var streamWriter = new StreamWriter(_sqlFileName, false))
            {
                foreach (var report in _exagoDataList.reportEntityList)
                {
                    
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(report.Content);
                    XmlNodeList nodeFilterList = xmlDocument.GetElementsByTagName("filter");
                    XmlNodeList nodeSortList = xmlDocument.GetElementsByTagName("sort");
                    if (nodeSortList.Count == 0 && nodeFilterList.Count == 0)
                    {
                        continue;
                    }
                    if (nodeFilterList.Count != 0)
                    {
                        foreach (XmlNode node in nodeFilterList)
                        {
                            var parentNode = node;
                            var filterNode = node.FirstChild;
                            var exagoFilterName = filterNode.InnerText.ToString();
                            var friendlyFilterName = (from f in _exagoDataList.exagoFilterList where f.ExagoFilterName == exagoFilterName select f.SoftwareAnswersFilterName).FirstOrDefault();
                            if (friendlyFilterName == null)
                            {
                                Global.additions.Debug("Filter Name " + exagoFilterName);
                                continue;
                            }
                            XmlElement childNode = xmlDocument.CreateElement("filter_title");
                            childNode.InnerText = friendlyFilterName;
                            parentNode.AppendChild(childNode);
                        }
                    }
                    if (nodeSortList.Count != 0)
                    {
                        foreach (XmlNode node in nodeSortList)
                        {
                            var parentNode = node;
                            var sortNode = node.FirstChild;
                            var exagoSortName = sortNode.InnerText.ToString();
                            var friendlySortName = (from s in _exagoDataList.exagoSortList where s.ExagoSortName == exagoSortName select s.SoftwareAnswersSortName).FirstOrDefault();
                            if (friendlySortName == null)
                            {
                                friendlySortName = (from s in _exagoDataList.exagoFilterList where s.ExagoFilterName == exagoSortName select s.SoftwareAnswersFilterName).FirstOrDefault();
                                if (friendlySortName == null)
                                {
                                    Global.additions.Debug("Sort Name " + exagoSortName + "Reference Report ID = " + report.ReportEntityId);
                                    continue;
                                }
                            }
                            XmlElement childNode = xmlDocument.CreateElement("sort_title");
                            childNode.InnerText = friendlySortName;
                            XmlNode nodetest = parentNode.SelectSingleNode("sort_title");
                            parentNode.ReplaceChild(childNode, nodetest);
                        }
                    }
                    streamWriter.WriteLine("Update CoreReports.ReportEntity SET [Content] = '");
                    AddEscapeCharacters(xmlDocument);
                    xmlDocument.Save(streamWriter);
                    streamWriter.WriteLine("' WHERE [ParentId] = \'" + report.ParentId + "' AND [ReportEntityId] = \'" + report.ReportEntityId + "\'");
                }
            }

            RemoveXmlDeclarations(_sqlFileName);

        }


        private string ConstructSQLForFilterTable()
        {
            return @"SELECT [ExagoFilterName], [FriendlyFilterName]
                     FROM CoreReports.FriendlyFilterNames";


        }
        private string ConstructSqlForSortTable()
        {
            return @"SELECT [ExagoSortName], [FriendlySortName]
                     FROM CoreReports.FriendlySortNames";
        }

        private string ConstructSqlForReportTable()
        {
            return @"SELECT [ReportEntityId], [ParentId], [Content]
                     FROM CoreReports.ReportEntity
                     WHERE [UserID] IS NULL AND [DistrictID] IS NULL AND [Content] IS NOT NULL AND [EntityType] = 1";
        }

        private void AddEscapeCharacters(XmlDocument xmlDocument)
        {
            var xmlParent = xmlDocument.FirstChild;
            if (xmlParent.InnerText.Contains("'"))
            {
                xmlParent.InnerText = xmlParent.InnerText.Replace("'", "''");
            }
        }

        private void RemoveXmlDeclarations(string fileName)
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



