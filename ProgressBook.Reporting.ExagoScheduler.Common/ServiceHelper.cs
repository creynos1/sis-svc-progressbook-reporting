namespace ProgressBook.Reporting.ExagoScheduler.Common
{
    using System;
    using System.Linq;
    using System.Management;
    using System.ServiceProcess;

    public class ServiceHelper
    {
        public static void StopService(string serviceName)
        {
            var service = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == serviceName);

            if (service != null && service.CanStop)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        public static void StartService(string serviceName)
        {
            var service = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == serviceName);

            if (service != null)
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
        }

        // from http://www.morgantechspace.com/2015/03/csharp-change-service-account-username-and-password.html#wmi
        public static void ChangeServiceAccount(string serviceName, string username, string password)
        {   
            var mgmntPath = string.Format("Win32_Service.Name='{0}'", serviceName);
            using (var service = new ManagementObject(new ManagementPath(mgmntPath)))
            {
                var accountParams = new object[11];
                accountParams[6] = username;
                accountParams[7] = password;
                var returnCode = (uint) service.InvokeMethod("Change", accountParams);

                if (returnCode != 0)
                {
                    throw new Exception(string.Format("Failed to change service account information. Error code: {0}", returnCode));
                }
            }
        }
    }
}
