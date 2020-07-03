
using PIM.Data.Objects;
using System.Linq;
using PIM.Common.Models;

namespace PIM.Data.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        PagingResultModel<Project> SearchProject(SearchProjectParam searchParam);
    }
}
