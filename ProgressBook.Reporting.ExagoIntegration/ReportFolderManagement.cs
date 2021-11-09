namespace ProgressBook.Reporting.ExagoIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Xml;
    using ProgressBook.Reporting.Data;
    using ProgressBook.Reporting.Data.Entities;
    using ProgressBook.Reporting.Data.Repositories;
    using WebReports.Api.Common;

    /// <summary>
    ///     Exago integration facade for custom report/folder management.
    /// </summary>
    /// <remarks>
    ///     NOTE: The public static methods are called by Exago using reflection. The method signatures should not be changed.
    /// </remarks>
    public class ReportFolderManagement
    {
        public static string GetReportListXml(SessionInfo sessionInfo)
        {
            var list = new List<ReportEntity>();

            //using (var dbContext = ReportEntityDbContextFactory.Create()) //GetUserDbContext(sessionInfo))
            using (var dbContext = GetUserDbContext(sessionInfo))
            {
                var mgr = new ReportEntityManager(dbContext);
                list = mgr.GetAll();

                // place My Reports folder (if exists) at end of folder list
                var myReportsFolder = list.Find(entity => entity.Name == "My Reports");
                if (myReportsFolder != null)
                {
                    list.Remove(myReportsFolder);
                    list.Insert(0, myReportsFolder);
                }

                // place My Reports folder (if exists) at end of folder list
                var pendingReportsFolder = list.Find(entity => entity.Name == "Pending Reports");
                if (pendingReportsFolder != null)
                {
                    list.Remove(pendingReportsFolder);
                    list.Insert(1, pendingReportsFolder);
                }
            }

            var xDoc = new XmlDocument();
            XmlNode rootNode = xDoc.CreateElement("reportlist");
            xDoc.AppendChild(rootNode);

            var xml = WriteXml(list, xDoc, rootNode);
            return xml;
        }

        public static string AddFolder(SessionInfo sessionInfo, string folderName)
        {
            try
            {
                using (var dbContext = GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);
                    var entity = mgr.GetByPath<ReportEntity>(folderName);
                    if (entity != null)
                        return String.Format("Folder '{0}' already exists", folderName);

                    mgr.AddFolder(folderName);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static string DeleteFolder(SessionInfo sessionInfo, string folderName)
        {
            try
            {
                using (var dbContext = GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);
                    mgr.DeleteFolder(folderName);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static string RenameFolder(SessionInfo sessionInfo, string oldName, string newName)
        {
            try
            {
                using (var dbContext = GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);

                    var entity = mgr.GetByPath<ReportEntity>(newName);
                    if (entity != null)
                        return String.Format("Folder '{0}' already exists", newName);

                    mgr.RenameFolder(oldName, newName);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static bool ExistFolder(SessionInfo sessionInfo, string folderName)
        {
            using (var dbContext = GetDbContext(sessionInfo))
            {
                var mgr = new ReportEntityManager(dbContext);
                var folder = mgr.GetByPath<Folder>(folderName);

                return folder != null;
            }
        }

        public static string MoveFolder(SessionInfo sessionInfo, string oldName, string newName)
        {
            try
            {
                using (var dbContext = GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);

                    var entity = mgr.GetByPath<ReportEntity>(newName);
                    if (entity != null)
                        return String.Format("Folder '{0}' already exists", newName);

                    mgr.MoveFolder(oldName, newName);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static string GetReportXml(SessionInfo sessionInfo, string reportName)
        {
            try
            {
                using (var dbContext = IsMyReportsPath(reportName) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);
                    var report = mgr.GetByPath<Report>(reportName);

                    if (report != null)
                        return report.Content;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static string SaveReport(SessionInfo sessionInfo, string reportName, string reportXml)
        {
            try
            {
                using (var dbContext = IsMyReportsPath(reportName) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);
                    mgr.SaveReport(reportName, reportXml, GetModifiedByUserId(sessionInfo));
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static string DuplicateReport(SessionInfo sessionInfo, string reportName, string reportXml)
        {
            return SaveReport(sessionInfo, reportName, reportXml);
        }

        public static string DeleteReport(SessionInfo sessionInfo, string reportName)
        {
            try
            {
                using (var dbContext = IsMyReportsPath(reportName) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);
                    mgr.DeleteReport(reportName);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static string RenameReport(SessionInfo sessionInfo, string reportName, string newReportName)
        {
            if (reportName == newReportName)
                return null;

            try
            {
                if ((IsMyReportsPath(reportName) && IsMyReportsPath(newReportName)) ||
                    (!IsMyReportsPath(reportName) && !IsMyReportsPath(newReportName)))
                {
                    using (var dbContext = IsMyReportsPath(reportName) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
                    {
                        var mgr = new ReportEntityManager(dbContext);

                        var entity = mgr.GetByPath<ReportEntity>(newReportName);
                        if (entity != null)
                            return string.Format("Folder '{0}' already exists", newReportName);

                        mgr.RenameReport(reportName, newReportName, GetModifiedByUserId(sessionInfo));
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    using (var srcDbContext = IsMyReportsPath(reportName) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
                    {
                        var mgr = new ReportEntityManager(srcDbContext);
                        var report = mgr.GetByPath<Report>(reportName);

                        using (var destdbContext = IsMyReportsPath(newReportName) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
                        {
                            var sysMgr = new ReportEntityManager(destdbContext);
                            sysMgr.SaveReport(newReportName, report.Content, GetModifiedByUserId(sessionInfo));
                            destdbContext.SaveChanges();
                        }

                        mgr.DeleteReport(reportName);
                        srcDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        public static List<string> GetTemplateList(SessionInfo sessionInfo)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["current_web_report"] = sessionInfo.Report.Name;
            }

            using (var dbContext = IsMyReportsPath(sessionInfo.Report.Name) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
            {
                var mgr = new ReportEntityManager(dbContext);
                return mgr.GetTemplateListForReport(sessionInfo.Report.Name);
            }
        }

        public static byte[] GetTemplate(SessionInfo sessionInfo, string templateName)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["current_web_report"] = sessionInfo.Report.Name;
            }

            using (var dbContext = IsMyReportsPath(sessionInfo.Report.Name) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
            {
                var mgr = new ReportEntityManager(dbContext);
                return mgr.GetTemplateForReport(sessionInfo.Report.Name, templateName);
            }
        }

        public static string SaveTemplate(SessionInfo sessionInfo, string templateName, byte[] templateData)
        {
            try
            {
                string reportName = null;
                if (HttpContext.Current != null)
                {
                    reportName = HttpContext.Current.Session["current_web_report"].ToString();
                }

                using (var dbContext = IsMyReportsPath(reportName) ? GetUserDbContext(sessionInfo) : GetDbContext(sessionInfo))
                {
                    var mgr = new ReportEntityManager(dbContext);
                    mgr.SaveTemplateForReport(reportName, templateName, null, templateData, GetModifiedByUserId(sessionInfo));
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        private static Guid GetModifiedByUserId(SessionInfo sessionInfo)
        {
            return new Guid(sessionInfo.GetParameter("ModifiedByUserId").Value);
        }

        public static List<string> GetThemeList(SessionInfo sessionInfo, string themeType)
        {
            return new List<string>();
        }

        public static bool ExistTheme(SessionInfo sessionInfo, string themeType, string themeName)
        {
            return false;
        }

        public static string GetThemeXml(SessionInfo sessionInfo, string themeType, string themeName)
        {
            return null;
        }

        public static string SaveTheme(SessionInfo sessionInfo, string themeType, string themeName, string themeXml)
        {
            return null;
        }

        private static IReportEntityDbContext GetUserDbContext(SessionInfo sessionInfo)
        {
            var districtIdParam = sessionInfo.SetupData.Parameters.GetParameter("DistrictId");
            var userIdParam = sessionInfo.SetupData.Parameters.GetParameter("UserId");

            return ReportEntityDbContextFactory.Create(new Guid(districtIdParam.Value), new Guid(userIdParam.Value));
        }

        private static IReportEntityDbContext GetDbContext(SessionInfo sessionInfo)
        {
            var districtIdParam = sessionInfo.SetupData.Parameters.GetParameter("DistrictId");

            return ReportEntityDbContextFactory.Create(new Guid(districtIdParam.Value));
        }

        private static string WriteXml(List<ReportEntity> entities, XmlDocument xmlDoc, XmlNode root)
        {
            foreach (var entity in entities)
            {
                XmlNode node = xmlDoc.CreateElement("entity");
                root.AppendChild(node);

                var attribute = xmlDoc.CreateAttribute("id");
                attribute.Value = entity.Id.ToString();
                node.Attributes.Append(attribute);

                XmlNode nameNode = xmlDoc.CreateElement("name");
                node.AppendChild(nameNode);
                nameNode.InnerText = entity.Name;

                XmlNode leafNode = xmlDoc.CreateElement("leaf_flag");
                node.AppendChild(leafNode);
                leafNode.InnerText = entity.IsLeafNode.ToString();

                XmlNode roNode = xmlDoc.CreateElement("readonly_flag");
                node.AppendChild(roNode);
                roNode.InnerText = entity.IsReadOnly.ToString();

                if (entity is Report)
                {
                    XmlNode descriptionNode = xmlDoc.CreateElement("description");
                    node.AppendChild(descriptionNode);
                    descriptionNode.InnerText = ((Report)entity).Description;
                }

                WriteXml(entity.Children, xmlDoc, node);
            }

            return xmlDoc.InnerXml;
        }

        private static bool IsMyReportsPath(string path)
        {
            return path.Split('\\', '"')[0] == "My Reports";
        }
    }
}