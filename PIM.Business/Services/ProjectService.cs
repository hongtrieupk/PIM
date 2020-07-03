using PIM.Common.Models;
using PIM.Data.Objects;
using PIM.Data.UnitOfWorks;
using PIM.Data.UnitOfWorks.GenericTransactions;
using PIM.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIM.Business.Services
{
    public class ProjectService : IProjectService
    {
        #region Fields
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfwork;
        #endregion

        #region Constructors
        public ProjectService(IProjectRepository projectRepository, IUnitOfWork unitOfwork)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException("Can not inject a null project repository!");
            _unitOfwork = unitOfwork ?? throw new ArgumentNullException("Can not inject a null unitOfwork!");
        }
        #endregion

        #region Method
        public async Task<int> CreateProjectAsync(Project newProject)
        {
            using (IGenericTransaction transaction = _unitOfwork.BeginTransaction())
            {
                await _projectRepository.AddAsync(newProject);
                transaction.Commit();
            }
            return newProject.ProjectID;
        }

        public async Task UpdateProjectAsync(Project project)
        {
            using (IGenericTransaction transaction = _unitOfwork.BeginTransaction())
            {
                await _projectRepository.UpdateAsync(project);
                transaction.Commit();
            }
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }
        public PagingResultModel<Project> SearchProject(SearchProjectParam searchParam)
        {
            if (!searchParam.IsValidParam)
            {
                return null;
            }
            PagingResultModel<Project> result = _projectRepository.SearchProject(searchParam);
            return result;
        }
        public bool IsDuplicateProjectNumber(int? projectId, int newProjectNumber)
        {
            int countDuplicatedProjects =  _projectRepository.FilterBy(p => p.ProjectNumber == newProjectNumber && p.ProjectID != projectId).Count();
            return countDuplicatedProjects > 0;
        }
        #endregion
    }
}
