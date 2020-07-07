using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace PIMWebMVC.Constants
{
    public interface IProjectStatuesConstant
    {
        ReadOnlyDictionary<string, string> PROJECT_STATUSES { get; }
    }
}