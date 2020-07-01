using PIMWebMVC.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PIMWebMVC.Constants
{
    public static class ProjectConstant
    {
        #region Fields
        private static readonly IDictionary<string, string> _projectStatuses = new Dictionary<string, string>()
        {
            { "INV", PIMResource.PROJECT_STATUS_INV},
            { "TOV", PIMResource.PROJECT_STATUS_TOV},
            { "VAL", PIMResource.PROJECT_STATUS_VAL},
            { "FIN", PIMResource.PROJECT_STATUS_FIN}
        };
        #endregion

        #region Properties
        public static readonly ReadOnlyDictionary<string, string> PROJECT_STATUSES = new ReadOnlyDictionary<string, string>(_projectStatuses);
        #endregion
    }
}