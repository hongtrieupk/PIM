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
        public ProjectModel(int number, string name)
        {
            ProjectID = number;
            ProjectNumber = number;
            ProjectName = name;
            StartDate = DateTime.Now;
        }
        #endregion

        #region Properties
        [Required]
        public int? ProjectID { get; set; }

        [Required(ErrorMessage = " ")] // leave blank string, this error message is handled by ErrorActionsConstant.SUM_REQUIRED_ERROR_FIELD_NAME
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

        public string StatusDisplay
        {
            get
            {
                if (this.Status != null && ProjectConstant.PROJECT_STATUSES.ContainsKey(this.Status))
                {
                    return ProjectConstant.PROJECT_STATUSES[this.Status];
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
        #endregion
    }
}