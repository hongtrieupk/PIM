using Moq;
using NHibernate;
using NUnit.Framework;
using PIM.Business.Services;
using PIM.Data.NHibernateConfiguration;
using PIM.Data.Objects;
using PIM.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PIM.Test
{
    [TestFixture]
    public class ProjectServiceTest
    {
        #region Fields
        private IProjectService _projectService;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IApplicationDbContext> _dbContextMock;
        private IQueryable<Project> _projectsIQueryable;
        #endregion

        #region Methods
        [SetUp]
        public void Init()
        {
            _projectsIQueryable = new List<Project>() {
                        new Project() { ProjectID = 1, ProjectName = "Galaxy Note 12 2020", ProjectNumber = 444, Customer = "Samsung", Status = "FIN", StartDate = DateTime.Now },
                        new Project() { ProjectID = 2, ProjectName = "Iphone 12 2020", ProjectNumber = 999, Customer = "Apple", Status = "VAL", StartDate = DateTime.Now },
                        new Project() { ProjectID = 3, ProjectName = "Auto Cars 2020", ProjectNumber = 666, Customer = "BMW", Status = "VAL", StartDate = DateTime.Now },
                        new Project() { ProjectID = 4, ProjectName = "World cup 2020", ProjectNumber = 2020, Customer = "FIFO", Status = "VAL", StartDate = DateTime.Now }
            }.AsQueryable();

            _dbContextMock = new Mock<IApplicationDbContext>();
            _dbContextMock.Setup(x => x.OpenSession()).Returns((new Mock<ISession>()).Object);

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(x => x.SetSession(It.IsAny<ISession>()));
            _projectRepositoryMock.Setup(x => x.FilterBy(It.IsAny<Expression<Func<Project, bool>>>()))
                .Returns<Expression<Func<Project, bool>>>((criteria) => _projectsIQueryable.Where(criteria));

            _projectService = new ProjectService(_projectRepositoryMock.Object, _dbContextMock.Object);
        }
        [Test]
        public void IsDuplicateProjectNumber__CheckExistedNumber_444_ProjectID_Null___ShouldReturnTrue()
        {
            // Arrange
            int existedProjectNumber = 444;
            int? newProjectId = null;

            // Action
            bool isDupplicated = _projectService.IsDuplicateProjectNumber(newProjectId, existedProjectNumber);

            // Assert
            Assert.IsTrue(isDupplicated);
        }

        [Test]
        public void IsDuplicateProjectNumber__CheckExistedNumber_999_TheSameProjectId_2___ShouldReturnFalse()
        {
            // Arrange
            int existedProjectNumber = 999;
            int theSameProjectId = 2;

            // Action
            bool isDupplicated = _projectService.IsDuplicateProjectNumber(theSameProjectId, existedProjectNumber);

            // Assert
            Assert.IsFalse(isDupplicated);
        }

        [Test]
        public void IsDuplicateProjectNumber__CheckNotExistedNumber_707020___ShouldReturnFalse()
        {
            // Arrange
            int newProjectNumber = 707020;
            int? newProjectId = null;

            // Action
            bool isDupplicated = _projectService.IsDuplicateProjectNumber(newProjectId, newProjectNumber);

            // Assert
            Assert.IsFalse(isDupplicated);
        }
        #endregion
    }
}
