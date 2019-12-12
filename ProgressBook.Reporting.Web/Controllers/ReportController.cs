using ProgressBook.Shared.Security.Infrastructure.Factories;
using ProgressBook.Shared.Security.Infrastructure.Services;

namespace ProgressBook.Reporting.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using ProgressBook.Reporting.Data;
    using ProgressBook.Reporting.Data.Repositories;
    using ProgressBook.Reporting.ExagoIntegration;
    using ProgressBook.Reporting.SharedModels;
    using ProgressBook.Shared.Security.Factories;
    using ProgressBook.Shared.Security.Services;

    [Authorize]
    public class ReportController : Controller
    {
        private readonly IAuthorizedSchoolService _authorizedSchoolService;
        private readonly IExagoSettings _exagoSettings;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public ReportController(IExagoSettings exagoSettings)
        {
            _exagoSettings = exagoSettings;
            _authorizedSchoolService = new AuthorizedSchoolService(new PlaceRepository());
            _userAuthorizationService = DefaultUserAuthorizationServiceFactory.Instance.Create();
        }

        [HttpPost]
        public ActionResult AdHocReportDesignerUrl(UserContext userContext)
        {
            return Json(GetReportUrl(new ReportUrlModel { UserContext = userContext }, false));
        }

        [HttpPost]
        public ActionResult ExecuteReport(ReportInformation model)
        {
            using (
                var dbContext = ReportEntityDbContextFactory.Create(model.UserContext.DistrictId,
                                                                    model.UserContext.UserId))
            {
                var helper = new ReportHelper(_exagoSettings,
                                              dbContext,
                                              _userAuthorizationService,
                                              _authorizedSchoolService);

                helper.ConfigureDefaultParameters(model.UserContext);
                
                var report = helper.ConfigureReport(model);
                var reportUrl = Url.Content("~/Exago").TrimEnd('/') + "/" + helper.GetAdHocReportingUrl();

                if (!string.IsNullOrEmpty(model.FileDownloadToken))
                {
                    ControllerContext.HttpContext.Response.AppendCookie(
                        new HttpCookie("FileDownloadToken", model.FileDownloadToken));
                }

                var data = report.GetExecuteData();

                if (report.Exception != null)
                {
                    throw report.Exception;
                }

                if (data == null || data.Length == 0)
                {
                    if (report.IsTemplateOutput)
                    {
                        throw new Exception(
                           $"Unable to merge {report.Name} report data with template. Verify report template contains valid bookmarks.");
                    }
                    else
                    {
                        throw new Exception(
                            $"No data returned for {report.Name} report");
                    }
                }


                var contentType = helper.GetExportContentType(model.ExportType);
                var fileExtension = helper.GetExportFileExtension(model.ExportType);
                return File(data, contentType, $"{report.Name.Split('\\').Last()}.{fileExtension}");
            }
        }

        [HttpPost]
        public ActionResult ReportExecutionUrl(ReportUrlModel model)
        {
            return Json(GetReportUrl(model, true));
        }

        private ReportUrl GetReportUrl(ReportUrlModel model, bool configureReport)
        {
            var userContext = model.UserContext;
            using (var dbContext = ReportEntityDbContextFactory.Create(userContext.DistrictId, userContext.UserId))
            {
                var helper = new ReportHelper(_exagoSettings,
                                              dbContext,
                                              _userAuthorizationService,
                                              _authorizedSchoolService);

                helper.ConfigureSecurity(userContext);

                if (configureReport)
                {
                    // configures to executes report
                    var reportInformation = new ReportInformation
                    {
                        ReportName = model.ReportName,
                        UserContext = userContext
                    };

                    helper.ConfigureDefaultParameters(userContext);
                    helper.ConfigureReport(reportInformation);
                }

                var reportUrl = Url.Content("~/Exago").TrimEnd('/') + "/" + helper.GetAdHocReportingUrl();
                return new ReportUrl
                       {
                           Url = GetAbsoluteUrl(reportUrl)
                       };
            }
        }

        private string GetAbsoluteUrl(string url)
        {
            var parts = url.Split('?');
            var path = parts[0];
            var query = parts[1];
            var uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
                             {
                                 Path = path,
                                 Query = query
                             };

            return uriBuilder.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userAuthorizationService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}