
using PIM.Data.Objects;
using System.Linq;
using PIM.Common.Models;
using System.Collections.Generic;

namespace PIM.Data.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        /// <summary>
        /// search projects by Number, Name, Customer and status
        /// </summary>
        /// <param name="searchParam"></param>
        /// <returns>return list projects with pagination option</returns>
        PagingResultModel<Project> SearchProject(SearchProjectParam searchParam);
        /// <summary>
        /// delete multiple projects by provided ProjectIds
        /// </summary>
        /// <param name="projectIds"></param>
        void DeleteProjectsByIds(IList<int> projectIds);
    }
}
