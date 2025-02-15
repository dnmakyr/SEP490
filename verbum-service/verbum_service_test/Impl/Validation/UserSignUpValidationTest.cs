using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class UserSignUpValidationTest
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
        public async Task UserSignUp_RoleInvalid()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UserSignUpValidation(dbContext);

            UserSignUp userSignUp = new UserSignUp
            {
                Name = "NewUser",
                Email = "newuser@gmail.com",
                Password = "Password1!",
                RoleCode = "ERROR"
            };

            //Act
            List<string> result = await validation.Validate(userSignUp);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Role ERROR is invalid"));
        }

        [TestMethod]
        public async Task UserSignUp_PasswordInvalid()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UserSignUpValidation(dbContext);

            UserSignUp userSignUp = new UserSignUp
            {
                Name = "NewUser",
                Email = "newuser@gmail.com",
                Password = "password123",
                RoleCode = "CLIENT"
            };

            //Act
            List<string> result = await validation.Validate(userSignUp);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Password is invalid"));
        }

        [TestMethod]
        public async Task UserSignUp_EmailInvalid()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UserSignUpValidation(dbContext);

            UserSignUp userSignUp = new UserSignUp
            {
                Name = "NewUser",
                Email = "newuser@gmail.com.",
                Password = "password123",
                RoleCode = "CLIENT"
            };

            //Act
            List<string> result = await validation.Validate(userSignUp);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("this email is invalid"));
        }

        [TestMethod]
        public async Task UserSignUp_EmailDupplicate()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UserSignUpValidation(dbContext);

            UserSignUp userSignUp = new UserSignUp
            {
                Name = "NewUser",
                Email = "tuan@gmail.com",
                Password = "Password1!",
                RoleCode = "CLIENT"
            };

            //Act
            List<string> result = await validation.Validate(userSignUp);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("this email is already in database"));
        }
    }
}
