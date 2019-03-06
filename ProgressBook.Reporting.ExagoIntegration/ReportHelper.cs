namespace ProgressBook.Reporting.ExagoIntegration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using ProgressBook.Reporting.Data;
    using ProgressBook.Reporting.Data.Repositories;
    using ProgressBook.Shared.Security;
    using ProgressBook.Shared.Security.Enums;
    using ProgressBook.Shared.Security.Factories;
    using ProgressBook.Shared.Security.Models;
    using ProgressBook.Shared.Security.Services;
    using WebReports.Api;
    using WebReports.Api.Common;
    using WebReports.Api.Reports;
    using WebReports.Api.Roles;
    using ProgressBook.Reporting.SharedModels;
    using Folder = ProgressBook.Reporting.Data.Entities.Folder;

    public class ReportHelper
    {
        private const string PendingReportsFolderName = "Pending Reports";
        private const string MyReportsFolderName = "My Reports";
        private readonly Api _api;
        private readonly Dictionary<string, object> _apiParameters = new Dictionary<string, object>();
        private readonly IAuthorizedSchoolService _authorizedSchoolService;
        private readonly IUserAuthorizationService _authService;
        private readonly IReportEntityDbContext _reportDbContext;
        private string _fileExtension;

        public ReportHelper(IExagoSettings exagoSettings,
                            IReportEntityDbContext reportDbContext,
                            IUserAuthorizationService authService,
                            IAuthorizedSchoolService authorizedSchoolService)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            _api = new Api(exagoSettings.ExagoInstallPath);
            _reportDbContext = reportDbContext;
            Thread.CurrentThread.CurrentCulture = currentCulture;
            _authService = authService;
            _authorizedSchoolService = authorizedSchoolService;
        }

        private void SetApiParameterIfExists(string name, object value)
        {
            if (_api.Parameters.GetParameter(name) != null)
            {
                _api.Parameters.GetParameter(name).Value = value.ToString();
                _apiParameters[name] = value;
            }
            else
            {
                var logger = Logger.Instance(GetType().ToString());
                logger.Warn($"API parameter \"{name}\" not found in configuration.");
            }
        }
        
        public string GetExportContentType(ExportType exportType)
        {
            switch (exportType)
            {
                case ExportType.Csv:
                    return "text/csv";
                case ExportType.Excel:
                    return _api.SetupData.General.ExcelExportTarget == wrExcelExportTarget.v2003
                               ? "application/vnd.ms-excel"
                               : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ExportType.Html:
                    return "text/html";
                case ExportType.Pdf:
                    return "application/pdf";
                case ExportType.Rtf:
                case ExportType.Word:
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default:
                    throw new Exception("Unsupported exportType: " + exportType);
            }

        }

        public string GetExportFileExtension(ExportType exportType)
        {
            if (_fileExtension != null)
            {
                return _fileExtension;
            }

            switch (exportType)
            {
                case ExportType.Csv:
                    return "csv";
                case ExportType.Excel:
                    return _api.SetupData.General.ExcelExportTarget == wrExcelExportTarget.v2003 ? "xls" : "xlsx";
                case ExportType.Html:
                    return "html";
                case ExportType.Pdf:
                    return "pdf";
                case ExportType.Rtf:
                case ExportType.Word:
                    return "docx";
                default:
                    throw new Exception("Unsupported exportType: " + exportType);
            }

        }

        public Report ConfigureReport(ReportInformation model)
        {
            var report = _api.ReportObjectFactory.LoadFromRepository(model.ReportName) as Report;
            //report.ApplyFilters(config.Filters);

            // workaround:
            // - Use exago parameters for filtering since setting regular report filters via API doesn't work with GetExecuteData()
            //  -For this to work, you have to add tenant column/parameters to the necessary exago data objects.

            // don't allow parameters passed in on formModel to override already set teneant parameters (potential security risk if we did)
            foreach (var key in model.Parameters.Keys.Except(_apiParameters.Keys))
            {
                SetApiParameterIfExists(key, model.Parameters[key]);
            }

            report.ExportType = (wrExportType)model.ExportType;
            
            if (!string.IsNullOrEmpty(model.TemplateName))
            {
                report.PdfTemplate = model.TemplateName;
                var extension = Path.GetExtension(model.TemplateName);
                if (string.IsNullOrEmpty(extension))
                {
                    _fileExtension = extension.TrimStart('.');
                }

                AddVirtualBookmarks(report);
            }

            report.OpenNewWindow = false;
            report.ShowStatus = false;
            _api.ReportObjectFactory.SaveToApi(report);

            _api.Action = wrApiAction.ExecuteReport;

            return report;
        }

        private void AddVirtualBookmarks(Report report)
        {
            var data = new ReportEntityManager(_reportDbContext).GetTemplateForReport(report.Name, report.PdfTemplate);

            var docxTemplateBookmarkNames = TemplateHelper.ReadBookmarksFromWordDocument(data);

            var regex = new Regex("^(.*?)_x(\\d+)$");

            foreach (var tmplBookmark in docxTemplateBookmarkNames)
            {
                if (report.PdfMaps.Exists(x => x.FieldName == tmplBookmark))
                {
                    continue;
                }

                var match = regex.Match(tmplBookmark);

                if (match.Success)
                {
                    var bookmarkName = match.Groups[1].Value;

                    var reportBookmark = report.PdfMaps.FirstOrDefault(x => x.FieldName == bookmarkName);

                    if (reportBookmark != null)
                    {
                        var idx = report.PdfMaps.IndexOf(reportBookmark);

                        report.PdfMaps.Insert(idx + 1,
                                              new PdfMap
                                              {
                                                  FieldName = tmplBookmark,
                                                  CellId = reportBookmark.CellId
                                              });
                    }
                }
            }
        }

        public string GetAdHocReportingUrl()
        {
            return _api.GetUrlParamString("SAHome");
        }

        public void ConfigureSecurity(UserContext context)
        {
            ConfigureDefaultParameters(context);

            var reportEntityManager = new ReportEntityManager(_reportDbContext);
            var exagoRole = _api.Roles.Single(x => x.Id == "DynamicRole");
            exagoRole.Security.Folders.AllowManagement = false;
            exagoRole.Security.Folders.ReadOnly = true;
            exagoRole.Security.Folders.IncludeAll = true;
            // NOTE: inverted folder selection is the only way to allow access to a root folder but deny subfolder.

            exagoRole.Security.DataObjects.IncludeAll = _authService.IsAllowedSync(
                context.UserId,
                context.AuthorizationPlaceId,
                Resources.AdHocReports.AllDataObjects.Activities.View);

            var allowedReports = new List<Data.Entities.Report>();

            var allowPendingReports = false;

            // Folders
            ConfigureSystemFolders(exagoRole, allowedReports, reportEntityManager, ref allowPendingReports, context);
            
            EnsureMyReportsFolderExists(exagoRole);

            if (!exagoRole.Security.DataObjects.IncludeAll)
            {
                allowedReports.AddRange(reportEntityManager.GetAllReportsInPath("My Reports", false));
            }

            // District "Pending Reports" folder
            if (allowPendingReports)
            {
                EnsurePendingReportsFolderExists(exagoRole, context);
            }
            else
            {
                var pendingReportsFolder = exagoRole.Security.Folders.NewFolder();
                pendingReportsFolder.Name = PendingReportsFolderName;
                pendingReportsFolder.ReadOnly = true;
            }

            // multi-school auto filter
            ConfigureMultiSchoolAutoFilter(exagoRole, context);

            // data objects
            ConfigureDataObjects(exagoRole, allowedReports);
            
            // Scheduled Reports
            var allowReportScheduler = _authService.IsAllowedSync(context.UserId,
                                                                  context.AuthorizationPlaceId,
                                                                  Resources.AdHocReports.ReportScheduler.Activities.View);

            exagoRole.General.ShowScheduleReports = allowReportScheduler;
            exagoRole.General.ShowScheduleReportsEmail = allowReportScheduler;
            exagoRole.General.ShowScheduleReportsManager = allowReportScheduler;

            exagoRole.Activate(true);
        }

        public void ConfigureDefaultParameters(UserContext context)
        {
            SetApiParameterIfExists("DistrictId", context.DistrictId);
            SetApiParameterIfExists("UserId", context.UserId);
            SetApiParameterIfExists("userId", context.UserId + "_" + context.DistrictId);
            SetApiParameterIfExists("ModifiedByUserId", context.UserId);
            SetApiParameterIfExists("DistrictIrn", context.DistrictIrn);
        }
        
        private void ConfigureSystemFolders(Role exagoRole,
                                            List<Data.Entities.Report> allowedReports,
                                            ReportEntityManager reportEntityManager,
                                            ref bool allowPendingReports,
                                            UserContext context)
        {
            var reportTypes = GetReportTypes();

            foreach (var reportType in reportTypes)
            {
                var isViewAllowed = _authService.IsAllowedSync(context.UserId,
                                                               context.AuthorizationPlaceId,
                                                               reportType.ResourceId,
                                                               Activity.View);
                if (!isViewAllowed)
                {
                    var roleFolder = exagoRole.Security.Folders.NewFolder();
                    roleFolder.Name = GetReportTypeFolderName(reportType);
                    roleFolder.ReadOnly = true;
                }
                else
                {
                    var hasFullAccess = _authService.IsAllowedSync(context.UserId,
                                                                   context.AuthorizationPlaceId,
                                                                   reportType.ResourceId,
                                                                   Activity.Update);
                    if (hasFullAccess)
                    {
                        var roleFolder = exagoRole.Security.Folders.NewFolder();
                        roleFolder.Name = GetReportTypeFolderName(reportType);
                        roleFolder.ReadOnly = false;
                        allowPendingReports = true;
                    }

                    // For this top level security folder that the user has access to, turn on all Exago reports at the root level and nested. 
                    // The presence of top level security folder is enough to turn these Exago reports on.
                    // (Except for Admin folder- which is handled as its own security node) 
                    if (!exagoRole.Security.DataObjects.IncludeAll)
                    {
                        // Admin reports are handled separately, as they have a security node.
                        var reportRange = reportEntityManager.GetAllReportsInPath(GetReportTypeFolderName(reportType), true).Where(r => !r.Path.Contains("\\Admin\\")).ToList();
                        allowedReports.AddRange(reportRange);
                    }

                    // sub security node folders (only 1 level deep for now), this is for the "Admin" subfolders
                    foreach (var subReportType in reportType.Children)
                    {
                        var isSubReportTypeAllowed = _authService.IsAllowedSync(context.UserId,
                                                                                context.AuthorizationPlaceId,
                                                                                subReportType.ResourceId,
                                                                                Activity.View);

                        if (!isSubReportTypeAllowed)
                        {
                            var roleSubFolder = exagoRole.Security.Folders.NewFolder();
                            roleSubFolder.Name = GetReportTypeFolderName(subReportType);
                            roleSubFolder.ReadOnly = true;
                        }
                        else
                        {
                            if (!exagoRole.Security.DataObjects.IncludeAll)
                            {
                                allowedReports.AddRange(
                                    reportEntityManager.GetAllReportsInPath(GetReportTypeFolderName(subReportType),
                                                                            false));
                            }
                        }
                    }
                }
            }

            // hide the "SIS" folder
            var sisFolder = exagoRole.Security.Folders.NewFolder();
            sisFolder.Name = "SIS";
            sisFolder.ReadOnly = true;
        }

        private static List<ResourceTree> GetReportTypes()
        {
            var resourceTreeService = DefaultResourceTreeServiceFactory.Instance.Create();
            var reportTypesResourceTree =
                resourceTreeService.GetResourceTreeSync(Resources.AdHocReports.ReportTypes.Namespace);
            return reportTypesResourceTree.Children
                                          .OrderBy(x => x.DisplayName)
                                          .ToList();
        }

        private static string GetReportTypeFolderName(ResourceTree resource)
        {
            if (resource.Parent != null)
            {
                if (resource.Parent.ResourceName == Resources.AdHocReports.ReportTypes.Namespace)
                {
                    return resource.DisplayName;
                }
                return resource.Parent.DisplayName + "\\" + resource.ResourceName.Split('.').ToList().Last();
            }

            return null;
        }

        private void EnsureMyReportsFolderExists(Role exagoRole)
        {
            // has access to My Reports, create folder if doesn't already exist
            if (!_reportDbContext.Folders.Any(x => x.Name == MyReportsFolderName))
            {
                _reportDbContext.Folders.Add(new Folder {Name = MyReportsFolderName});
                _reportDbContext.SaveChanges();
            }

            var myReportsRoleFolder = exagoRole.Security.Folders.NewFolder();
            myReportsRoleFolder.Name = MyReportsFolderName;
            myReportsRoleFolder.ReadOnly = false;
        }

        private void EnsurePendingReportsFolderExists(Role exagoRole, UserContext context)
        {
            using (var districtDbContext = ReportEntityDbContextFactory.Create(context.DistrictId))
            {
                if (!districtDbContext.Folders.Any(x => x.Name == PendingReportsFolderName))
                {
                    districtDbContext.Folders.Add(new Folder {Name = PendingReportsFolderName});
                    districtDbContext.SaveChanges();
                }
            }

            var myReportsRoleFolder = exagoRole.Security.Folders.NewFolder();
            myReportsRoleFolder.Name = PendingReportsFolderName;
            myReportsRoleFolder.ReadOnly = false;
        }

        private void ConfigureMultiSchoolAutoFilter(Role exagoRole, UserContext context)
        {
            var allowedSchoolIds = _authorizedSchoolService.GetAllowedSchoolIds(context.UserId,
                                                                                context.DistrictId);
            var allowedSchoolCodes = _authorizedSchoolService.GetAllowedSchoolCodes(context.UserId,
                                                                                    context.DistrictId);

            foreach (var dataObject in _api.Entities)
            {
                var schoolId = dataObject.Columns.FirstOrDefault(x => x.Name == "SchoolId");

                if (schoolId != null)
                {
                    var schoolIdRoleFilter = exagoRole.Security.DataObjectRows.NewDataObjectRow();
                    schoolIdRoleFilter.ObjectName = dataObject.DbName;

                    var schoolFilter = string.Join(" ",
                                                   allowedSchoolIds.Select(
                                                       x => $"OR {dataObject.DbName}.SchoolId = '{x}'"));
                    schoolIdRoleFilter.FilterString = $"({dataObject.DbName}.SchoolId IS NULL {schoolFilter})";
                }

                var distBldg = dataObject.Columns.FirstOrDefault(x => x.Name == "_DistBldg");

                if (distBldg != null)
                {
                    var distBldgRoleFilter = exagoRole.Security.DataObjectRows.NewDataObjectRow();
                    distBldgRoleFilter.ObjectName = dataObject.DbName;
                    var distBldgFilter = string.Join(" ",
                                                     allowedSchoolCodes.Select(
                                                         x => $"OR {dataObject.DbName}._DistBldg = '{x}'"));
                    distBldgRoleFilter.FilterString = $"({dataObject.DbName}._DistBldg IS NULL {distBldgFilter})";
                }
            }
        }

        private static void ConfigureDataObjects(Role exagoRole,
                                                 IEnumerable<Data.Entities.Report> allowedReports)
        {
            if (exagoRole.Security.DataObjects.IncludeAll)
            {
                return;
            }

            var allowedDataObjectNames = allowedReports.SelectMany(r => r.DataObjects)
                                                       .Distinct()
                                                       .ToList();

            foreach (var allowedDataObjectName in allowedDataObjectNames)
            {
                var roleDataObject = exagoRole.Security.DataObjects.NewDataObject();
                roleDataObject.Name = allowedDataObjectName;
            }
        }
    }
}