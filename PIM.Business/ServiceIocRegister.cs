using Autofac;
using PIM.Business.Services;
using PIM.Data;

namespace PIM.Business
{
    public class ServiceIocRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DataIocRegister());
            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();
        }
    }
}
