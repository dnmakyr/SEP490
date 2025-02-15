using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class DeleteCategoryValidationTest
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
        public async Task DeleteCategory_EmptyId()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new DeleteCategoryValidation(dbContext);

            CategoryDelete categoryDelete = new CategoryDelete
            {
                Id = 0
            };

            //Act
            List<string> result = await validation.Validate(categoryDelete);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("CategoryId is required"));
            Assert.IsFalse(result.Contains("Exist associated works is invalid"));
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task DeleteCategory_Exist()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new DeleteCategoryValidation(dbContext);

            CategoryDelete categoryDelete = new CategoryDelete
            {
                Id = 77
            };

            //Act
            List<string> result = await validation.Validate(categoryDelete);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Category is not found in database"));
            Assert.IsFalse(result.Contains("Exist associated works is invalid"));
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task DeleteCategory_WorkExist()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new DeleteCategoryValidation(dbContext);

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

            var general2 = dbContext.Categories.Where(c => c.CategoryId == 6).ToList();

            Work work = new Work()
            {
                WorkId = Guid.Parse("d83fe577-b134-45ad-a204-e0514837cc0c"),
                OrderId = Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),
                ServiceCode = "TL",
                Categories = general2
            };

            var checkWork = await dbContext.Works.FirstOrDefaultAsync(c => c.WorkId.ToString() == "d83fe577-b134-45ad-a204-e0514837cc0c");
            if (checkWork == null)
            {
                dbContext.Works.Add(work);
                await dbContext.SaveChangesAsync();
            }

            CategoryDelete categoryDelete = new CategoryDelete
            {
                Id = 6
            };

            //Act
            List<string> result = await validation.Validate(categoryDelete);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Exist associated works is invalid"));
            Assert.AreEqual(1, result.Count());
        }
    }
}
