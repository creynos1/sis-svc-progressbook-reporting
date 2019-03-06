namespace ProgressBook.Reporting.ExagoScheduler.Config
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using ProgressBook.Reporting.ExagoScheduler.Common;

    internal static class Program
    {
        private static readonly string ResourcesNamespace = $"{typeof(Program).Assembly.GetName().Name}.Resources";
        private const string XmlConfigFileName = "eWebReportsScheduler.xml";

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ResolveEmbeddedAssemblies();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RunApp();
        }

        private static void RunApp()
        {
#if DEBUG
            var filePath = "C:\\Program Files\\Exago\\ExagoScheduler\\";
#else
            var filePath = AppDomain.CurrentDomain.BaseDirectory;
#endif
            try
            {
                var xmlConfigFilePath = Path.Combine(filePath, XmlConfigFileName);
                if (!File.Exists(xmlConfigFilePath))
                {
                    throw new FileNotFoundException(string.Format("Configuration file not found:\n{0}", xmlConfigFilePath));
                }

                Application.Run(new ConfigForm(filePath, null)
                {
                    StartPosition = FormStartPosition.CenterScreen,
                    Icon = Properties.Resources.GearIcon
                });
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private static void ResolveEmbeddedAssemblies()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var resourceName = string.Format("{0}.{1}.dll", ResourcesNamespace, new AssemblyName(args.Name).Name);

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    var assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
    }
}