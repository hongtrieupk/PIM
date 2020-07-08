using PIMWebMVC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIMWebMVC.Models.Projects
{
    public class ProjectForViewTableModel
    {
        #region Properties
        public  int ProjectID { get; set; }
        public  int ProjectNumber { get; set; }
        public  string ProjectName { get; set; }
        public  string Customer { get; set; }
        public  string Status { get; set; }
        public  DateTime StartDate { get; set; }
        public  int Version { get; set; }
        public string StatusDisplay
        {
            get
            {
                IProjectStatusesConstant projectStatues = new ProjectStatusesConstant();
                if (this.Status != null && projectStatues.PROJECT_STATUSES.ContainsKey(this.Status))
                {
                    return projectStatues.PROJECT_STATUSES[this.Status];
                }
                return string.Empty;
            }
        }
        #endregion
    }
}