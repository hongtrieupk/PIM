﻿using PIM.Common.Models;
using PIM.Data.Objects;
using PIM.Data.Repositories;
using System;
using System.Linq;
using PIM.Data.NHibernateConfiguration;
using PIM.Data.Repositories.GenericTransactions;
using System.Collections.Generic;
using PIM.Common.CustomExceptions;

namespace PIM.Business.Services
{
    public class ProjectService : IProjectService
    {
        #region Fields
        private readonly IProjectRepository _projectRepository;
        private readonly IApplicationDbContext _dbContext;
        #endregion

        #region Constructors
        public ProjectService(IProjectRepository projectRepository, IApplicationDbContext dbContext)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException("Can not inject a null project repository!");
            _dbContext = dbContext ?? throw new ArgumentNullException("Can not inject a null dbContext!");
        }
        #endregion

        #region Method
        public int CreateProject(Project newProject)
        {
            using (var session = _dbContext.OpenSession())
            {
                _projectRepository.SetSession(session);
                using (IGenericTransaction transaction = _dbContext.BeginTransaction())
                {
                    _projectRepository.Add(newProject);
                    transaction.Commit();
                }
            }
            return newProject.ProjectID;
        }
        public void UpdateProject(Project project)
        {
            using (var session = _dbContext.OpenSession())
            {
                _projectRepository.SetSession(session);
                using (IGenericTransaction transaction = _dbContext.BeginTransaction())
                {
                    _projectRepository.Update(project);
                    transaction.Commit();
                }
            }
        }
        public Project GetProjectById(int id)
        {
            using (var session = _dbContext.OpenSession())
            {
                _projectRepository.SetSession(session);
                return _projectRepository.GetById(id);
            }
        }
        public PagingResultModel<Project> SearchProject(SearchProjectParam searchParam)
        {
            if (!searchParam.IsValidParam)
            {
                return null;
            }
            using (var session = _dbContext.OpenSession())
            {
                _projectRepository.SetSession(session);
                PagingResultModel<Project> result = _projectRepository.SearchProject(searchParam);
                return result;
            }
        }
        public bool IsDuplicateProjectNumber(int? projectId, int newProjectNumber)
        {
            using (var session = _dbContext.OpenSession())
            {
                _projectRepository.SetSession(session);
                int countDuplicatedProjects = _projectRepository.FilterBy(p => p.ProjectNumber == newProjectNumber && p.ProjectID != projectId).Count();
                return countDuplicatedProjects > 0;
            }
        }
        public void DeleteProjectsByIds(IList<int> projectIds)
        {
            if (projectIds == null || projectIds.Count == 0)
            {
                throw new ArgumentNullException("Invalid projectIds");
            }
            using (var session = _dbContext.OpenSession())
            {
                _projectRepository.SetSession(session);
                int countProjectsInDb = _projectRepository.FilterBy(p => projectIds.Contains(p.ProjectID)).Count();
                if (countProjectsInDb != projectIds.Count)
                {
                    throw new ConcurrencyDbException("List deleted projects was modified!");
                }
                _projectRepository.DeleteProjectsByIds(projectIds);
            }
        }
        #endregion
    }
}
