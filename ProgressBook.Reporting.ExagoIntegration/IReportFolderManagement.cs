namespace ProgressBook.Reporting.ExagoIntegration
{
    using System.Collections.Generic;
    using WebReports.Api.Common;

    public interface IReportFolderManagement
    {
        string GetReportListXml(SessionInfo sessionInfo);
        string AddFolder(SessionInfo sessionInfo, string companyId, string userId, string folderName);
        string DeleteFolder(SessionInfo sessionInfo, string companyId, string userId, string folderName);
        string RenameFolder(SessionInfo sessionInfo, string companyId, string userId, string oldName, string newName);
        bool ExistFolder(SessionInfo sessionInfo, string companyId, string userId, string folderName);
        string GetReportXml(SessionInfo sessionInfo, string reportName);
        string SaveReport(SessionInfo sessionInfo, string companyId, string userId, string reportName, string reportXml);
        string DuplicateReport(SessionInfo sessionInfo, string companyId, string userId, string reportName, string reportXml);
        string DeleteReport(SessionInfo sessionInfo, string companyId, string userId, string reportName);
        string RenameReport(SessionInfo sessionInfo, string companyId, string userId, string reportName, string newReportName);
        List<string> GetTemplateList(SessionInfo sessionInfo, string companyId, string userId);
        byte[] GetTemplate(SessionInfo sessionInfo, string companyId, string userId, string templateName);
        string SaveTemplate(SessionInfo sessionInfo, string companyId, string userId, string templateName, byte[] templateData);
        List<string> GetThemeList(SessionInfo sessionInfo, string companyId, string userId, string themeType);
        bool ExistTheme(SessionInfo sessionInfo, string companyId, string userId, string themeType, string themeName);
        string GetThemeXml(SessionInfo sessionInfo, string companyId, string userId, string themeType, string themeName);
        string SaveTheme(SessionInfo sessionInfo, string companyId, string userId, string themeType, string themeName, string themeXml);
    }
}