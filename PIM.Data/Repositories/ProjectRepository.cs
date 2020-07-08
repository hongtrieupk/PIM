using PIM.Data.Objects;
using PIM.Common.Models;
using System;

namespace PIM.Data.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        #region Methods
        /// <summary>
        /// search projects by Number, Name, Customer and status
        /// </summary>
        /// <param name="searchParam"></param>
        /// <returns>return list projects with pagination option</returns>
        public PagingResultModel<Project> SearchProject(SearchProjectParam searchParam)
        {
            PagingResultModel<Project> result = new PagingResultModel<Project>();
            var queryOver = _session.QueryOver<Project>();
            queryOver = searchParam.ProjectNumber.HasValue 
                    ? queryOver.Where(x => x.ProjectNumber == searchParam.ProjectNumber) : queryOver;

            queryOver = !string.IsNullOrWhiteSpace(searchParam.ProjectName)
                    ? queryOver.WhereRestrictionOn(x => x.ProjectName).IsLike($"%{searchParam.ProjectName}%") : queryOver;

            queryOver = !string.IsNullOrWhiteSpace(searchParam.Status)
                    ? queryOver.Where(x => x.Status == searchParam.Status) : queryOver;

            queryOver = !string.IsNullOrWhiteSpace(searchParam.Customer)
                    ? queryOver.WhereRestrictionOn(x => x.Customer).IsLike($"%{searchParam.Customer}%") : queryOver;

            queryOver = queryOver.OrderBy(x => x.ProjectNumber).Asc;

            result.TotalRecords = queryOver.RowCount();
            result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / searchParam.PageSize);
            int numberOfSkipItems = (searchParam.CurrentPage - 1) * searchParam.PageSize;
            result.Records = queryOver.Skip(numberOfSkipItems).Take(searchParam.PageSize).List();

            return result;
        }
        #endregion
    }
}
