using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class CreateRatingValidationTest
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
        public async Task CreateRating_InvalidIntime()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateRatingValidation(dbContext);
            RatingCreate ratingCreate = new RatingCreate
            {
                OrderId = Guid.Parse("b72fc935-83d6-434e-bfef-2f0c07cdab4b"),
                InTime = 6,
                Expectation = 4,
                IssueResolved = 4,
                MoreThought = "good"
            };

            //Act
            List<string> result = await validation.Validate(ratingCreate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Rating InTime is invalid"));
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task CreateRating_InvalidExpectation()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateRatingValidation(dbContext);
            RatingCreate ratingCreate = new RatingCreate
            {
                OrderId = Guid.Parse("b72fc935-83d6-434e-bfef-2f0c07cdab4b"),
                InTime = 3,
                Expectation = 8,
                IssueResolved = 4,
                MoreThought = "good"
            };

            //Act
            List<string> result = await validation.Validate(ratingCreate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Rating Expectation is invalid"));
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task CreateRating_InvalidIssueResolved()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateRatingValidation(dbContext);
            RatingCreate ratingCreate = new RatingCreate
            {
                OrderId = Guid.Parse("b72fc935-83d6-434e-bfef-2f0c07cdab4b"),
                InTime = 3,
                Expectation = 4,
                IssueResolved = 10,
                MoreThought = "good"
            };

            //Act
            List<string> result = await validation.Validate(ratingCreate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Rating IssueResolved is invalid"));
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task CreateRating_OrderNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateRatingValidation(dbContext);
            RatingCreate ratingCreate = new RatingCreate
            {
                OrderId = Guid.Parse("b72fc935-83d6-434e-bfef-2f0c07cdab4b"),
                InTime = 3,
                Expectation = 4,
                IssueResolved = 4,
                MoreThought = "good"
            };

            //Act
            List<string> result = await validation.Validate(ratingCreate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Order is not found in database"));
            Assert.AreEqual(1, result.Count());
        }
    }
}
