using Autofac;
using Autofac.Integration.Mvc;
using PIM.Business;
using PIM.Common.SystemConfigurationHelper;
using System.Web.Mvc;

namespace PIMWebMVC.App_Start
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModule(new ServiceIocRegister());
            builder.RegisterType<AppConfiguration>().As<IAppConfiguration>().SingleInstance();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}