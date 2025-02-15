
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class UpdateRatingValidationTest
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
        public async Task UpdateRating_InvalidIntime()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateRatingValidation(dbContext);
            RatingUpdate ratingCreate = new RatingUpdate
            {
                RatingId = Guid.Parse("348c2dc9-3a0d-4cf2-aa11-93a9ab1df9aa"),
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
        public async Task UpdateRating_ExpectationIntime()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateRatingValidation(dbContext);
            RatingUpdate ratingCreate = new RatingUpdate
            {
                RatingId = Guid.Parse("348c2dc9-3a0d-4cf2-aa11-93a9ab1df9aa"),
                InTime = 4,
                Expectation = 11,
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
        public async Task UpdateRating_IssueResolvedIntime()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateRatingValidation(dbContext);
            RatingUpdate ratingCreate = new RatingUpdate
            {
                RatingId = Guid.Parse("348c2dc9-3a0d-4cf2-aa11-93a9ab1df9aa"),
                InTime = 3,
                Expectation = 4,
                IssueResolved = 11,
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
        public async Task UpdateRating_RatingNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateRatingValidation(dbContext);
            RatingUpdate ratingCreate = new RatingUpdate
            {
                RatingId = Guid.Parse("348c2dc9-3a0d-4cf2-aa11-93a9ab1df9aa"),
                InTime = 3,
                Expectation = 4,
                IssueResolved = 4,
                MoreThought = "good"
            };

            //Act
            List<string> result = await validation.Validate(ratingCreate);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Rating is not found in database"));
            Assert.AreEqual(1, result.Count());
        }
    }
}
