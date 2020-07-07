using PIM.Common.Constants;
using PIMWebMVC.Constants;
using PIMWebMVC.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Mvc;

namespace PIMWebMVC.Models.Projects
{
    public class ProjectModel
    {
        #region Fields
        #endregion

        #region Construtors
        public ProjectModel() { }
        #endregion

        #region Properties
        public int? ProjectID { get; set; }

        // leave ErrorMessage blank string, this error message is handled in ProjectsControler.ValidateProjectModel() 
        [Required(ErrorMessage = " ")]
        [Range(GlobalConfigurationConstants.MIN_PROJECT_NUMBER_VALUE,
            GlobalConfigurationConstants.MAX_PROJECT_NUMBER_VALUE,
            ErrorMessageResourceName = "MESSAGE_PROJECT_NUMBER_NOT_IN_RANGE",
            ErrorMessageResourceType = typeof(PIMResource))]
        public int? ProjectNumber { get; set; }

        [StringLength(100)]
        [Required]
        public string ProjectName { get; set; }

        [StringLength(500)]
        [Required]
        public string Customer { get; set; }

        [StringLength(3)]
        [Required]
        public string Status { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public int Version { get; set; }
        public string StatusDisplay
        {
            get
            {
                IProjectStatuesConstant projectStatues = new ProjectStatuesConstant();
                if (this.Status != null && projectStatues.PROJECT_STATUSES.ContainsKey(this.Status))
                {
                    return projectStatues.PROJECT_STATUSES[this.Status];
                }
                return string.Empty;
            }
        }
        #endregion

        #region Methods
        public bool IsValidMadatoryFields()
        {
            return ProjectNumber.HasValue
                && !string.IsNullOrWhiteSpace(ProjectName)
                && !string.IsNullOrWhiteSpace(Customer)
                && !string.IsNullOrWhiteSpace(Status)
                && StartDate.HasValue;
        }
        public bool IsValidEndDate()
        {
            // if null fallback to "Required" validation
            if (EndDate.HasValue && StartDate.HasValue)
            {
                return EndDate.Value.Date > StartDate.Value.Date;
            }
            return true;
        }
        public bool IsProjectNumberInRange()
        {
            return ProjectNumber.HasValue && ProjectNumber.Value >= GlobalConfigurationConstants.MIN_PROJECT_NUMBER_VALUE
                && ProjectNumber.Value <= GlobalConfigurationConstants.MAX_PROJECT_NUMBER_VALUE;
        }
        #endregion
    }
}