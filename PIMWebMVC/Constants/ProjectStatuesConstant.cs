using PIMWebMVC.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PIMWebMVC.Constants
{
    public class ProjectStatuesConstant: IProjectStatuesConstant
    {
        #region Fields
        private IDictionary<string, string> _projectStatuses = new Dictionary<string, string>()
        {
            { "INV", PIMResource.PROJECT_STATUS_INV},
            { "TOV", PIMResource.PROJECT_STATUS_TOV},
            { "VAL", PIMResource.PROJECT_STATUS_VAL},
            { "FIN", PIMResource.PROJECT_STATUS_FIN}
        };
        private readonly ReadOnlyDictionary<string, string> _readOnlyProjectStatues;
        #endregion
        #region Constructors
        public ProjectStatuesConstant()
        {
            _readOnlyProjectStatues = new ReadOnlyDictionary<string, string>(_projectStatuses);
        }
        #endregion

        #region Properties
        public ReadOnlyDictionary<string, string> PROJECT_STATUSES
        {
            get
            {
                return _readOnlyProjectStatues;
            }
        }
        #endregion
    }
}