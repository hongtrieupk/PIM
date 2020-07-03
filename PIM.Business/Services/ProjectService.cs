using PIM.Data.Objects;
using PIM.Object.GenericRepositories;
using PIM.Object.UnitOfWork;
using PIM.Object.UnitOfWork.GenericTransactions;
using System;
using System.Threading.Tasks;

namespace PIM.Business.Services
{
    public class ProjectService : IProjectService
    {
        #region Fields
        private readonly IGenericRepository<Project> _projectRepository;
        private readonly IUnitOfWork _unitOfwork;
        #endregion

        #region Constructors
        public ProjectService(IGenericRepository<Project> projectRepository, IUnitOfWork unitOfwork)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException("Can not inject a null project repository!");
            _unitOfwork = unitOfwork ?? throw new ArgumentNullException("Can not inject a null unitOfwork!");
        }
        #endregion

        #region Method
        public async Task<Project> CreateProjectAsync(Project newProject)
        {
            using (IGenericTransaction transaction = _unitOfwork.BeginTransaction())
            {
                await _projectRepository.AddAsync(newProject);
                transaction.Commit();
            }
            return newProject;
        }
        #endregion
    }
}
