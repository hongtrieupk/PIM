using PIM.Data.Objects;
using System.Threading.Tasks;

namespace PIM.Business.Services
{
    public interface IProjectService
    {
        Task<Project> CreateProjectAsync(Project newProject);
    }
}
