using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Service;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class UserServiceImplTests
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
        public async Task GetAssignList()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var tokenService = new Mock<TokenService>();

            var userService = new UserServiceImpl(dbContext, mockMapper.Object, tokenService.Object, null, null);

            mockMapper.Setup(m => m.Map<IEnumerable<UserInfo>>(It.IsAny<IEnumerable<User>>()))
                      .Returns(new List<UserInfo>
                      {
                          new UserInfo{ Id = Guid.NewGuid() },
                          new UserInfo{ Id = Guid.NewGuid() }
                      });

            //Act
            var result = userService.GetAssignList();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAssignList_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var tokenService = new Mock<TokenService>();

            var userService = new UserServiceImpl(null, mockMapper.Object, tokenService.Object, null, null);

            //Act
            var result = userService.GetAssignList();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task LoginGoogleNewUser()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var tokenService = new Mock<TokenService>();

            var userService = new UserServiceImpl(dbContext, mockMapper.Object, tokenService.Object, null, null);

            //Act
            var result = userService.LoginGoogleNewUser("newuser11@gmail.com", "John Doe");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, dbContext.Users.Where(u => u.Email == "newuser11@gmail.com").ToList().Count());
        }

        [TestMethod]
        public async Task LoginGoogleNewUser_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var tokenService = new Mock<TokenService>();

            var userService = new UserServiceImpl(null, mockMapper.Object, tokenService.Object, null, null);

            //Act
            var result = userService.LoginGoogleNewUser("newuser11@gmail.com", "John Doe");

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task LoginGoogleOldUser()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var tokenService = new Mock<TokenService>();

            var userService = new UserServiceImpl(dbContext, mockMapper.Object, tokenService.Object, null, null);

            User oldUser = new User()
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "newuser11@gmail.com",
                Status = "ACTIVE",
                RoleCode = "CLIENT"
            };

            //Act
            var result = userService.LoginGoogleOldUser(oldUser);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task LoginGoogleOldUser_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var tokenService = new Mock<TokenService>();

            var userService = new UserServiceImpl(null, mockMapper.Object, tokenService.Object, null, null);

            User oldUser = new User()
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "newuser11@gmail.com",
                Status = "ACTIVE",
                RoleCode = "CLIENT"
            };

            //Act
            var result = userService.LoginGoogleOldUser(oldUser);

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }
    }
}
