using PIM.Common.Models;
using PIM.Data.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIM.Business.Services
{
    public interface IProjectService
    {
        Task<int> CreateProjectAsync(Project newProject);
        Task UpdateProjectAsync(Project project);
        Task<Project> GetProjectByIdAsync(int id);
        /// <summary>
        /// search projects with pagination option
        /// </summary>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        PagingResultModel<Project> SearchProject(SearchProjectParam searchParam);

        bool IsDuplicateProjectNumber(int? projectId, int newProjectNumber);
    }
}
