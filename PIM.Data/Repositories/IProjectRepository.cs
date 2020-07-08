
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
    }
}
