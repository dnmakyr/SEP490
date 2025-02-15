using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_infrastructure.DataContext;
using verbum_service_domain.Models;
using AutoMapper;
using Moq;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Response;
using verbum_service_infrastructure.Impl.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using verbum_service_application.Service;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class WorkServiceImplTests
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            if(await dbContext.Works.CountAsync() <= 0)
            {
                dbContext.Works.Add(new Work
                {
                    WorkId = Guid.Parse("7ed92254-895a-4644-a96f-fe8d3ab3ae70"),
                    OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                    ServiceCode = "TL"
                });

                dbContext.Works.Add(new Work
                {
                    WorkId = Guid.Parse("d4ea0069-55f4-4850-a693-490033c3f692"),
                    OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                    ServiceCode = "ED"
                });

                dbContext.Works.Add(new Work
                {
                    WorkId = Guid.Parse("ff21791c-03bc-4b38-8705-9755fafa0c9f"),
                    OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                    ServiceCode = "EV"
                });

                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [TestMethod]
        public async Task GetAllWork_TranslateManager()
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
                Role = "TRANSLATE_MANAGER"
            };

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            //Act
            var result = workService.GetAllWork();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllWork_Empty()
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
                Role = "TRANSLATE_MANAGER"
            };

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                      });

            //Act
            var result = workService.GetAllWork();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllWork_EditManager()
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
                Role = "EDIT_MANAGER"
            };

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            //Act
            var result = workService.GetAllWork();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllWork_EvaluateManager()
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
                Role = "EVALUATE_MANAGER"
            };

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            //Act
            var result = workService.GetAllWork();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllWork_Linguist()
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
                Role = "LINGUIST"
            };

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() },
                          new WorkResponse{ WorkId = Guid.NewGuid() },
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            //Act
            var result = workService.GetAllWork();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllWork_RoleException()
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
                Role = "DEFAULT"
            };

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            //Act
            var result = workService.GetAllWork();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task GetAllWork_Exception()
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
                Role = "LINGUIST"
            };

            var workService = new WorkServiceImpl(null, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            //Act
            var result = workService.GetAllWork();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task AddWorkCategory()
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
                Role = "LINGUIST"
            };

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            List<int> categoryIds = new List<int> { 1 };

            //Act
            await workService.AddWorkCategory(Guid.Parse("ff21791c-03bc-4b38-8705-9755fafa0c9f"), categoryIds);

            //Assert
            Work workCategory = await dbContext.Works.FirstOrDefaultAsync(w => w.WorkId == Guid.Parse("ff21791c-03bc-4b38-8705-9755fafa0c9f"));
            Assert.IsNotNull(workCategory);
        }

        [TestMethod]
        public async Task AddWorkCategory_Exception()
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
                Role = "LINGUIST"
            };

            var workService = new WorkServiceImpl(null, mockMapper.Object, currentUser);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            List<int> categoryIds = new List<int> { 1 };

            //Act
            //Assert
            var exception = await Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
            {
                await workService.AddWorkCategory(Guid.Parse("ff21791c-03bc-4b38-8705-9755fafa0c9f"), categoryIds);
            });

        }

        [TestMethod]
        public async Task AddRange()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, null);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            List<string> serviceList = new List<string> { "EV" };

            //Act
            await workService.AddRange(Guid.Parse("ff21791c-03bc-4b38-8705-9755fafa0c9f"), DateTime.Now, serviceList);
            //Assert
            var count = dbContext.Works.ToList().Count;
            Assert.IsTrue(count > 3);
        }

        [TestMethod]
        public async Task AddRange_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var workService = new WorkServiceImpl(null, mockMapper.Object, null);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            List<string> serviceList = new List<string> { "EV" };

            //Act
            //Assert
            var exception = await Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
            {
                await workService.AddRange(Guid.Parse("ff21791c-03bc-4b38-8705-9755fafa0c9f"), DateTime.Now, serviceList);
            });
        }

        [TestMethod]
        public async Task GenerateWork()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var workService = new WorkServiceImpl(dbContext, mockMapper.Object, null);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            verbum_service_domain.DTO.Request.GenerateWork generateWork = new verbum_service_domain.DTO.Request.GenerateWork()
            {
                OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                OrderName = "Test",
                DueDate = DateTime.Now,
                HasTranslateService = true,
                HasEditService = true,
                HasEvaluateService = true
            };

            Category general = new Category()
            {
                CategoryId = 6,
                CategoryName = "General"
            };

            var generalCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == 6);
            if(generalCategory == null)
            {
                dbContext.Categories.Add(general);
                await dbContext.SaveChangesAsync();
            }

            //Act
            await workService.GenerateWork(generateWork);
            //Assert
            var count = dbContext.Works.ToList().Count;
            Assert.IsTrue(count > 4);
        }

        [TestMethod]
        public async Task GenerateWork_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var workService = new WorkServiceImpl(null, mockMapper.Object, null);

            mockMapper.Setup(m => m.Map<IEnumerable<WorkResponse>>(It.IsAny<IEnumerable<Work>>()))
                      .Returns(new List<WorkResponse>
                      {
                          new WorkResponse{ WorkId = Guid.NewGuid() }
                      });

            verbum_service_domain.DTO.Request.GenerateWork generateWork = new verbum_service_domain.DTO.Request.GenerateWork()
            {
                OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                OrderName = "Test",
                DueDate = DateTime.Now,
                HasTranslateService = false,
                HasEditService = false,
                HasEvaluateService = false
            };

            Category general = new Category()
            {
                CategoryId = 6,
                CategoryName = "General"
            };

            var generalCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == 6);
            if (generalCategory == null)
            {
                dbContext.Categories.Add(general);
                await dbContext.SaveChangesAsync();
            }

            //Act
            //Assert
            var exception = await Assert.ThrowsExceptionAsync<BusinessException>(async () =>
            {
                await workService.GenerateWork(generateWork);
            });
        }
    }
}
