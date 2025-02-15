using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Service;

namespace verbum_service_test.Impl.Service
{
    [TestClass]
    public class ReferenceServiceImplTests
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
        public async Task AreAllUrlsValid_True()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var referenceService = new ReferenceServiceImpl(dbContext, mockMapper.Object);

            List<string> urls = new List<string> { "test.com", "good.com" };

            //Act
            var result = referenceService.AreAllUrlsValid(urls);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task AreAllUrlsValid_False()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var referenceService = new ReferenceServiceImpl(dbContext, mockMapper.Object);

            List<string> urls = new List<string>();

            //Act
            var result = referenceService.AreAllUrlsValid(urls);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public async Task AddRange()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var referenceService = new ReferenceServiceImpl(dbContext, mockMapper.Object);

            List<string> urls = new List<string> { "goodtest.com", "oktest.com" };

            //Act
            referenceService.AddRange(Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"),urls,"TRANSLATION");

            //Assert
            var addedReference = await dbContext.OrderReferences.FirstOrDefaultAsync(c => c.OrderId == Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5") && c.ReferenceFileUrl.Equals("oktest.com"));
            Assert.IsNotNull(addedReference);
        }

        [TestMethod]
        public async Task AddRange_Exception()
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();

            var referenceService = new ReferenceServiceImpl(null, mockMapper.Object);

            List<string> urls = new List<string> { "goodtest.com", "oktest.com" };

            //Act
            referenceService.AddRange(Guid.Parse("e5a521cc-ec2d-4034-bf83-68035577bed5"), urls, "TRANSLATION");

            //Assert
        }
    }
}
