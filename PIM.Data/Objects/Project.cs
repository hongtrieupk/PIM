
using System;

namespace PIM.Data.Objects
{
    public class Project
    {
        #region Properties
        public virtual int ProjectID { get; set; }
        public virtual int ProjectNumber { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual string Customer { get; set; }    
        public virtual string Status { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        #endregion
    }
}
