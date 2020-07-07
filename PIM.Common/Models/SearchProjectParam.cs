
using PIM.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PIM.Common.Models
{
    public class SearchProjectParam
    {
        #region Properties
        [Range(GlobalConfigurationConstants.MIN_PROJECT_NUMBER_VALUE,
           GlobalConfigurationConstants.MAX_PROJECT_NUMBER_VALUE)]
        public int? ProjectNumber { get; set; }
        [StringLength(100)]
        public string ProjectName { get; set; }
        [StringLength(3)]
        public string Status { get; set; }
        [StringLength(500)]
        public string Customer { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public bool IsValidParam
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(ProjectName) 
                    || !string.IsNullOrWhiteSpace(Customer) 
                    || !string.IsNullOrWhiteSpace(Status)
                    || ProjectNumber.HasValue;
            }
        }
        public bool IsValidPageSize
        {
            get
            {
                return PageSize > 0;
            }
        }
        #endregion
    }
}