using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using PIM.Data.Objects;

namespace PIM.Data.Maps
{
    public class ProjectMap : ClassMapping<Project>
    {
        public ProjectMap()
        {
            OptimisticLock(OptimisticLockMode.Version);
            Table("Projects");
            Id(x => x.ProjectID,
                m =>
                {
                    m.Column("ProjectID");
                    m.Generator(Generators.Identity);
                });
            Property(x => x.ProjectNumber, m =>
            {
                m.Column("ProjectNumber");
                m.NotNullable(true);
                m.Unique(true);
            });
            Property(x => x.ProjectName, m =>
            {
                m.Column("ProjectName");
                m.NotNullable(true);
                m.Length(100);
            });
            Property(x => x.Customer, m =>
            {
                m.Column("Customer");
                m.NotNullable(true);
                m.Length(500);
            });
            Property(x => x.Status, m =>
            {
                m.Column("Status");
                m.NotNullable(true);
                m.Length(3);
            });
            Property(x => x.StartDate, m =>
            {
                m.Column("StartDate");
                m.NotNullable(true);
            });
            Property(x => x.EndDate, m =>
            {
                m.Column("EndDate");
                m.NotNullable(false);
            });
            Version(x => x.Version, m =>
            {
                m.Column("Version");
                m.UnsavedValue(0);
            });
        }
    }
}
