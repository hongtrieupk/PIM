

using System.Web.UI;

namespace PIM.Common.Models
{
    public class SearchProjectParam
    {
        public int? ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
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
    }
}