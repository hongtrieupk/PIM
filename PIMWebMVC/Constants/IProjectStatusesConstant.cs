using System.Collections.ObjectModel;

namespace PIMWebMVC.Constants
{
    public interface IProjectStatusesConstant
    {
        ReadOnlyDictionary<string, string> PROJECT_STATUSES { get; }
    }
}