using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class SaveDiscountValidationTest
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        [TestMethod]
        public async Task SaveDiscountValidation_DiscountPercentEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new SaveDiscountValidation(dbContext);

            DiscountDTO discountDTO = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountName = "discount",
                IsUpdate = true
            };

            //Act
            List<string> result = await validation.Validate(discountDTO);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("discount percent is required"));
        }

        [TestMethod]
        public async Task SaveDiscountValidation_DiscountNameEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new SaveDiscountValidation(dbContext);

            DiscountDTO discountDTO = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercent = 50,
                DiscountName = "",
                IsUpdate = true
            };

            //Act
            List<string> result = await validation.Validate(discountDTO);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("discount name is required"));
        }

        [TestMethod]
        public async Task SaveDiscountValidation_DiscountPercentInvalid()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new SaveDiscountValidation(dbContext);

            DiscountDTO discountDTO = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercent = 100,
                DiscountName = "discount",
                IsUpdate = true
            };

            //Act
            List<string> result = await validation.Validate(discountDTO);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("discount percentage is invalid"));
        }
    }
}
