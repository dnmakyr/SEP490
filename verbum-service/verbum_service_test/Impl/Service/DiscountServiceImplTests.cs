using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_infrastructure.DataContext;
using verbum_service_domain.Models;
using AutoMapper;
using Moq;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Response;
using verbum_service_infrastructure.Impl.Service;
using verbum_service_infrastructure.Impl.Validation;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Common.ErrorModel;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class DiscountServiceImplTests
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            if(await dbContext.Discounts.CountAsync() <= 0)
            {
                for (int i = 1; i <= 2; i++)
                {
                    dbContext.Discounts.Add(new Discount
                    {
                        DiscountId = Guid.NewGuid(),
                        DiscountName = "discount",
                        DiscountPercent = 50
                    });
                }
            }

            return dbContext;
        }

        [TestMethod]
        public async Task GetAllDiscount()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(dbContext);

            var discountService = new DiscountServiceImpl(mockMapper.Object, dbContext, saveDiscountValidation.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<DiscountResponse>>(It.IsAny<IEnumerable<Discount>>()))
                      .Returns(new List<DiscountResponse>
                      {
                          new DiscountResponse{ DiscountId = Guid.NewGuid() },
                          new DiscountResponse { DiscountId = Guid.NewGuid() },
                      });

            //Act
            var result = discountService.GetAllDiscount();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllDiscount_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(dbContext);

            var discountService = new DiscountServiceImpl(mockMapper.Object, dbContext, saveDiscountValidation.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<DiscountResponse>>(It.IsAny<IEnumerable<Discount>>()))
                      .Returns(new List<DiscountResponse>
                      {
                      });

            //Act
            var result = discountService.GetAllDiscount();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllDiscount_Excepiton()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(dbContext);

            var discountService = new DiscountServiceImpl(mockMapper.Object, null, saveDiscountValidation.Object);

            //Act
            var result = discountService.GetAllDiscount();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task AddDiscount()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(dbContext);

            var discountService = new DiscountServiceImpl(mockMapper.Object, dbContext, saveDiscountValidation.Object);

            DiscountDTO request = new DiscountDTO
            {
                DiscountId = Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"),
                DiscountPercent = 80,
                DiscountName = "addDiscount",
                IsUpdate = false
            };

            mockMapper.Setup(m => m.Map<Discount>(It.IsAny<DiscountDTO>()))
                      .Returns(new Discount
                      {
                          DiscountId = Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"),
                          DiscountPercent = 80,
                          DiscountName = "addDiscount",
                      });

            //Act
            var result = discountService.AddDiscount(request);

            //Assert
            var addedDiscount = await dbContext.Discounts.FirstOrDefaultAsync(c => c.DiscountId == Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"));
            Assert.IsNotNull(addedDiscount);
        }

        [TestMethod]
        public async Task AddDiscount_ErrorValidation()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(null);

            var discountService = new DiscountServiceImpl(mockMapper.Object, null, saveDiscountValidation.Object);

            DiscountDTO request = new DiscountDTO();

            //Act
            //Assert
            discountService.AddDiscount(request);
        }

        [TestMethod]
        public async Task UpdateDiscount()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(dbContext);

            var discountService = new DiscountServiceImpl(mockMapper.Object, dbContext, saveDiscountValidation.Object);

            DiscountDTO request = new DiscountDTO
            {
                DiscountId = Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"),
                DiscountPercent = 90,
                DiscountName = "addDiscount",
                IsUpdate = true
            };

            mockMapper.Setup(m => m.Map<Discount>(It.IsAny<DiscountDTO>()))
                      .Returns(new Discount
                      {
                          DiscountId = Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"),
                          DiscountPercent = 90,
                          DiscountName = "newaddDiscount",
                      });

            //Act
            //Assert
            await discountService.UpdateDiscount(request);
            var addedDiscount = await dbContext.Discounts.FirstOrDefaultAsync(c => c.DiscountId == Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"));
            Assert.AreEqual(90,addedDiscount.DiscountPercent);
        }

        [TestMethod]
        public async Task UpdateDiscount_ErrorValidation()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(dbContext);

            var discountService = new DiscountServiceImpl(mockMapper.Object, dbContext, saveDiscountValidation.Object);

            DiscountDTO request = new DiscountDTO();

            //Act
            //Assert
            discountService.UpdateDiscount(request);
        }

        [TestMethod]
        public async Task UpdateDiscount_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var saveDiscountValidation = new Mock<SaveDiscountValidation>(dbContext);

            var discountService = new DiscountServiceImpl(mockMapper.Object, dbContext, saveDiscountValidation.Object);

            DiscountDTO request = new DiscountDTO
            {
                DiscountId = Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"),
                DiscountPercent = 100,
                DiscountName = "addDiscount",
                IsUpdate = true
            };

            mockMapper.Setup(m => m.Map<Discount>(It.IsAny<DiscountDTO>()))
                      .Returns(new Discount
                      {
                          DiscountId = Guid.Parse("e522870d-3976-4afe-b2fc-9918acedf316"),
                          DiscountPercent = 100,
                          DiscountName = "addDiscount",
                      });

            //Act
            //Assert
            discountService.UpdateDiscount(request);
        }
    }
}
