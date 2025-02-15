using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class UpdateJobValidationTest
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
        public async Task UpdateJobValidationTest_DueDateInvalid()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateJobValidation(dbContext);

            UpdateJobRequest request = new UpdateJobRequest
            {
                DueDate = DateTime.Now.AddDays(-10),
                Status = "NEW"
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("due date is invalid"));
        }

        [TestMethod]
        public async Task UpdateJobValidationTest_InvalidJob()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateJobValidation(dbContext);

            UpdateJobRequest request = new UpdateJobRequest
            {
                DueDate = DateTime.Now,
                Status = "Error"
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("job status is invalid"));
        }
    }
}
