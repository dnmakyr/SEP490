using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_infrastructure.Impl.Service;
using Moq;
using verbum_service_infrastructure.DataContext;
using verbum_service_domain.DTO.Request;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using verbum_service_domain.Common;
using verbum_service_domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using AutoMapper;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using verbum_service_application.Service;

namespace verbum_service_test
{
    [TestClass]
    public class OrderServiceImplTests
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            if (await dbContext.Languages.CountAsync() <= 0)
            {
                dbContext.Languages.Add(new Language
                {
                    LanguageId = "EN",
                    LanguageName = "English",
                    Support = true
                });
                await dbContext.SaveChangesAsync();
            }

            if (await dbContext.Roles.CountAsync() <= 0)
            {
                dbContext.Roles.Add(new Role
                {
                    RoleId = "CLIENT",
                    RoleName = "Client"
                });
                await dbContext.SaveChangesAsync();
            }

            if (await dbContext.Users.CountAsync() <= 0)
            {
                dbContext.Users.Add(new User
                {
                    Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                    Name = "Tuan",
                    Email = "tuan@gmail.com",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = "ACTIVE",
                    RoleCode = "CLIENT"
                });
                await dbContext.SaveChangesAsync();
            }

            if (await dbContext.Orders.CountAsync() <= 0)
            {
                for(int i = 1; i <= 2; i++)
                {
                    dbContext.Orders.Add(new Order
                    {
                        OrderId = Guid.NewGuid(),
                        ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                        SourceLanguageId = "EN",
                        HasEditService = true,
                        HasTranslateService = true,
                        HasEvaluateService = true
                    });
                }
                
                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        //[TestCleanup]
        //public void Cleanup()
        //{
        //    context.Database.EnsureDeleted();
        //    context.Dispose();
        //}

        [TestMethod]
        public async Task CreateOrder()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext,mockMapper.Object,mockCurrentUser.Object, null, null, mailService.Object);

            var newOrder = new Order
            {
                OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                SourceLanguageId = "EN",
                HasEditService = true,
                HasTranslateService = true,
                HasEvaluateService = true
            };

            //Act
            orderService.CreateOrder(newOrder);

            //Assert
            var addedOrder = await dbContext.Orders.FirstOrDefaultAsync(c => c.OrderId == Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"));
            Assert.IsNotNull(addedOrder);
        }

        [TestMethod]
        public async Task CreateOrder_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();
            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(null, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            var newOrder = new Order
            {
                OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                SourceLanguageId = "EN",
                HasEditService = true,
                HasTranslateService = true,
                HasEvaluateService = true
            };

            //Act
            //Assert
            orderService.CreateOrder(newOrder);
        }

        [TestMethod]
        public async Task GetAllOrder_Client()
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
            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext, mockMapper.Object, currentUser, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<OrderDetailsResponse>>(It.IsAny<IEnumerable<Order>>()))
                      .Returns(new List<OrderDetailsResponse>
                      {
                          new OrderDetailsResponse{ OrderId = Guid.NewGuid() },
                          new OrderDetailsResponse { OrderId = Guid.NewGuid() },
                      });

            //Act
            var result = orderService.GetAllOrder();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllOrder_Empty()
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
            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext, mockMapper.Object, currentUser, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<OrderDetailsResponse>>(It.IsAny<IEnumerable<Order>>()))
                      .Returns(new List<OrderDetailsResponse>
                      {
                      });

            //Act
            var result = orderService.GetAllOrder();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllOrder_EvaluateManager()
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

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext, mockMapper.Object, currentUser, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<OrderDetailsResponse>>(It.IsAny<IEnumerable<Order>>()))
                      .Returns(new List<OrderDetailsResponse>
                      {
                          new OrderDetailsResponse{ OrderId = Guid.NewGuid() },
                          new OrderDetailsResponse { OrderId = Guid.NewGuid() },
                      });

            //Act
            var result = orderService.GetAllOrder();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllOrder_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(null, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<OrderDetailsResponse>>(It.IsAny<IEnumerable<Order>>()))
                      .Returns(new List<OrderDetailsResponse>
                      {
                          new OrderDetailsResponse{ OrderId = Guid.NewGuid() },
                          new OrderDetailsResponse { OrderId = Guid.NewGuid() },
                      });

            //Act
            var result = orderService.GetAllOrder();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task GetOrderDetails()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            mockMapper.Setup(m => m.Map<OrderDetailsResponse>(It.IsAny<IEnumerable<Order>>()))
                      .Returns(new OrderDetailsResponse
                      {
                          OrderId = Guid.NewGuid()
                      });

            //Act
            var result = orderService.GetOrderDetails(Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"));

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetOrderDetails_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(null, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            //Act
            var result = orderService.GetOrderDetails(Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed9"));

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task AddRangeMiddle()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            var newOrder = new Order
            {
                OrderId = Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"),
                ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                SourceLanguageId = "EN",
                HasEditService = true,
                HasTranslateService = true,
                HasEvaluateService = true
            };

            List<string> languageList = new List<string> { "EN" };

            dbContext.Orders.Add(newOrder);

            //Act
            orderService.AddRangeMiddle(Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"), languageList);

            //Assert
            var addedRangeMiddle = await dbContext.Orders.FirstOrDefaultAsync(c => c.OrderId == Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"));
            Assert.IsNotNull(addedRangeMiddle.TargetLanguages);
            Assert.AreEqual(1, addedRangeMiddle.TargetLanguages.Count());
        }

        [TestMethod]
        public async Task AddRangeMiddle_Exception()
        {

            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();
            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(null, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            var newOrder = new Order
            {
                OrderId = Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"),
                ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                SourceLanguageId = "EN",
                HasEditService = true,
                HasTranslateService = true,
                HasEvaluateService = true
            };

            List<string> languageList = new List<string> { "AB" };

            dbContext.Orders.Add(newOrder);

            //Act
            orderService.AddRangeMiddle(Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"), languageList);

            //Assert
        }

        [TestMethod]
        public async Task UpdateOrderTargetLanguage()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            List<string> languageList = new List<string> { "EN" };

            var request = new OrderUpdate
            {
                OrderId = Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"),
                TargetLanguageIdList = languageList
            };

            //Act
            await orderService.UpdateOrderTargetLanguage(request);

            //Assert
            var addedRangeMiddle = await dbContext.Orders.FirstOrDefaultAsync(c => c.OrderId == Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"));
            Assert.IsNotNull(addedRangeMiddle.TargetLanguages);
            Assert.AreEqual(1, addedRangeMiddle.TargetLanguages.Count());
        }

        [TestMethod]
        public async Task GetAllOrderReferenceFile()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var orderReference = new OrderReference
            {
                OrderId = Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"),
                Tag = "TRANSLATION",
                IsDeleted = false
            };

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(dbContext, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<UploadOrderFileRequest>>(It.IsAny<IEnumerable<OrderReference>>()))
                      .Returns(new List<UploadOrderFileRequest>
                      {
                          new UploadOrderFileRequest{ OrderId = Guid.NewGuid() },
                      });

            //Act
            var result = orderService.GetAllOrderRefrenceFiles();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllOrderReferenceFile_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var orderReference = new OrderReference
            {
                OrderId = Guid.Parse("5300c159-8a4c-4721-9379-4d10171f25bd"),
                Tag = "TRANSLATION",
                IsDeleted = false
            };

            var mockIConfiguration = new Mock<IConfiguration>();
            var mockIhttpcontextAccessor = new Mock<IHttpContextAccessor>();
            var mailService = new Mock<MailService>();

            var orderService = new OrderServiceImpl(null, mockMapper.Object, mockCurrentUser.Object, mockIConfiguration.Object, mockIhttpcontextAccessor.Object, mailService.Object);

            //Act
            var result = orderService.GetAllOrderRefrenceFiles();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }
    }
}