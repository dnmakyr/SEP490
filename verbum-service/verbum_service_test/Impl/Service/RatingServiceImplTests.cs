using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Service;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class RatingServiceImplTests
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            if(await dbContext.Languages.CountAsync() <= 0)
            {
                dbContext.Languages.Add(new Language
                {
                    LanguageId = "EN",
                    LanguageName = "English",
                    Support = true
                });
                await dbContext.SaveChangesAsync();
            }

            if(await dbContext.Roles.CountAsync() <= 0)
            {
                dbContext.Roles.Add(new Role
                {
                    RoleId = "CLIENT",
                    RoleName = "Client"
                });
                await dbContext.SaveChangesAsync();
            }

            if(await dbContext.Users.CountAsync() <= 0)
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

            if(await dbContext.Orders.CountAsync() <= 0)
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

            if(await dbContext.Ratings.CountAsync() <= 0)
            {
                for (int i = 1; i <= 3; i++)
                {
                    dbContext.Ratings.Add(new Rating
                    {
                        RatingId = Guid.NewGuid(),
                        OrderId = Guid.Parse("551cc0f7-4600-4b69-a07f-44b7817b3e30"),
                        InTime = 3,
                        Expectation = 3,
                        IssueResolved = 3,
                        MoreThought = "Good"
                    });
                }
                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [TestMethod]
        public async Task GetAllRatingTest()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            mockMapper.Setup(m => m.Map<IEnumerable<RatingResponse>>(It.IsAny<IEnumerable<Rating>>()))
                      .Returns(new List<RatingResponse>
                      {
                          new RatingResponse { RatingId = Guid.NewGuid() },
                          new RatingResponse { RatingId = Guid.NewGuid() },
                          new RatingResponse { RatingId = Guid.NewGuid() }
                      });

            var ratingServiceImpl = new RatingServiceImpl(dbContext, mockMapper.Object, mockCurrentUser.Object);

            //Act
            var result = ratingServiceImpl.GetAllRating();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllRatingTest_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            mockMapper.Setup(m => m.Map<IEnumerable<RatingResponse>>(It.IsAny<IEnumerable<Rating>>()))
                      .Returns(new List<RatingResponse>
                      {
                      });

            var ratingServiceImpl = new RatingServiceImpl(dbContext, mockMapper.Object, mockCurrentUser.Object);

            //Act
            var result = ratingServiceImpl.GetAllRating();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllRatingTest_Exception()
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var ratingServiceImpl = new RatingServiceImpl(null, mockMapper.Object, mockCurrentUser.Object);

            //Act
            var result = ratingServiceImpl.GetAllRating();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task CreateRatingTest()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var mockCurrentUser = new Mock<CurrentUser>();

            var ratingServiceImpl = new RatingServiceImpl(dbContext, mockMapper.Object, mockCurrentUser.Object);

            var newRating = new Rating
            {
                RatingId = Guid.Parse("dd310342-59b1-4b45-a2de-9cce64661a33"),
                OrderId = Guid.Parse("551cc0f7-4600-4b69-a07f-44b7817b3e30"),
                InTime = 4,
                Expectation = 4,
                IssueResolved = 4,
                MoreThought = "Super"
            };

            //Act
            ratingServiceImpl.CreateRating(newRating);

            //Assert
            var addedRating = await dbContext.Ratings.FirstOrDefaultAsync(c => c.RatingId == Guid.Parse("dd310342-59b1-4b45-a2de-9cce64661a33"));
            Assert.IsNotNull(addedRating);
            Assert.AreEqual("Super", addedRating.MoreThought);
        }
    }
}
