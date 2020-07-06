using PIM.Data.Objects;
using NHibernate;
using System.Linq;
using PIM.Common.Models;
using System;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Collections;
using System.Collections.Generic;

namespace PIM.Data.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        #region Constructors
        public ProjectRepository(ISession session) : base(session)
        {
        }
        #endregion
        #region Methods
        public void DeleteProjectByIds(IList<int> projectIds)
        {
            _session.Query<Project>()
                    .Where(p => projectIds.Contains(p.ProjectID)).Delete();
        }
        public PagingResultModel<Project> SearchProject(SearchProjectParam searchParam)
        {
            PagingResultModel<Project> result = new PagingResultModel<Project>();
            var query = _session.Query<Project>();
            query = searchParam.ProjectNumber.HasValue ? query.Where(x => x.ProjectNumber == searchParam.ProjectNumber) : query;

            query = !string.IsNullOrWhiteSpace(searchParam.ProjectName)
                    ? query.Where(x => x.ProjectName.Contains(searchParam.ProjectName)) : query;

            query = !string.IsNullOrWhiteSpace(searchParam.Status)
                    ? query = query.Where(x => x.Status == searchParam.Status) : query;

            query = !string.IsNullOrWhiteSpace(searchParam.Customer)
                        ? query.Where(x => x.Customer.Contains(searchParam.Customer)) : query;

            result.TotalRecords = query.Count();
            result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / searchParam.PageSize);
            query = query.Skip((searchParam.CurrentPage - 1) * searchParam.PageSize)
                        .Take(searchParam.PageSize);
            query = query.OrderBy(x => x.ProjectNumber);

            result.Records = query.ToList();
            return result;
        }
        #endregion
    }
}
