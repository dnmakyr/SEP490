using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayPal.Api;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.JustMock;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class UpdateOrderValidationTest
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
        public async Task UpdateOrderValidation_TargetLanguageIdEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateOrderValidation(dbContext);

            OrderUpdate request = new OrderUpdate
            {
                OrderId = Guid.NewGuid(),
                OrderName = "Order",
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> {  },
                TranslateService = true,
                EditService = true,
                EvaluateService = false
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("TargetLanguage is required"));
        }

        [TestMethod]
        public async Task UpdateOrderValidation_SourceLanguageEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateOrderValidation(dbContext);

            OrderUpdate request = new OrderUpdate
            {
                OrderId = Guid.NewGuid(),
                OrderName = "Order",
                SourceLanguageId = "",
                TargetLanguageIdList = new List<string> { "abc" },
                TranslateService = true,
                EditService = true,
                EvaluateService = false
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("SourceLanguageId is required"));
        }

        [TestMethod]
        public async Task UpdateOrderValidation_ServiceEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateOrderValidation(dbContext);

            OrderUpdate request = new OrderUpdate
            {
                OrderId = Guid.NewGuid(),
                OrderName = "Order",
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                TranslateService = false,
                EditService = false,
                EvaluateService = false
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("You need to order at least 1 service is required"));
        }

        [TestMethod]
        public async Task UpdateOrderValidation_OrderNote()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateOrderValidation(dbContext);

            OrderUpdate request = new OrderUpdate
            {
                OrderId = Guid.NewGuid(),
                OrderName = "Order",
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                TranslateService = false,
                EditService = false,
                EvaluateService = false,
                OrderNote = "This is a sample order note that exceeds the maximum allowed character limit of 255 characters.It is important to provide detailed information about the order,including any specific requirements,preferences,or instructions that need to be followed during the processing of this order.Please ensure that all relevant details are included to avoid any misunderstandings or errors in the order fulfillment process."
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("OrderNote can not be over 255 characters is invalid"));
        }

        [TestMethod]
        public async Task UpdateOrderValidation_SourceLanguage()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateOrderValidation(dbContext);

            OrderUpdate request = new OrderUpdate
            {
                OrderId = Guid.NewGuid(),
                OrderName = "Order",
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                TranslateService = true,
                EditService = false,
                EvaluateService = false
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("SourceLanguage is not found in database"));
        }

        [TestMethod]
        public async Task UpdateOrderValidation_TargetLanguage()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateOrderValidation(dbContext);

            OrderUpdate request = new OrderUpdate
            {
                OrderId = Guid.NewGuid(),
                OrderName = "Order",
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                TranslateService = true,
                EditService = false,
                EvaluateService = false
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("TargetLanguage is not found in database"));
        }
    }
}
