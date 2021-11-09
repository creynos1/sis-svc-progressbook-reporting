namespace ProgressBook.Reporting.ExagoIntegration.VendorExtract
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using WebReports.Api.Common;

    public class SshKnownHostsManager
    {
        private readonly string _filename;

        public SshKnownHostsManager() : this(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ssh-known-hosts.json"))
        {
        }

        public SshKnownHostsManager(string knownHostsFile)
        {
            _filename = knownHostsFile;
            SshKnownHosts = new List<SshKnownHost>();
            Load();
        }

        public List<SshKnownHost> SshKnownHosts { get; private set; }

        public void Load()
        {
            if (!File.Exists(_filename)) return;
            var json = File.ReadAllText(_filename);
            var obj = Json.Deserialize(json, typeof(SshKnownHostsJsonObject)) as SshKnownHostsJsonObject;
            if (obj != null)
                SshKnownHosts = obj.SshKnownHosts;
        }

        public void Save()
        {
            var obj = new SshKnownHostsJsonObject { SshKnownHosts = SshKnownHosts };
            var json = Json.Serialize(obj);
            File.WriteAllText(_filename, json);
        }
    }
}