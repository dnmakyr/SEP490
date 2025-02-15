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

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class ReceiptServiceImplTests
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
                dbContext.Orders.Add(new Order
                {
                    OrderId = Guid.Parse("551cc0f7-4600-4b69-a07f-44b7817b3e30"),
                    ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                    SourceLanguageId = "EN",
                    HasEditService = true,
                    HasTranslateService = true,
                    HasEvaluateService = true
                });
                await dbContext.SaveChangesAsync();
            }

            if (await dbContext.Receipts.CountAsync() <= 0)
            {
                for (int i = 1; i <= 2; i++)
                {
                    dbContext.Receipts.Add(new Receipt
                    {
                        ReceiptId = Guid.NewGuid(),
                        OrderId = Guid.Parse("551cc0f7-4600-4b69-a07f-44b7817b3e30"),
                        DepositeOrPayment = true,
                        Amount = 100,
                        PaymentId = "PAYID-M5BVLEQ5U784141KJ729161G"
                    });
                }
                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [TestMethod]
        public async Task GetAllReceiptTest()
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

            mockMapper.Setup(m => m.Map<IEnumerable<ReceiptInfoResponse>>(It.IsAny<IEnumerable<Receipt>>()))
                      .Returns(new List<ReceiptInfoResponse>
                      {
                          new ReceiptInfoResponse { OrderId = Guid.Parse("551cc0f7-4600-4b69-a07f-44b7817b3e30") },
                          new ReceiptInfoResponse { OrderId = Guid.Parse("551cc0f7-4600-4b69-a07f-44b7817b3e30") },
                      });

            var receiptServiceImpl = new ReceiptServiceImpl(dbContext, mockMapper.Object, currentUser);

            //Act
            var result = receiptServiceImpl.GetAllReceipt();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllReceiptTest_Empty()
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

            mockMapper.Setup(m => m.Map<IEnumerable<ReceiptInfoResponse>>(It.IsAny<IEnumerable<Receipt>>()))
                      .Returns(new List<ReceiptInfoResponse>
                      {
                      });

            var receiptServiceImpl = new ReceiptServiceImpl(dbContext, mockMapper.Object, currentUser);

            //Act
            var result = receiptServiceImpl.GetAllReceipt();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllReceiptTest_Exception()
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();

            var currentUser = new CurrentUser
            {
                Id = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                Email = "test@example.com",
                Name = "Test User",
                Status = "Active",
                Role = "CLIENT"
            };

            var receiptServiceImpl = new ReceiptServiceImpl(null, mockMapper.Object, currentUser);

            //Act
            var result = receiptServiceImpl.GetAllReceipt();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

    }
}
