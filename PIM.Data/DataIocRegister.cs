using Autofac;
using PIM.Data.NHibernateConfiguration;
using PIM.Data.Repositories;

namespace PIM.Data
{
    public class DataIocRegister : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationDbContext>().As<IApplicationDbContext>().InstancePerRequest();
            builder.RegisterType<ProjectRepository>().As<IProjectRepository>().InstancePerLifetimeScope();
        }
    }
}
