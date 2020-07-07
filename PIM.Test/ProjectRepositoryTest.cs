﻿using NHibernate;
using NHibernate.Exceptions;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using PIM.Common.CustomExceptions;
using PIM.Common.Models;
using PIM.Data.NHibernateConfiguration;
using PIM.Data.Objects;
using PIM.Data.Repositories;
using PIM.Data.Repositories.GenericTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PIM.Test
{
    /// <summary>
    /// User Project Object to test GenericRepository
    /// </summary>
    [TestFixture]
    public class ProjectRepositoryTest
    {
        #region Fields
        // _projectToAddNew will be used to test Add project function
        private static Project _projectToAddNew = new Project() { ProjectNumber = 333, ProjectName = "Viacar", Customer = "Switzerland", Status = "INV", StartDate = DateTime.Now };
        // - projectTest1 will be used to test GetById and Update function
        private static readonly Project _projectTest1 = new Project() { ProjectNumber = 111, ProjectName = "Project Test 1", Customer = "Apple", Status = "INV", StartDate = DateTime.Now };
        // - projectTest2will be used to test Delete function then will be inserted again
        private static readonly Project _projectTest2 = new Project() { ProjectNumber = 222, ProjectName = "Project Test 2", Customer = "Beats", Status = "INV", StartDate = DateTime.Now };
        // - projectTest 3, 4 will be used to test DeleteProjectByIds then will be inserted again
        private static readonly Project _projectTest3 = new Project() { ProjectName = "CHI", ProjectNumber = 777, Customer = "Hagen", Status = "INV", StartDate = DateTime.Now };
        private static readonly Project _projectTest4 = new Project() { ProjectName = "Macbook 2025", ProjectNumber = 555, Customer = "Apple", Status = "INV", StartDate = DateTime.Now };
        // - the rest will be used to test SearchProject
        private static readonly Project _projectTest5 = new Project() { ProjectName = "Galaxy Note 12 2020", ProjectNumber = 444, Customer = "Samsung", Status = "FIN", StartDate = DateTime.Now };
        private static readonly Project _projectTest6 = new Project() { ProjectName = "Iphone 12 2020", ProjectNumber = 999, Customer = "Apple", Status = "VAL", StartDate = DateTime.Now };
        private static readonly Project _projectTest7 = new Project() { ProjectName = "Auto Cars 2020", ProjectNumber = 666, Customer = "BMW", Status = "VAL", StartDate = DateTime.Now };
        private static readonly Project _projectTest8 = new Project() { ProjectName = "World cup 2020", ProjectNumber = 2020, Customer = "FIFO", Status = "VAL", StartDate = DateTime.Now };

        private static List<Project> _seedProjectsData = new List<Project>()
        {
            _projectTest1,
            _projectTest2,
            _projectTest3,
            _projectTest4,
            _projectTest5,
            _projectTest6,
            _projectTest7,
            _projectTest8
        };
        #endregion

        #region Methods       
        #region Helper methods
        [OneTimeSetUp]
        public void Init()
        {
            CreateDatabase();
        }
        [TearDown]
        public void Rollback()
        {
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        if (_projectToAddNew.ProjectID > 0)// after running Add__AddNewProject__ShouldStoreInDatabase
                        {
                            projectRepository.Delete(_projectToAddNew);
                            _projectToAddNew.ProjectID = 0;
                        }
                        if (_projectTest2.ProjectID == 0)// after running Delete__DeleteAnProjectTest2__ShouldRemovedFromDatabase
                        {
                            projectRepository.Add(_projectTest2);
                        }
                        if (_projectTest3.ProjectID == 0)// after running DeleteProjectByIds__DeleteProjectId_3_4__ShouldRemovedFromDatabase
                        {
                            projectRepository.Add(_projectTest3);
                        }
                        if (_projectTest4.ProjectID == 0)// after running DeleteProjectByIds__DeleteProjectId_3_4__ShouldRemovedFromDatabase
                        {
                            projectRepository.Add(_projectTest4);
                        }
                        transaction.Commit();
                    }
                }
            }
        }
        private void CreateDatabase()
        {
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                new SchemaExport(dbContext.Configuration).Drop(useStdOut: false, execute: true);
                new SchemaExport(dbContext.Configuration).Create(useStdOut: false, execute: true);
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        foreach (Project project in _seedProjectsData)
                        {
                            projectRepository.Add(project);
                        }
                        transaction.Commit();
                    }
                }
            }

        }
        #endregion

        #region Test Methods     
        [Test]
        public void GetById__GetProjectTest1__ShouldReturnCorrectProject1Name()
        {
            // Arrange
            int projectTest1Id = _projectTest1.ProjectID;
            Project projectTest1FromDb;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    projectTest1FromDb = projectRepository.GetById(projectTest1Id);
                }
            }
            // Assert
            Assert.IsNotNull(projectTest1FromDb);
            Assert.AreEqual(_projectTest1.ProjectName, projectTest1FromDb.ProjectName);
        }
        [Test]
        public void GetById__GetInvalidProjectId__ShouldReturnNull()
        {
            // Arrange
            int invalidProjectId = 0;
            Project projectFromDb;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    projectFromDb = projectRepository.GetById(invalidProjectId);
                }
            }

            // Assert
            Assert.IsNull(projectFromDb);
        }
        [Test]
        public void Add__AddNewProject__ShouldStoreInDatabase()
        {
            // Arrange
            Project newProject = _projectToAddNew;
            object objectId;
            Project insertedProject;

            // Action 
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        objectId = projectRepository.Add(newProject);
                        transaction.Commit();
                    }
                    insertedProject = projectRepository.GetById(objectId);
                }
            }

            // Assert
            Assert.IsNotNull(insertedProject);
            Assert.AreEqual(insertedProject.ProjectName, insertedProject.ProjectName);
        }
        [Test]
        public void Update__UpdateProjectTest1_Name_Customer___ShouldStoreInDatabase()
        {
            // Arrange
            _projectTest1.ProjectName = "New Name";
            _projectTest1.Customer = "New Customer";
            Project updatedProjectFromDb;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        projectRepository.Update(_projectTest1);
                        transaction.Commit();

                    }
                    updatedProjectFromDb = projectRepository.GetById(_projectTest1.ProjectID);
                }
            }

            // Assert
            Assert.IsNotNull(updatedProjectFromDb);
            Assert.AreEqual(updatedProjectFromDb.ProjectName, updatedProjectFromDb.ProjectName);
            Assert.AreEqual(updatedProjectFromDb.Customer, updatedProjectFromDb.Customer);
        }
        [Test]
        public void Delete__DeleteAnProjectTest2__ShouldRemovedFromDatabase()
        {
            // Arrange
            Project existedProject = _projectTest2;
            Project deletedProject;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        projectRepository.Delete(existedProject);
                        transaction.Commit();
                    }
                    deletedProject = projectRepository.GetById(existedProject.ProjectID);
                    _projectTest2.ProjectID = 0;
                }
            }

            // Assert
            Assert.IsNull(deletedProject);
        }
        [Test]
        public void DeleteProjectByIds__DeleteProjectId_3_4__ShouldRemovedFromDatabase()
        {
            // Arrange
            int projectId3 = _projectTest3.ProjectID, projectId4 = _projectTest4.ProjectID;
            IList<int> projectIds = new List<int>() { projectId3, projectId4 };
            Project deletedProject4;
            Project deletedProject5;
            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        projectRepository.DeleteProjectsByIds(projectIds);
                        transaction.Commit();
                    }
                    deletedProject4 = projectRepository.GetById(projectId3);
                    deletedProject5 = projectRepository.GetById(projectId4);
                    _projectTest3.ProjectID = 0;
                    _projectTest4.ProjectID = 0;
                }
            }

            // Assert
            Assert.IsNull(deletedProject4);
            Assert.IsNull(deletedProject5);
        }
        [Test]
        public void FilterBy__SearchContainsProjectName__ShouldReturnCorrectResult()
        {
            // Arrange
            string searchValue = "Test 1";
            IList<Project> projectsResult;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    projectsResult = projectRepository.FilterBy((x) => x.ProjectName.Contains(searchValue)).ToList();
                }
            }

            //Assert
            Assert.IsNotNull(projectsResult);
            Assert.IsTrue(projectsResult.Count() > 0);

        }
        [Test]
        public void Delete__DeleteNotExistedProject__ShouldThrowConcurrencyUpdateException()
        {
            // Arrange
            // a Project, which does not existed in the database
            Project notExistedProject = new Project()
            { ProjectID = 100000, ProjectName = "fake", Customer = "Fake", Status = "Fake", ProjectNumber = 123, StartDate = DateTime.Now };
            ConcurrencyUpdateException staleStateException = null;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        projectRepository.Delete(notExistedProject);
                        staleStateException = Assert.Throws<ConcurrencyUpdateException>(() => transaction.Commit());
                    }
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
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        genericADOException = Assert.Throws<GenericADOException>(() => projectRepository.Add(newProject));
                    }
                }
            }

            // Assert
            Assert.IsNotNull(genericADOException);
        }
        [Test]
        public void Update__UpdateNotExistedProject___ShouldThrowConcurrencyUpdateException()
        {
            // Arrange
            Project notExistedProject = new Project()
            { ProjectID = 100000, ProjectName = "fake", Customer = "Fake", Status = "Fake", ProjectNumber = 123, StartDate = DateTime.Now };
            ConcurrencyUpdateException dbException = null;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        projectRepository.Update(notExistedProject);
                        dbException = Assert.Throws<ConcurrencyUpdateException>(() => transaction.Commit());
                    }
                }
            }

            // Assert
            Assert.IsNotNull(dbException);
        }
        [Test]
        public void Update__UpdateWithDifferentVersion___ShouldThrowConcurrencyUpdateException()
        {
            // Arrange
            Project existedProject = _projectTest1;
            _projectTest1.ProjectName = "new name";
            _projectTest1.Version = 0; // simulate _project1 haved been modified in the database
            ConcurrencyUpdateException dbException = null;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    using (IGenericTransaction transaction = dbContext.BeginTransaction())
                    {
                        projectRepository.Update(_projectTest1);
                        dbException = Assert.Throws<ConcurrencyUpdateException>(() => transaction.Commit());
                    }
                }
            }

            // Assert
            Assert.IsNotNull(dbException);
        }
        [Test]
        public void SearchProject__SearchBy_Name_2020_Customer_a__ShouldReturnCountEqual2()
        {
            // Arrange
            int expectedFoundedRecords = 2;
            SearchProjectParam param = new SearchProjectParam()
            {
                PageSize = 5,
                CurrentPage = 1,
                ProjectName = "2020",
                Customer = "a"
            };
            PagingResultModel<Project> result;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    result = projectRepository.SearchProject(param);
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFoundedRecords, result.Records.Count);
        }
        [Test]
        public void SearchProject__SearchBy_Status_VAL__ShouldReturnCountEqual3()
        {
            // Arrange
            int expectedFoundedRecords = 3;
            SearchProjectParam param = new SearchProjectParam()
            {
                PageSize = 5,
                CurrentPage = 1,
                Status = "VAL"
            };
            PagingResultModel<Project> result;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    result = projectRepository.SearchProject(param);
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFoundedRecords, result.Records.Count);
        }
        [Test]
        public void SearchProject__SearchBy_Status_VAL_Name_2020_Number_999_Cutomer_Apple_ShouldReturnCountEqual1()
        {
            // Arrange
            int expectedFoundedRecords = 1;
            SearchProjectParam param = new SearchProjectParam()
            {
                PageSize = 5,
                CurrentPage = 1,
                Status = "VAL",
                Customer = "a",
                ProjectName = "2020",
                ProjectNumber = 999
            };
            PagingResultModel<Project> result;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                using (var session = dbContext.OpenSession())
                {
                    IProjectRepository projectRepository = new ProjectRepository();
                    projectRepository.SetSession(session);
                    result = projectRepository.SearchProject(param);
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFoundedRecords, result.Records.Count);
        }
        #endregion
        #endregion
    }
}
