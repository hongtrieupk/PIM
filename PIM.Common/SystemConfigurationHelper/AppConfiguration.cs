using PIM.Common.Constants;
using System.Configuration;
using System.Web.UI;

namespace PIM.Common.SystemConfigurationHelper
{
    public class AppConfiguration : IAppConfiguration
    {
        #region Properties
        public int DefaultPageSize
        {
            get 
            {
                int pageSize;
                return int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageSize)
                    ? pageSize
                    : GlobalConfigurationConstants.DEFAULT_PAGESIZE;
            }
        }
        #endregion
    }
}
