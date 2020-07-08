using AutoMapper;
using log4net;
using PIM.Business.Services;
using PIM.Common.CustomExceptions;
using PIM.Common.Models;
using PIM.Common.SystemConfigurationHelper;
using PIM.Data.Objects;
using PIMWebMVC.Constants;
using PIMWebMVC.Models.Common;
using PIMWebMVC.Models.Projects;
using PIMWebMVC.Resources;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Web.Mvc;

namespace PIMWebMVC.Controllers
{
    public class ProjectsController : Controller
    {
        #region Fields
        private readonly IProjectService _projectService;
        private readonly IAppConfiguration _appConfiguration;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ProjectsController));

        private const string CREATE_SUCCESSFULLY_MESSAGE = "CREATE_SUCCESS_MESSAGE";
        private const string UPDATE_SUCCESSFULLY_MESSAGE = "UPDATE_SUCCESS_MASSAGE";
        #endregion
        #region Constructors
        public ProjectsController(IProjectService projectService, IAppConfiguration appConfiguration)
        {
            _projectService = projectService ?? throw new ArgumentNullException("Can not inject a null projectService!");
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException("Can not inject a null appConfiguration!");
        }
        #endregion
        #region Methods
        public ActionResult Index()
        {
            // the Index View will use those ViewBag's properties to show message if the Index view is redirected from Update or Create Action
            ViewBag.CREATE_SUCCESS_MESSAGE = TempData[CREATE_SUCCESSFULLY_MESSAGE];
            ViewBag.UPDATE_SUCCESS_MASSAGE = TempData[UPDATE_SUCCESSFULLY_MESSAGE];
            return View(new SearchProjectParam());
        }
        /// <summary>
        /// depend on the id parameter, this will be a Add new Project page if id is null and Update Project page if id has value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewDetail(int? id)
        {
            ProjectModel projectModelToCreateOrUpdate = new ProjectModel();
            if (id.HasValue)
            {
                Project projectFromDb = _projectService.GetProjectById(id.Value);
                projectModelToCreateOrUpdate = Mapper.Map<ProjectModel>(projectFromDb);
            }

            return View(projectModelToCreateOrUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(ProjectModel projectModel)
        {
            ValidateForCreateOrUpdate(projectModel);
            if (!ModelState.IsValid)
            {
                ViewBag.FormHasError = true;
                return View("ViewDetail", projectModel);
            }

            Project project = Mapper.Map<Project>(projectModel);
            try
            {
                if (projectModel.ProjectID.HasValue) // Update project
                {
                    _projectService.UpdateProject(project);
                    TempData[UPDATE_SUCCESSFULLY_MESSAGE] = PIMResource.MESSAGE_UPDATE_PROJECT_SUCCESS;
                }
                else // Create new project
                {
                    _projectService.CreateProject(project);
                    TempData[CREATE_SUCCESSFULLY_MESSAGE] = PIMResource.MESSAGE_CREATE_PROJECT_SUCCESS;
                }

                return RedirectToAction("Index");
            }
            catch (ConcurrencyDbException concurrencyException)
            {
                _logger.Error(concurrencyException.InnerException);
                ModelState.AddModelError(ErrorsConstant.SUM_ERROR_FIELD_NAME, PIMResource.ERROR_DB_CONCURRENCY_MESSAGE);
                ViewBag.FormHasError = true;
                return View("ViewDetail", projectModel);
            }
        }

        [HttpPost]
        public ActionResult SearchProjects(SearchProjectParam searchParam)
        {
            ProjectsPaginationResult result = new ProjectsPaginationResult();
            if (searchParam.IsValidParam && ModelState.IsValid)
            {
                searchParam.PageSize = searchParam.PageSize > 0 ? searchParam.PageSize : _appConfiguration.DefaultPageSize;
                PagingResultModel<Project> projectsPagingResult = _projectService.SearchProject(searchParam);
                result = Mapper.Map<ProjectsPaginationResult>(projectsPagingResult);
            }   
            return PartialView("_ProjectsPaginationPartial", result);
        }

        [HttpPost]
        public ActionResult DeleteProjects(IList<ProjectModel> projects)
        {
            try
            {
                IList<Project> deletedProjects = Mapper.Map<IList<Project>>(projects);
                _projectService.DeleteProjects(deletedProjects);
                return Json(new ActionResultModel() { IsSuccess = true, Message = PIMResource.MESSAGE_DELETE_PROJECTS_SUCCESS });
            }
            catch(ConcurrencyDbException concurrencyException)
            {
                _logger.Error(concurrencyException.InnerException);
                return Json(new ActionResultModel() { IsSuccess = false, Message = PIMResource.ERROR_DB_CONCURRENCY_MESSAGE });
            }

        }

        public ActionResult PaginationBar(PaginationBarModel dataModel)
        {
            return PartialView("_PaginationBarPartial", dataModel);
        }

        private void ValidateForCreateOrUpdate(ProjectModel project)
        {
            ValidateMandatoryFields(project);
            ValidateSpecialCharacters(project);
            ValidateEndDate(project);
            ValidateProjectNumber(project);
        }
        private void ValidateMandatoryFields(ProjectModel project)
        {
            if (project.IsValidMandatoryFields())
            {
                ModelState.Remove(ErrorsConstant.SUM_ERROR_FIELD_NAME);
            }
            else
            {
                ModelState.AddModelError(ErrorsConstant.SUM_ERROR_FIELD_NAME, PIMResource.MESSAGE_MANDATORY_FIELD);
            }
        }
        private void ValidateSpecialCharacters(ProjectModel project)
        {

            if (project.IsMatchAllowCharacterSet())
            {
                ModelState.Remove(ErrorsConstant.SUM_ERROR_SPECIAL_CHARACTERS_FIELD_NAME);
            }
            else
            {
                ModelState.AddModelError(ErrorsConstant.SUM_ERROR_SPECIAL_CHARACTERS_FIELD_NAME, PIMResource.MESSAGE_SPECIAL_CHARACTER_ERROR);
            }
        }
        private void ValidateEndDate(ProjectModel project)
        {
            if (project.IsValidEndDate())
            {
                ModelState.Remove(nameof(project.EndDate));
            }
            else
            {
                ModelState.AddModelError(nameof(project.EndDate), PIMResource.MESSAGE_END_DATE_MUST_GREATER_THAN_START_DATE);
            }
        }
        private void ValidateProjectNumber(ProjectModel project)
        {
            bool isDuplicatedProjectNumber = project.ProjectNumber.HasValue
                ? _projectService.IsDuplicateProjectNumber(project.ProjectID, project.ProjectNumber.Value)
                : false;
            if (isDuplicatedProjectNumber)
            {
                ModelState.AddModelError(nameof(project.ProjectNumber), PIMResource.MESSAGE_DUPLICATED_PROJECT_NUMBER);
            }
            else
            {
                if (project.IsProjectNumberInRange())
                {
                    ModelState.Remove(nameof(project.ProjectNumber));
                }
            }
        }
        #endregion
    }
}