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
    public class LanguageServiceImplTests
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
        public async Task GetAllSupportedLanguage()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var languageService = new LanguageServiceImpl(dbContext, mockMapper.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<LanguageResponse>>(It.IsAny<IEnumerable<Language>>()))
                      .Returns(new List<LanguageResponse>
                      {
                          new LanguageResponse{ LanguageId = "EN", LanguageName = "English", Support = true },
                      });

            //Act
            var result = languageService.GetAllSupportedLanguages();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllSupportedLanguage_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var languageService = new LanguageServiceImpl(dbContext, mockMapper.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<LanguageResponse>>(It.IsAny<IEnumerable<Language>>()))
                      .Returns(new List<LanguageResponse>
                      {
                      });

            //Act
            var result = languageService.GetAllSupportedLanguages();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllSupportedLanguage_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var languageService = new LanguageServiceImpl(null, mockMapper.Object);

            //Act
            var result = languageService.GetAllSupportedLanguages();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task GetAllLanguages()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var languageService = new LanguageServiceImpl(dbContext, mockMapper.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<LanguageResponse>>(It.IsAny<IEnumerable<Language>>()))
                      .Returns(new List<LanguageResponse>
                      {
                          new LanguageResponse{ LanguageId = "EN", LanguageName = "English", Support = true },
                          new LanguageResponse{ LanguageId = "VI", LanguageName = "Vietnamese", Support = false }
                      });

            //Act
            var result = languageService.GetAllLanguages();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllLanguages_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var languageService = new LanguageServiceImpl(dbContext, mockMapper.Object);

            mockMapper.Setup(m => m.Map<IEnumerable<LanguageResponse>>(It.IsAny<IEnumerable<Language>>()))
                      .Returns(new List<LanguageResponse>
                      {
                      });

            //Act
            var result = languageService.GetAllLanguages();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllLanguages_Exception()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var languageService = new LanguageServiceImpl(null, mockMapper.Object);

            //Act
            var result = languageService.GetAllLanguages();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }
    }
}
