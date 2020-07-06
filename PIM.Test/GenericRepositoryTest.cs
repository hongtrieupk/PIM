﻿using NHibernate;
using NHibernate.Exceptions;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
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
    public class GenericRepositoryTest
    {
        #region Fields
        //Projects testing data to Assert
        // - projectTest1, ProjectId = 1 will be used for testing GetById and Update function
        private static readonly Project _projectTest1 = new Project() { ProjectNumber = 111, ProjectName = "Project Test 1", Customer = "Apple", Status = "INV", StartDate = DateTime.Now };
        // - projectTest1, ProjectId = 2 will be used for testing Delete function
        private static readonly Project _projectTest2 = new Project() { ProjectNumber = 222, ProjectName = "Project Test 2", Customer = "Beats", Status = "INV", StartDate = DateTime.Now };
        private static readonly List<Project> _seedProjectsData = new List<Project>()
        {
            _projectTest1,
            _projectTest2
        };
        #endregion

        #region Constructors
        public GenericRepositoryTest()
        {
            CreateDatabase();

        }
        #endregion
        #region Methods       
        #region Set up methods
        private void CreateDatabase()
        {
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                new SchemaExport(dbContext.Configuration).Drop(useStdOut: false, execute: true);
                new SchemaExport(dbContext.Configuration).Create(useStdOut: false, execute: true);
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                using (IGenericTransaction transaction = dbContext.BeginTransaction())
                {
                    foreach (Project project in _seedProjectsData)
                    {
                        projectRepository.Add(project);
                        dbContext.Flush();
                    }
                    transaction.Commit();
                }
            }

        }
        #endregion

        #region Test Methods     
        [Test]
        public void GetById__GetProjectTest1__ShouldReturnProject1Name()
        {
            // Arrange
            int projectTest1Id = _projectTest1.ProjectID;
            Project projectTest1FromDb;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                projectTest1FromDb = projectRepository.GetById(projectTest1Id);
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
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                projectFromDb = projectRepository.GetById(invalidProjectId);
            }

            // Assert
            Assert.IsNull(projectFromDb);
        }
        [Test]
        public void Add__AddNewProject__ShouldStoreInDatabase()
        {
            // Arrange
            Project newProject = new Project() { ProjectNumber = 333, ProjectName = "Viacar", Customer = "Switzerland", Status = "INV", StartDate = DateTime.Now };
            object objectId;
            Project insertedProject;

            // Action 
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                using (IGenericTransaction transaction = dbContext.BeginTransaction())
                {
                    objectId = projectRepository.Add(newProject);
                    transaction.Commit();
                }
                insertedProject = projectRepository.GetById(objectId);
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
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                using (IGenericTransaction transaction = dbContext.BeginTransaction())
                {
                    projectRepository.Update(_projectTest1);
                    transaction.Commit();
                    
                }
                updatedProjectFromDb = projectRepository.GetById(_projectTest1.ProjectID);
            }

            // Assert
            Assert.IsNotNull(updatedProjectFromDb);
            Assert.AreEqual(updatedProjectFromDb.ProjectName, updatedProjectFromDb.ProjectName);
            Assert.AreEqual(updatedProjectFromDb.Customer, updatedProjectFromDb.Customer);
        }
        [Test]
        public void Delete__DeleteAnExistedProject__ShouldRemovedFromDatabase()
        {
            // Arrange
            Project existedProject = _projectTest2;
            Project deletedProject;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                using (IGenericTransaction transaction = dbContext.BeginTransaction())
                {
                    projectRepository.Delete(existedProject);
                    transaction.Commit();
                }
                deletedProject = projectRepository.GetById(existedProject.ProjectID);
            }

            // Assert
            Assert.IsNull(deletedProject);
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
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                projectsResult = projectRepository.FilterBy((x) => x.ProjectName.Contains(searchValue)).ToList();
            }

            //Assert
            Assert.IsNotNull(projectsResult);
            Assert.IsTrue(projectsResult.Count() > 0);

        }
        [Test]
        public void Delete__DeleteNotExistedProject__ShouldThrowStaleStateException()
        {
            // Arrange
            // a Project, which does not existed in the database
            Project notExistedProject = new Project()
            { ProjectID = 100000, ProjectName = "fake", Customer = "Fake", Status = "Fake", ProjectNumber = 123, StartDate = DateTime.Now };
            StaleStateException staleStateException = null;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                GenericRepository<Project> projectRepository = new GenericRepository<Project>(dbContext.CurrentSession);
                using (IGenericTransaction transaction = dbContext.BeginTransaction())
                {
                    projectRepository.Delete(notExistedProject);
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
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                GenericRepository<Project> projectRepo = new GenericRepository<Project>(dbContext.CurrentSession);
                using (IGenericTransaction transaction = dbContext.BeginTransaction())
                {
                    genericADOException = Assert.Throws<GenericADOException>(() => projectRepo.Add(newProject));
                }
            }

            // Assert
            Assert.IsNotNull(genericADOException);
        }
        [Test]
        public void Update__UpdateNotExistedProject___ShouldThrowStaleStateException()
        {
            // Arrange
            Project notExistedProject = new Project()
            { ProjectID = 100000, ProjectName = "fake", Customer = "Fake", Status = "Fake", ProjectNumber = 123, StartDate = DateTime.Now };
            StaleStateException staleStateException = null;

            // Action
            using (IApplicationDbContext dbContext = new ApplicationDbContext())
            {
                GenericRepository<Project> projectRepo = new GenericRepository<Project>(dbContext.CurrentSession);
                using (IGenericTransaction transaction = dbContext.BeginTransaction())
                {
                    projectRepo.Update(notExistedProject);
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
