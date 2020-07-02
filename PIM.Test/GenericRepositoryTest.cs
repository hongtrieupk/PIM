using NHibernate;
using NHibernate.Cfg;
using NHibernate.Exceptions;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using PIM.Data.Objects;
using PIM.Object.GenericRepositories;
using PIM.Object.UnitOfWork;
using PIM.Object.UnitOfWork.GenericTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Threading.Tasks;

namespace PIM.Test
{
    /// <summary>
    /// User Project Object to test GenericRepository
    /// </summary>
    [TestFixture]
    public class GenericRepositoryTest
    {
        #region Fields
        private GenericRepository<Project> _projectRepository;
        private IUnitOfWork _currentUnitOfWork;
        //Projects testing data to Assert
        // - projectTest1, ProjectId = 1 will be used for testing GetById and Update function
        private static readonly Project _projectTest1 = new Project() { ProjectNumber = 111, ProjectName = "Project Test 1", Customer = "Apple", Status = "INV", StartDate = DateTime.Now };
        // - projectTest1, ProjectId = 2 will be used for testing Delete function
        private static readonly Project _projectTest2 = new Project() { ProjectNumber = 222, ProjectName = "Project Test 2", Customer = "Beats", Status = "INV", StartDate = DateTime.Now };
        #endregion

        #region Constructors
        public GenericRepositoryTest()
        {
            _currentUnitOfWork = UnitOfWork.Start();
            CreateDatabase(UnitOfWork.Configuration);
            _projectRepository = new GenericRepository<Project>(_currentUnitOfWork.Session);
            InsertTestProjectsData();

        }
        #endregion
        #region Destructor
        ~GenericRepositoryTest()
        {
            if (_currentUnitOfWork != null)
            {
                _currentUnitOfWork.Dispose();
            }
        }
        #endregion
        #region Methods       
        #region Set up methods
        private void CreateDatabase(Configuration nHibernateConfiguration)
        {
            new SchemaExport(nHibernateConfiguration).Drop(useStdOut: false, execute: true);
            new SchemaExport(nHibernateConfiguration).Create(useStdOut: false, execute: true);
        }
        private void InsertTestProjectsData()
        {
            using (IGenericTransaction transaction = _currentUnitOfWork.BeginTransaction())
            {
                var projectId1 = _projectRepository.AddAsync(_projectTest1).Result;
                var projectId2 = _projectRepository.AddAsync(_projectTest2).Result;
                transaction.Commit();
            }

        }
        #endregion

        #region Test Methods     
        [Test]
        public async Task GetById__GetProjectTest1__ShouldReturnProject1Name()
        {
            // Arrange
            int projectTest1Id = _projectTest1.ProjectID;
            Project projectTest1FromDb;

            // Action
            projectTest1FromDb = await _projectRepository.GetByIdAsync(projectTest1Id);

            // Assert
            Assert.IsNotNull(projectTest1FromDb);
            Assert.AreEqual(_projectTest1.ProjectName, projectTest1FromDb.ProjectName);
        }
        [Test]
        public async Task GetById__GetInvalidProjectId__ShouldReturnNull()
        {
            // Arrange
            int invalidProjectId = 0;
            Project projectFromDb;

            // Action
            projectFromDb = await _projectRepository.GetByIdAsync(invalidProjectId);

            // Assert
            Assert.IsNull(projectFromDb);
        }
        [Test]
        public async Task Add__AddNewProject__ShouldStoreInDatabase()
        {
            // Arrange
            Project newProject = new Project() { ProjectNumber = 333, ProjectName = "Viacar", Customer = "Switzerland", Status = "INV", StartDate = DateTime.Now };
            Project insertedProject;
            // Action 
            object objectId = await _projectRepository.AddAsync(newProject);
            insertedProject = await _projectRepository.GetByIdAsync(int.Parse(objectId.ToString()));
            // Assert
            Assert.IsNotNull(insertedProject);
            Assert.AreEqual(insertedProject.ProjectName, insertedProject.ProjectName);
        }
        [Test]
        public async Task Update__UpdateProjectTest1_Name_Customer___ShouldStoreInDatabase()
        {
            // Arrange
            _projectTest1.ProjectName = "New Name";
            _projectTest1.Customer = "New Customer";
            Project updatedProjectFromDb;

            // Action
            using (IGenericTransaction transaction = _currentUnitOfWork.BeginTransaction())
            {
                await _projectRepository.UpdateAsync(_projectTest1);
                await transaction.CommitAsync();
                updatedProjectFromDb = await _projectRepository.GetByIdAsync(_projectTest1.ProjectID);
            }

            // Assert
            Assert.IsNotNull(updatedProjectFromDb);
            Assert.AreEqual(updatedProjectFromDb.ProjectName, updatedProjectFromDb.ProjectName);
            Assert.AreEqual(updatedProjectFromDb.Customer, updatedProjectFromDb.Customer);
        }
        [Test]
        public async Task Delete__DeleteAnExistedProject__ShouldRemovedFromDatabase()
        {
            // Arrange
            Project existedProject = _projectTest2;
            Project deletedProject;
            _currentUnitOfWork.Session.Refresh(existedProject);
            // Action
            using (IGenericTransaction transaction = _currentUnitOfWork.BeginTransaction())
            {
                await _projectRepository.DeleteAsync(existedProject);
                await transaction.CommitAsync();
                deletedProject = await _projectRepository.GetByIdAsync(existedProject.ProjectID);
            }

            // Assert
            Assert.IsNull(deletedProject);
        }
        [Test]
        public void FilterBy__SearchContainsProjectName__ShouldReturnCorrectResult()
        {
            // Arrange
            string searchValue = "Test 1";
            int pageIndex = 1, pageSize = 10;

            // Action
            IList<Project> projectsResult = _projectRepository.FilterBy((x) => x.ProjectName.Contains(searchValue), pageIndex, pageSize).ToList();

            //Assert
            Assert.IsTrue(projectsResult.Count() > 0);

        }
        [Test]
        public async Task Delete__DeleteNotExistedProject__ShouldThrowStaleStateException()
        {
            // Arrange
            // a Project, which does not existed in the database
            Project notExistedProject = new Project()
            { ProjectID = 100000, ProjectName = "fake", Customer = "Fake", Status = "Fake", ProjectNumber = 123, StartDate = DateTime.Now };
            StaleStateException staleStateException = null;

            // Action
            using (IUnitOfWork unitOfworkForExceptionTest = new UnitOfWorkFactory().Create())
            {
                GenericRepository<Project> projectRepo = new GenericRepository<Project>(unitOfworkForExceptionTest.Session);
                using (IGenericTransaction transaction = unitOfworkForExceptionTest.BeginTransaction())
                {
                    await projectRepo.DeleteAsync(notExistedProject);
                    staleStateException = Assert.Throws<StaleStateException>(() => transaction.Commit());
                }
            }
            // Assert
            Assert.IsNotNull(staleStateException);
        }
        [Test]
        public void Add__AddNewProject_DuplicatedProjectNumber___ShouldThrowGenericADOException()
        {
            // Arrange
            Project newProject = new Project() { ProjectNumber = _projectTest1.ProjectNumber, ProjectName = "Test Duplicated Number", Customer = "Customer 1", Status = "INV", StartDate = DateTime.Now };
            Exception genericADOException;

            // Action 
            using (IUnitOfWork unitOfworkForExceptionTest = new UnitOfWorkFactory().Create())
            {
                GenericRepository<Project> projectRepo = new GenericRepository<Project>(unitOfworkForExceptionTest.Session);
                using (IGenericTransaction transaction = unitOfworkForExceptionTest.BeginTransaction())
                {
                    genericADOException = Assert.ThrowsAsync<GenericADOException>(() => projectRepo.AddAsync(newProject));
                }
            }


            // Assert
            Assert.IsNotNull(genericADOException);
        }
        [Test]
        public async Task Update__UpdateNotExistedProject___ShouldThrowStaleStateException()
        {
            // Arrange
            Project notExistedProject = new Project()
            { ProjectID = 100000, ProjectName = "fake", Customer = "Fake", Status = "Fake", ProjectNumber = 123, StartDate = DateTime.Now };
            StaleStateException staleStateException = null;

            // Action
            using (IUnitOfWork unitOfworkForExceptionTest = new UnitOfWorkFactory().Create())
            {
                GenericRepository<Project> projectRepo = new GenericRepository<Project>(unitOfworkForExceptionTest.Session);
                using (IGenericTransaction transaction = unitOfworkForExceptionTest.BeginTransaction())
                {
                    await projectRepo.UpdateAsync(notExistedProject);
                    staleStateException = Assert.Throws<StaleStateException>(() => transaction.Commit());
                }
            }

            // Assert
            Assert.IsNotNull(staleStateException);
        }
        #endregion
        #endregion
    }
}
