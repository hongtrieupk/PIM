using AutoMapper;
using PIM.Business.Services;
using PIM.Common.Models;
using PIM.Common.SystemConfigurationHelper;
using PIM.Data.Objects;
using PIM.Data.Repositories;
using PIM.Data.UnitOfWorks;
using PIMWebMVC.Constants;
using PIMWebMVC.Models.Common;
using PIMWebMVC.Models.Projects;
using PIMWebMVC.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PIMWebMVC.Controllers
{
    public class ProjectsController : Controller
    {
        #region Fields
        private readonly IProjectService _projectService;
        private readonly IAppConfiguration _appConfiguration;
        #endregion
        #region Constructors
        public ProjectsController()
        {
            // TODO: apply IOC
            _projectService = new ProjectService(new ProjectRepository(UnitOfWork.CurrentSession), UnitOfWork.Current);
            _appConfiguration = new AppConfiguration();
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> AddUpdate(int? id)
        {
            ProjectModel initProject = new ProjectModel();
            if (id.HasValue)
            {
                Project projectFromDb = await _projectService.GetProjectByIdAsync(id.Value);
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
        public async Task<ActionResult> AddUpdate(ProjectModel projectModel)
        {
            if (!ValidateProjectModel(projectModel))
            {
                return View(projectModel);
            }
            Project project = Mapper.Map<Project>(projectModel);

            if (projectModel.ProjectID.HasValue) // Update project
            {
                await _projectService.UpdateProjectAsync(project);
            }
            else // Create new project
            {

                await _projectService.CreateProjectAsync(project);
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