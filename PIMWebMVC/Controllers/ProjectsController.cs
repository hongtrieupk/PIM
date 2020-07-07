﻿using AutoMapper;
using PIM.Business.Services;
using PIM.Common.Models;
using PIM.Common.SystemConfigurationHelper;
using PIM.Data.NHibernateConfiguration;
using PIM.Data.Objects;
using PIM.Data.Repositories;
using PIMWebMVC.Constants;
using PIMWebMVC.Models.Common;
using PIMWebMVC.Models.Projects;
using PIMWebMVC.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PIMWebMVC.Controllers
{
    public class ProjectsController : Controller
    {
        #region Fields
        private readonly IProjectService _projectService;
        private readonly IAppConfiguration _appConfiguration;
        private const string createSuccessfullyMessageKey = "CREATE_SUCCESS_MESSAGE";
        private const string updateSuccessfullyMessageKey = "UPDATE_SUCCESS_MASSAGE";
        #endregion
        #region Constructors
        public ProjectsController()
        {
            // TODO: apply IOC
            IApplicationDbContext dbContext = new ApplicationDbContext();
            _projectService = new ProjectService(new ProjectRepository(), dbContext);
            _appConfiguration = new AppConfiguration();
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            // the Index View will use those ViewBag's properties to show message if the Index view is redirect from Update or Create Action
            ViewBag.CREATE_SUCCESS_MESSAGE = TempData[createSuccessfullyMessageKey];
            ViewBag.UPDATE_SUCCESS_MASSAGE = TempData[updateSuccessfullyMessageKey];
            return View();
        }
        /// <summary>
        /// depend on the id parameter, this will be a Add new Project page if id is null and Update Project page if id has value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddUpdate(int? id)
        {
            ProjectModel initProject = new ProjectModel();
            if (id.HasValue)
            {
                Project projectFromDb = _projectService.GetProjectById(id.Value);
                if (projectFromDb == null)
                {
                    throw new HttpException((int)HttpStatusCode.NotFound, PIMResource.ERROR_NOT_FOUND_MESSAGE);
                }
                initProject = Mapper.Map<ProjectModel>(projectFromDb);
            }
            return View(initProject);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdate(ProjectModel projectModel)
        {
            if (!ValidateProjectModel(projectModel))
            {
                return View(projectModel);
            }
            Project project = Mapper.Map<Project>(projectModel);

            if (projectModel.ProjectID.HasValue) // Update project
            {
                _projectService.UpdateProject(project);
                TempData[updateSuccessfullyMessageKey] = PIMResource.MESSAGE_UPDATE_PROJECT_SUCCESS;
            }
            else // Create new project
            {
                _projectService.CreateProject(project);
                TempData[createSuccessfullyMessageKey] = PIMResource.MESSAGE_CREATE_PROJECT_SUCCESS;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SearchProjects(SearchProjectParam searchParam)
        {
            if (!searchParam.IsValidParam)
            {
                throw new ArgumentNullException($"{PIMResource.MESSAGE_INVALID_CRITERIA}");
            }
            searchParam.PageSize = searchParam.PageSize > 0 ? searchParam.PageSize : _appConfiguration.DefaultPageSize;
            PagingResultModel<Project> projectsPagingResult = _projectService.SearchProject(searchParam);
            var result = new ProjectsPaginationResult
            {
                Projects = Mapper.Map<IList<ProjectModel>>(projectsPagingResult.Records),
                CurrentPage = searchParam.CurrentPage,
                TotalPages = projectsPagingResult.TotalPages,
                TotalRecords = projectsPagingResult.TotalRecords,
            };
            return PartialView("_ProjectsPaginationPartial", result);
        }

        [HttpPost]
        public ActionResult DeleteProjectByIds(IList<int> projectIds)
        {
            _projectService.DeleteProjectsByIds(projectIds);
            return Json(new ActionResultModel() { IsSuccess = true, Message = PIMResource.MESSAGE_DELETE_PROJECTS_SUCCESS });

        }

        public ActionResult PaginationBar(PaginationBarModel dataModel)
        {
            return PartialView("_PaginationBarPartial", dataModel);
        }

        private bool ValidateProjectModel(ProjectModel project)
        {
            bool isValid = true;
            if (project.IsValidMadatoryFields())
            {
                ModelState.Remove(ErrorActionsConstant.SUM_REQUIRED_ERROR_FIELD_NAME);
            }
            else
            {
                isValid = false;
                ModelState.AddModelError(ErrorActionsConstant.SUM_REQUIRED_ERROR_FIELD_NAME, PIMResource.MESSAGE_MANDATORY_FIELD);
            }

            if (project.IsValidEndDate())
            {
                ModelState.Remove(nameof(project.EndDate));
            }
            else
            {
                isValid = false;
                ModelState.AddModelError(nameof(project.EndDate), PIMResource.MESSAGE_END_DATE_MUST_GREATER_THAN_START_DATE);
            }

            bool isDuplicatedProjectNumber = _projectService.IsDuplicateProjectNumber(project.ProjectID, project.ProjectNumber.Value);
            if (isDuplicatedProjectNumber)
            {
                isValid = false;
                ModelState.AddModelError(nameof(project.ProjectNumber), PIMResource.MESSAGE_DUPLICATED_PROJECT_NUMBER);
            }
            else
            {
                if (project.ProjectNumber.HasValue)
                {
                    ModelState.Remove(nameof(project.ProjectNumber));
                }
            }

            return isValid;
        }
        #endregion
    }
}