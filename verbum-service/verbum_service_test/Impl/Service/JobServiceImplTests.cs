
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Service;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class JobServiceImplTests
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            if (await dbContext.Jobs.CountAsync() <= 0)
            {
                dbContext.Jobs.Add(new Job
                {
                    Id = Guid.Parse("0db8b89e-5eb5-4fb8-a0da-b4e753494cc1"),
                    Name = "Test",
                    Status = "NEW",
                    WordCount = 100,
                    DocumentUrl = "test.com",
                    TargetLanguageId = "EN",
                    WorkId = Guid.Parse("7ed92254-895a-4644-a96f-fe8d3ab3ae70"),
                    DeliverableUrl = "final.com"
                });

                dbContext.Jobs.Add(new Job
                {
                    Id = Guid.Parse("5087c2aa-a177-45bf-9b7c-337d5ed171c4"),
                    Name = "Test",
                    Status = "NEW",
                    WordCount = 100,
                    DocumentUrl = "test.com",
                    TargetLanguageId = "EN",
                    WorkId = Guid.Parse("7ed92254-895a-4644-a96f-fe8d3ab3ae70"),
                    DeliverableUrl = "final.com"
                });

                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [TestMethod]
        public async Task GetAllJob()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var updateJobValidation = new Mock<UpdateJobValidation>(dbContext);

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "LINGUIST"
            };

            var jobService = new JobServiceImpl(dbContext, mockMapper.Object, updateJobValidation.Object, null,currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<JobListResponse>>(It.IsAny<IEnumerable<Job>>()))
                      .Returns(new List<JobListResponse>
                      {
                          new JobListResponse{ Id = Guid.NewGuid() },
                          new JobListResponse{ Id = Guid.NewGuid() }
                      });

            //Act
            var result = jobService.GetAllJob();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllJob_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var updateJobValidation = new Mock<UpdateJobValidation>(dbContext);

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "LINGUIST"
            };

            var jobService = new JobServiceImpl(dbContext, mockMapper.Object, updateJobValidation.Object, null, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<JobListResponse>>(It.IsAny<IEnumerable<Job>>()))
                      .Returns(new List<JobListResponse>
                      {
                      });

            //Act
            var result = jobService.GetAllJob();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllJob_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var updateJobValidation = new Mock<UpdateJobValidation>(dbContext);

            var jobService = new JobServiceImpl(null, mockMapper.Object, updateJobValidation.Object, null, null);

            //Act
            var result = jobService.GetAllJob();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task GetJobById()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var updateJobValidation = new Mock<UpdateJobValidation>(dbContext);

            var jobService = new JobServiceImpl(dbContext, mockMapper.Object, updateJobValidation.Object, null, null);

            mockMapper.Setup(m => m.Map<JobListResponse>(It.IsAny<IEnumerable<Job>>()))
                      .Returns(new JobListResponse
                      {
                          Id = Guid.Parse("0db8b89e-5eb5-4fb8-a0da-b4e753494cc1")
                      });

            //Act
            var result = jobService.GetJobById(Guid.Parse("0db8b89e-5eb5-4fb8-a0da-b4e753494cc1"));

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetJobById_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var updateJobValidation = new Mock<UpdateJobValidation>(dbContext);

            var jobService = new JobServiceImpl(null, mockMapper.Object, updateJobValidation.Object, null, null);

            //Act
            var result = jobService.GetJobById(Guid.Parse("7357a81d-ca86-40e3-a8c6-4ef9f4134f4f"));

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }
    }
}
