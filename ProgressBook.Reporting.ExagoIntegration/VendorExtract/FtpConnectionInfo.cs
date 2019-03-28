namespace ProgressBook.Reporting.ExagoIntegration.VendorExtract
{
    using System;
    using WinSCP;

    public class FtpConnectionInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RemoteDirectory { get; set; }

        public string OutputFileName { get; set; }

        public TestConnectionResult TestConnection()
        {
            try
            {
                var sessionOptions = new SessionOptions
                {
                    HostName = Host,
                    UserName = Username,
                    Password = Password,
                    PortNumber = Port
                };
                switch (ProtocolType)
                {
                    case ProtocolType.Sftp:
                        TestConnectionViaSftp(sessionOptions);
                        break;

                    case ProtocolType.Ftp:
                    case ProtocolType.FtpExplicitTls:
                    case ProtocolType.FtpImplicitTls:
                        TestConnectionViaFtp(sessionOptions);
                        break;
                }
            }
            catch (Exception ex)
            {
                return new TestConnectionResult {Error = ex.Message, Result = false};
            }

            return new TestConnectionResult {Result = true};
        }

        private void TestConnectionViaSftp(SessionOptions sessionOptions)
        {
            sessionOptions.Protocol = Protocol.Sftp;
            using (var session = new Session())
            {
                var fingerprint = session.ScanFingerprint(sessionOptions);
                sessionOptions.SshHostKeyFingerprint = fingerprint;
                session.Open(sessionOptions);
            }
        }

        private void TestConnectionViaFtp(SessionOptions sessionOptions)
        {
            sessionOptions.Protocol = Protocol.Ftp;
            if (ProtocolType == ProtocolType.FtpExplicitTls)
            {
                sessionOptions.FtpSecure = FtpSecure.Explicit;
            }

            if (ProtocolType == ProtocolType.FtpImplicitTls)
            {
                sessionOptions.FtpSecure = FtpSecure.Implicit;
            }

            using (var session = new Session())
            {
                session.Open(sessionOptions);
            }
        }
    }

    public class TestConnectionResult
    {
        public bool Result { get; set; }

        public string Error { get; set; }
    }
}