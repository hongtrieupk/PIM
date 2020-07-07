using PIM.Common.Models;
using PIM.Data.Objects;
using System.Collections.Generic;

namespace PIM.Business.Services
{
    public interface IProjectService
    {
        int CreateProject(Project newProject);
        void UpdateProject(Project project);
        Project GetProjectById(int id);
        /// <summary>
        /// search projects with pagination option
        /// </summary>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        PagingResultModel<Project> SearchProject(SearchProjectParam searchParam);

        bool IsDuplicateProjectNumber(int? projectId, int newProjectNumber);
        void DeleteProjectsByIds(IList<int> projectIds);
    }
}
