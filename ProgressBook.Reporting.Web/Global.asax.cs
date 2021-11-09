namespace ProgressBook.Reporting.Web
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using log4net.Config;
    using ProgressBook.Master.Connections;
    using ProgressBook.Reporting.ExagoIntegration;
    using ProgressBook.Shared.Utilities.Logging;
    using ProgressBook.Shared.Utilities.Logging.Logging.LogProviders;
    using SoftwareAnswers.ProgressBook.Framework;
    using SoftwareAnswers.ProgressBook.IdentityServer.Consumer.Configuration;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            var searchableAssemblies = ConsumerBootstrap.GetSearchableAssemblies().ToList();
            FrameworkBootstrap.Initialize("ProgressBook.Reporting", searchableAssemblies);

            var container = new WindsorContainer().Install(FromAssembly.This());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel));

            IExagoSettings exagoSettings = new ExagoSettings(new ServerPathResolver());
            var connectionStrings = new ConnectionStrings(exagoSettings);

            var exagoConfigurationFileFactory = new ExagoConfigurationFileFactory(new ExagoEntitySerializer(),
                                                                                  exagoSettings, connectionStrings.QuickReports != null);

            var helper = new ExagoConfigurationHelper(exagoSettings,
                                                      exagoConfigurationFileFactory,
                                                      connectionStrings);
            helper.Configure();

            LogProvider.SetCurrentLogProvider(new Log4NetLogProvider());
            XmlConfigurator.Configure();
        }
    }
}