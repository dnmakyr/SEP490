using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class CreateCategoryValidationTest
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
        public async Task CreateCategoryValidation_Empty()
        {
            var dbContext = await GetDatabaseContext();

            var validation = new CreateCategoryValidation(dbContext);

            CategoryInfo categoryInfo = new CategoryInfo
            {
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
            List<string> result = await validation.Validate(categoryInfo);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("CategoryName is required"));
            Assert.IsTrue(result.Contains("Category name must be between 1 to 30 characters is invalid"));
            Assert.AreEqual(2, result.Count());
        }


        [TestMethod]
        public async Task CreateCategoryValidation_Dupplicate()
        {
            var dbContext = await GetDatabaseContext();

            var validation = new CreateCategoryValidation(dbContext);

            CategoryInfo categoryInfo = new CategoryInfo
            {
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
            List<string> result = await validation.Validate(categoryInfo);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Category is already in database"));
        }

        [TestMethod]
        public async Task CreateCategoryValidation_Format()
        {
            var dbContext = await GetDatabaseContext();

            var validation = new CreateCategoryValidation(dbContext);

            CategoryInfo categoryInfo = new CategoryInfo
            {
                Name = "General!!"
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
            List<string> result = await validation.Validate(categoryInfo);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Category name must only contain letter or digits is invalid"));
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task CreateCategoryValidation_LongFormat()
        {
            var dbContext = await GetDatabaseContext();

            var validation = new CreateCategoryValidation(dbContext);

            CategoryInfo categoryInfo = new CategoryInfo
            {
                Name = "ThisCategoryIsWrongCategoryBecauseTheNameIsTooLong"
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
            List<string> result = await validation.Validate(categoryInfo);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Category name must be between 1 to 30 characters is invalid"));
            Assert.AreEqual(1, result.Count());
        }
    }
}
