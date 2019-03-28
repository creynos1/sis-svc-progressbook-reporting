namespace ProgressBook.Reporting.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using ProgressBook.Reporting.Data.Entities;

    public class ReportEntityManager
    {
        private static readonly char[] PathSeparators = { '\\', '/' };
        private readonly IReportEntityDbContext _dbContext;

        public ReportEntityManager(IReportEntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ReportEntity> GetAll()
        {
            List<ReportEntity> list = _dbContext.ReportEntities.ToList().Where(x => (x is Report) || (x is Folder)).ToList();

            var orderedList = new List<ReportEntity>();

            foreach (ReportEntity item in list.OrderBy(x => x.ParentId).ToArray())
            {
                if (!item.ParentId.HasValue)
                {
                    orderedList.Add(item);
                }
                else
                {
                    break;
                }
            }

            SortEntities(orderedList);

            return orderedList;
        }

        public T GetByPath<T>(string path) where T : ReportEntity
        {
            string[] pathParts = path.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            Guid? parentId = GetParentFolderId(pathParts);
            string entityName = pathParts.Last();

            T entity = _dbContext.Set<T>().SingleOrDefault(x => (parentId.HasValue ? x.ParentId == parentId : x.ParentId == null) && x.Name == entityName);

            return entity;
        }

        public T GetById<T>(Guid id) where T : ReportEntity
        {
            T entity = _dbContext.Set<T>().SingleOrDefault(x => (x.Id == id));

            return entity;
        }

        public void AddFolder(string folderName)
        {
            string[] pathParts = folderName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            Guid? parentId = GetParentFolderId(pathParts);
            string entityName = pathParts.Last();

            _dbContext.Folders.Add(new Folder { ParentId = parentId, Name = entityName });
        }

        public void DeleteFolder(string folderName)
        {
            var entity = GetByPath<ReportEntity>(folderName);

            _dbContext.ReportEntities.Remove(entity);
        }

        public void RenameFolder(string oldFolderName, string newFolderName)
        {
            var folder = GetByPath<Folder>(oldFolderName);

            string[] pathParts = newFolderName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            string newName = pathParts.Last();

            folder.Name = newName;
        }

        public void MoveFolder(string oldFolderName, string newFolderName)
        {
            var folder = GetByPath<Folder>(oldFolderName);
            Guid? newParentId = GetParentFolderId(newFolderName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries));

            if (folder.Id == newParentId)
            {
                throw new Exception("Error in MoveFolder: Tried to move a folder to be its own parent");
            }

            folder.ParentId = newParentId;
            folder.Parent = null;
        }

        public void SaveReport(string reportName, string reportXml, Guid modifiedByUserId)
        {
            var report = GetByPath<Report>(reportName);

            string description = null;

            var reportDoc = new XmlDocument();
            reportDoc.LoadXml(reportXml);
            XmlNode descriptionNode = reportDoc.SelectSingleNode("/report/main/description");

            if (descriptionNode != null)
                description = descriptionNode.InnerText;

            if (report != null)
            {
                report.Content = reportXml;
                report.Description = description;
                report.ModifiedBy = modifiedByUserId;
            }
            else
            {
                string[] pathParts = reportName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
                Guid? parentId = GetParentFolderId(pathParts);
                string entityName = pathParts.Last();

                _dbContext.Reports.Add(new Report { ParentId = parentId, Name = entityName, Content = reportXml, Description = description, ModifiedBy = modifiedByUserId });
            }
        }

        public void DeleteReport(string reportName)
        {
            var report = GetByPath<Report>(reportName);

            if (report != null)
            {
                _dbContext.Reports.Remove(report);
            }
        }

        public Template GetTemplateById(Guid templateId)
        {
            var template = GetById<Template>(templateId);

            return template;
        }

        public void DeleteTemplateById(Guid templateId)
        {
            var template = GetById<Template>(templateId);

            if (template != null)
            {
                _dbContext.Templates.Remove(template);
            }
        }

        public void RenameReport(string oldReportName, string newReportName, Guid modifiedByUserId)
        {
            var report = GetByPath<Report>(oldReportName);

            string[] oldPathParts = oldReportName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            string oldFolder = string.Join("\\", oldPathParts.Take(oldPathParts.Length - 1).ToArray());

            string[] newPathParts = newReportName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            string newFolder = string.Join("\\", newPathParts.Take(newPathParts.Length - 1).ToArray());

            report.Name = newPathParts.Last();
            report.ModifiedBy = modifiedByUserId;

            if (oldFolder != newFolder)
            {
                var parentFolder = GetByPath<Folder>(newFolder);
                report.Parent = parentFolder;
                report.ParentId = parentFolder.Id;
            }
        }

        public void SaveTemplateForReport(string reportName, string templateName, string templateDescription, byte[] templateData, Guid modifiedByUserId)
        {
            var report = GetByPath<Report>(reportName);

            var template = GetByPath<Template>(reportName + "\\" + templateName);
            if (template != null)
            {
                template.BinaryContent = templateData;
                template.ModifiedBy = modifiedByUserId;
            }
            else
            {
                template = new Template();
                template.ParentId = report.Id;
                template.Name = templateName;
                template.Description = templateDescription;
                template.BinaryContent = templateData;
                template.ModifiedBy = modifiedByUserId;
                _dbContext.Templates.Add(template);
            }
        }

        public byte[] GetTemplateForReport(string reportName, string templateName)
        {
            var template = GetByPath<Template>(reportName + "\\" + templateName);

            if (template != null)
            {
                return template.BinaryContent;
            }

            return null;
        }

        public List<string> GetTemplateListForReport(string reportName)
        {
            var list = new List<string>();
            var report = GetByPath<Report>(reportName);

            if (report != null)
            {
                list = _dbContext.Templates
                                 .Where(x => x.ParentId == report.Id)
                                 .Select(x => x.Name).ToList();
            }

            return list;
        }

        public List<Template> GetTemplatesForReport(Report report)
        {
            var list = new List<Template>();

            if (report != null)
            {
                list = _dbContext.Templates
                                 .Where(x => x.ParentId == report.Id)
                                 .ToList();
            }

            return list;
        }

        public List<Template> GetTemplatesForReport(string reportName)
        {
            var list = new List<Template>();
            var report = GetByPath<Report>(reportName);

            if (report != null)
            {
                list = _dbContext.Templates
                                 .Where(x => x.ParentId == report.Id)
                                 .ToList();
            }

            return list;
        }

        private Guid? GetParentFolderId(string[] pathParts)
        {
            if (pathParts.Length < 2)
                return null;

            Guid? parentId = null;

            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                string name = pathParts[i];
                List<Guid> list = _dbContext.ReportEntities
                                            .Where(x =>
                                                       x.Name == name &&
                                                       (parentId.HasValue ? x.ParentId == parentId : x.ParentId == null)) //&&
                                            //!x.IsLeafNode)
                                            .Select(x => x.Id)
                                            .ToList();
                if (list.Count == 0)
                    parentId = null;
                else
                    parentId = list[0];
            }

            return parentId;
        }

        private void SortEntities(List<ReportEntity> entities)
        {
            entities.Sort((a, b) =>
            {
                if (a.IsLeafNode && !b.IsLeafNode)
                    return -1;
                if (b.IsLeafNode && !a.IsLeafNode)
                    return 1;
                return a.Name.CompareTo(b.Name);
            });

            foreach (ReportEntity entity in entities)
            {
                SortEntities(entity.Children);
            }
        }

        public List<Template> GetAllTemplatesInFolder(Folder folder)
        {
            var list = new List<Template>();

            foreach (ReportEntity child in folder.Children)
            {
                if (child is Folder)
                {
                    list.AddRange(GetAllTemplatesInFolder((Folder)child));
                }
                else if (child is Report)
                {
                    list.AddRange(GetTemplatesForReport((Report)child));
                }
            }

            return list;
        }

        public List<Template> GetAllTemplatesInPath(string folderPath)
        {
            List<ReportEntity> all = GetAll();
            var list = new List<Template>();
            var folder = GetByPath<Folder>(folderPath);

            if (folder != null)
            {
                list = GetAllTemplatesInFolder(folder);
            }

            return list;
        }

        public List<Report> GetAllReportsInFolder(Folder folder, bool includeSubFolders = true)
        {
            var list = new List<Report>();

            foreach (ReportEntity child in folder.Children)
            {
                if (includeSubFolders && child is Folder)
                {
                    list.AddRange(GetAllReportsInFolder((Folder)child));
                }
                else if (child is Report)
                {
                    list.Add((Report)child);
                }
            }

            return list;
        }

        public List<Report> GetAllReportsInPath(string folderPath, bool includeSubFolders = true)
        {
            List<ReportEntity> all = GetAll();
            var list = new List<Report>();
            var folder = GetByPath<Folder>(folderPath);

            if (folder != null)
            {
                list = GetAllReportsInFolder(folder, includeSubFolders);
            }

            return list;
        }

        public List<Report> GetAllReportsInPath(IEnumerable<string> folderPaths, bool includeSubFolders = true)
        {
            List<ReportEntity> all = GetAll();
            var list = new List<Report>();

            foreach (var folderName in folderPaths)
            {
                string[] pathParts = folderName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
                var folder = all.FirstOrDefault(x => x.Path == pathParts[0]) as Folder;
                if (folder == null) continue;

                if (pathParts.Length == 1)
                {
                    list.AddRange(GetAllReportsInFolder(folder, includeSubFolders));
                    continue;
                }

                if (pathParts.Length > 1)
                {
                    var subFolder = folder.Children.FirstOrDefault(x => x.Path == folderName) as Folder;
                    if (subFolder != null)
                    {
                        list.AddRange(GetAllReportsInFolder(subFolder, includeSubFolders));
                    }
                }
            }

            return list;
        }

        public List<String> GetNodeMap(string folderPath)
        {
            var report = GetByPath<Report>(folderPath);

            var bookmarkList = new List<String>();

            if (report != null)
            {
                XmlNodeList nodeList = null;
                var doc = new XmlDocument();
                doc.LoadXml(report.Content);

                XmlNode root = doc.DocumentElement;
                nodeList = root.SelectNodes("/report/pdfmap/field_name");

                foreach (XmlNode node in nodeList)
                {
                    bookmarkList.Add(node.InnerText);
                }
            }

            return bookmarkList;
        }
    }
}