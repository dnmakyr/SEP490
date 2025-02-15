using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Service;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class TokenServiceImplTests
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
        public async Task AddRefreshToken()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var tokenService = new TokenServiceImpl(dbContext);
            //Act
            Tokens tokens = new Tokens()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken"
            };

            var result = tokenService.AddRefreshToken(tokens.RefreshToken);
            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task AddRefreshToken_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var tokenService = new TokenServiceImpl(null);
            //Act
            Tokens tokens = new Tokens()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken"
            };

            var result = tokenService.AddRefreshToken(tokens.RefreshToken);
            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }
    }
}
