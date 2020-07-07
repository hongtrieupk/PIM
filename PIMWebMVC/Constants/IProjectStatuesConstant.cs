using System.Collections.ObjectModel;

namespace PIMWebMVC.Constants
{
    public interface IProjectStatuesConstant
    {
        ReadOnlyDictionary<string, string> PROJECT_STATUSES { get; }
    }
}