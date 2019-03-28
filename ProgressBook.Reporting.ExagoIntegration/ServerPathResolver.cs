namespace ProgressBook.Reporting.ExagoIntegration
{
    using System.Web;
    using System.Web.Hosting;

    public interface IServerPathResolver
    {
        string MapPath(string path);
    }

    public class ServerPathResolver : IServerPathResolver
    {
        public string MapPath(string path)
        {
            return HostingEnvironment.MapPath(path);
        }
    }
}