using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class UpdateCategoryValidationTest
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
        public async Task UpdateCategoryValidation_IdEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var validation = new UpdateCategoryValidation(dbContext);

            CategoryUpdate categoryUpdate = new CategoryUpdate
            {
                Id = 0,
                Name = "Something"
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
            List<string> result = await validation.Validate(categoryUpdate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("CategoryId is required"));
        }

        [TestMethod]
        public async Task UpdateCategoryValidation_NameEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var validation = new UpdateCategoryValidation(dbContext);

            CategoryUpdate categoryUpdate = new CategoryUpdate
            {
                Id = 100,
                Name = ""
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
            List<string> result = await validation.Validate(categoryUpdate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("CategoryName is required"));
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task UpdateCategoryValidation_Dupplicate()
        {
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var validation = new UpdateCategoryValidation(dbContext);

            CategoryUpdate categoryUpdate = new CategoryUpdate
            {
                Id = 100,
                Name = "Dupplicate"
            };

            Category general = new Category()
            {
                CategoryId = 35,
                CategoryName = "Dupplicate"
            };

            var generalCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == 35);
            if (generalCategory == null)
            {
                dbContext.Categories.Add(general);
                await dbContext.SaveChangesAsync();
            }

            //Act
            List<string> result = await validation.Validate(categoryUpdate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Category is already in database"));
        }

        [TestMethod]
        public async Task UpdateCategoryValidation_Exist()
        {
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var validation = new UpdateCategoryValidation(dbContext);

            CategoryUpdate categoryUpdate = new CategoryUpdate
            {
                Id = 100,
                Name = "Chicken"
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
            List<string> result = await validation.Validate(categoryUpdate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Category is not found in database"));
            Assert.AreEqual(1, result.Count());
        }
    }
}
