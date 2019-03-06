namespace ProgressBook.Reporting.Web
{
    using System.Web.Mvc;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using ProgressBook.Master.Connections;
    using ProgressBook.Reporting.ExagoIntegration;

    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IControllerFactory>()
                         .ImplementedBy<WindsorControllerFactory>()
                         .LifestyleSingleton(),
                Classes.FromAssemblyContaining<IExagoConfigurationHelper>()
                       .Pick()
                       .WithServiceDefaultInterfaces()
                       .LifestylePerWebRequest(),
                Component.For<IDistrictProfileService>()
                         .UsingFactoryMethod(CreateDistrictProfileService)
                         .LifestylePerWebRequest(),
                Classes.FromThisAssembly()
                       .BasedOn<IController>()
                       .LifestylePerWebRequest()
            );
        }

        private IDistrictProfileService CreateDistrictProfileService(IKernel kernel)
        {
            var connectionStrings = kernel.Resolve<IConnectionStrings>();
            var districtProfileService = new DistrictProfileService(connectionStrings.PbMaster);
            kernel.ReleaseComponent(connectionStrings);
            return districtProfileService;
        }
    }
}