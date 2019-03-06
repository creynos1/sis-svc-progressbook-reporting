namespace ProgressBook.Reporting.Client
{
    using System;
    using SoftwareAnswers.ProgressBook.IdentityServer.Client.CentralAdmin.Providers;

    public interface IExagoUri
    {
        Uri GetExagoUri();
    }

    public class ExagoUri : IExagoUri
    {
        public Uri GetExagoUri()
        {
            return SettingsProvider.Instance.ReportingUrl;
        }
    }
}