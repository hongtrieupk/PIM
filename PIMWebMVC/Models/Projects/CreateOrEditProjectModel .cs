using PIM.Common.Constants;
using PIMWebMVC.Constants;
using PIMWebMVC.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PIMWebMVC.Models.Projects
{
    public class ProjectModel
    {
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
        [RegularExpression(RegexPatternConstants.ALLOWED_CHARACTER_SET)]
        public string ProjectName { get; set; }

        [StringLength(500)]
        [Required]
        [RegularExpression(RegexPatternConstants.ALLOWED_CHARACTER_SET)]
        public string Customer { get; set; }

        [StringLength(3)]
        [Required]
        public string Status { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = GlobalConfigurationConstants.DATETIME_MVC_MODEL_FORMAT, ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = GlobalConfigurationConstants.DATETIME_MVC_MODEL_FORMAT, ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public int Version { get; set; }
        #endregion

        #region Methods
        public bool IsValidMandatoryFields()
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
        public bool IsMatchAllowCharacterSet()
        {
            Regex allowCharactersRegex = new Regex(RegexPatternConstants.ALLOWED_CHARACTER_SET);
            string projectName = ProjectName ?? string.Empty;
            string customer = Customer ?? string.Empty;
            return allowCharactersRegex.IsMatch(projectName)
                && allowCharactersRegex.IsMatch(customer);

        }
        #endregion
    }
}