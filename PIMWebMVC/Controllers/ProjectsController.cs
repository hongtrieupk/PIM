using AutoMapper;
using PIM.Business.Services;
using PIM.Data.Objects;
using PIM.Object.GenericRepositories;
using PIM.Object.UnitOfWork;
using PIMWebMVC.Constants;
using PIMWebMVC.Models.Common;
using PIMWebMVC.Models.Projects;
using PIMWebMVC.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PIMWebMVC.Controllers
{
    public class ProjectsController : Controller
    {
        #region Fields
        private readonly IProjectService _projectService;
        #endregion
        #region Constructors
        public ProjectsController()
        {
            // TODO: apply IOC
            _projectService = new ProjectService(new GenericRepository<Project>(UnitOfWork.CurrentSession), UnitOfWork.Current);
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Project(int? id)
        {
            ProjectModel initProject = new ProjectModel();
            if (id.HasValue)
            {
                initProject.ProjectID = id;
                initProject.ProjectName = "Iphones";
                initProject.Customer = "Apple";
                initProject.StartDate = DateTime.Now;
                initProject.Status = "INV";
            }
            return View(initProject);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Project(ProjectModel project)
        {
            if (!ValidateProjectModel(project))
            {
                return View(project);
            }
            if (project.ProjectID.HasValue) // Update project
            {
                
            }
            else // Create new project
            {
                Project newProject = Mapper.Map<Project>(project);
                await _projectService.CreateProjectAsync(newProject);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SearchProjects(SearchProjectParam searchParam)
        {
            if (!searchParam.IsValidParam)
            {
                throw new ArgumentNullException($"{Resources.PIMResource.MESSAGE_INVALID_CRITERIA}");
            }
            var result = new ProjectsPaginationResult
            {
                Projects = new List<ProjectModel>
                {
                    new ProjectModel(1, "Achilles"),
                    new ProjectModel(2, "Medidata"),
                    new ProjectModel(3, "Viacar")
                },
                CurrentPage = searchParam.CurrentPage,
                TotalPages = 5,
                TotalRecords = 100,
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

            //TODO: call service to check the project number whether is duplicated or not
            if (project.ProjectNumber == 100)
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