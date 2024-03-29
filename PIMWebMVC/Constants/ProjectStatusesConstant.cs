﻿using PIMWebMVC.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PIMWebMVC.Constants
{
    public class ProjectStatusesConstant : IProjectStatusesConstant
    {
        #region Fields
        private ReadOnlyDictionary<string, string> _readOnlyProjectStatues;
        #endregion

        #region Properties
        public ReadOnlyDictionary<string, string> PROJECT_STATUSES
        {
            get
            {
                if (_readOnlyProjectStatues == null)
                {
                    _readOnlyProjectStatues = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(){
                                            { "INV", PIMResource.PROJECT_STATUS_INV},
                                            { "TOV", PIMResource.PROJECT_STATUS_TOV},
                                            { "VAL", PIMResource.PROJECT_STATUS_VAL},
                                            { "FIN", PIMResource.PROJECT_STATUS_FIN}
                                        });
                }
                return _readOnlyProjectStatues;
            }
        }
        #endregion
    }
}