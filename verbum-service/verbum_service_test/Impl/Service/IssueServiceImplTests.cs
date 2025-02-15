using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet.Frameworks;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Service;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class IssueServiceImplTests
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            if(dbContext.Roles.Where(r => r.RoleId == "LINGUIST").ToList().Count() <= 0)
            {
                dbContext.Roles.Add(new Role
                {
                    RoleId = "LINGUIST",
                    RoleName = "Linguist"
                });
                await dbContext.SaveChangesAsync();
            }
            
            if (dbContext.Users.Where(u => u.RoleCode == "LINGUIST").ToList().Count() <= 0)
            {
                dbContext.Users.Add(new User
                {
                    Id = Guid.Parse("c78d7b35-7fe0-45f3-b171-df0eb48179c5"),
                    Name = "Linguist",
                    Email = "linguist@gmail.com",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = "ACTIVE",
                    RoleCode = "LINGUIST"
                });
                await dbContext.SaveChangesAsync();
            }

            if (await dbContext.Issues.CountAsync() <= 0)
            {
                dbContext.Issues.Add(new Issue
                {
                    IssueId = Guid.Parse("b740df46-793d-490f-94dc-77525c155de0"),
                    JobId = Guid.Parse("0db8b89e-5eb5-4fb8-a0da-b4e753494cc1"),
                    SrcDocumentUrl = "issue.com",
                    IssueName = "testName",
                    ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                    IssueAttachments = new List<IssueAttachment>
                    {
                        new IssueAttachment{
                            IssueId = Guid.Parse("b740df46-793d-490f-94dc-77525c155de0"),
                            AttachmentUrl = "issueAttachment.com",
                            IsDeleted = false,
                            Tag = "ATTACHMENT"
                        }
                    }
                });

                dbContext.Issues.Add(new Issue
                {
                    IssueId = Guid.Parse("31d3d5e9-1034-4289-ac35-438018708e9b"),
                    JobId = Guid.Parse("5087c2aa-a177-45bf-9b7c-337d5ed171c4"),
                    SrcDocumentUrl = "issue.com",
                    IssueName = "testName",
                    AssigneeId = Guid.Parse("c78d7b35-7fe0-45f3-b171-df0eb48179c5"),
                    IssueAttachments = new List<IssueAttachment>
                    {
                        new IssueAttachment{
                            IssueId = Guid.Parse("31d3d5e9-1034-4289-ac35-438018708e9b"),
                            AttachmentUrl = "issueAttachment2.com",
                            IsDeleted = false,
                            Tag = "ATTACHMENT"
                        }
                    }
                });

                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [TestMethod]
        public async Task GetAllIssue_Linguist()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("c78d7b35-7fe0-45f3-b171-df0eb48179c5"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "LINGUIST"
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, dbContext, currentUser , null);

            mockMapper.Setup(m => m.Map<IEnumerable<IssueResponse>>(It.IsAny<IEnumerable<Issue>>()))
                      .Returns(new List<IssueResponse>
                      {
                          new IssueResponse { IssueId = Guid.NewGuid() }
                      });

            //Act
            var result = issueService.ViewAllIssue();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllIssue_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("c78d7b35-7fe0-45f3-b171-df0eb48179c5"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "LINGUIST"
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, dbContext, currentUser, null);

            mockMapper.Setup(m => m.Map<IEnumerable<IssueResponse>>(It.IsAny<IEnumerable<Issue>>()))
                      .Returns(new List<IssueResponse>
                      {
                      });

            //Act
            var result = issueService.ViewAllIssue();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllIssue_Client()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "CLIENT"
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, dbContext, currentUser, null);

            mockMapper.Setup(m => m.Map<IEnumerable<IssueResponse>>(It.IsAny<IEnumerable<Issue>>()))
                      .Returns(new List<IssueResponse>
                      {
                          new IssueResponse { IssueId = Guid.NewGuid() }
                      });

            //Act
            var result = issueService.ViewAllIssue();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllIssue_EvaludateManager()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "EVALUATE_MANAGER"
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, dbContext, currentUser, null);
            //Act
            var result = issueService.ViewAllIssue();

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAllIssue_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = ""
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, dbContext, currentUser, null);
            //Act
            var result = issueService.ViewAllIssue();
            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task GetAllIssueAttachments()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "CLIENT"
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, dbContext, currentUser, null);

            mockMapper.Setup(m => m.Map<IEnumerable<UploadIssueAttachmentFiles>>(It.IsAny<IEnumerable<IssueAttachment>>()))
                      .Returns(new List<UploadIssueAttachmentFiles>
                      {
                          new UploadIssueAttachmentFiles { AttachmentUrl = "issueAttachment.com", Tag = "ATTACHMENT" },
                          new UploadIssueAttachmentFiles { AttachmentUrl = "issueAttachment2.com", Tag = "ATTACHMENT" }
                      });

            //Act
            var result = issueService.GetAllIssueAttachments();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllIssueAttachments_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "CLIENT"
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, null, currentUser, null);
            //Act
            var result = issueService.GetAllIssueAttachments();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task UpdateIssue()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "CLIENT"
            };

            UpdateIssueRequest request = new UpdateIssueRequest()
            {
                IssueId = Guid.Parse("31d3d5e9-1034-4289-ac35-438018708e9b"),
                IssueName = "UpdatedIssue",
                IssueDescription = "UpdatedIssue",
                AssigneeId = Guid.Parse("c78d7b35-7fe0-45f3-b171-df0eb48179c5"),
                IssueAttachments = null
            };

            var issueService = new IssueServiceImpl(mockMapper.Object, dbContext, currentUser, null);
            //Act
            var result = issueService.UpdateIssue(request);

            //Assert
            var updatedIssue = dbContext.Issues.FirstOrDefault(i => i.IssueId == request.IssueId);
            Assert.AreEqual("UpdatedIssue",updatedIssue.IssueName);
        }
    }
}
