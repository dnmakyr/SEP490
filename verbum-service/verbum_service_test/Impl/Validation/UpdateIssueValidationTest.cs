using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;
using verbum_service_domain.Models;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class UpdateIssueValidationTest
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
        public async Task UpdateIssue_MissingFields()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateIssueValidation(dbContext);

            Order order = new Order
            {
                OrderId = Guid.Parse("2dd5050e-4528-4a26-a81d-8d773347317e"),
                ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                SourceLanguageId = "EN",
                HasEditService = true,
                HasTranslateService = true,
                HasEvaluateService = true,
                OrderStatus = "NEW"
            };

            var existOrder = await dbContext.Orders.FirstOrDefaultAsync(c => c.OrderId == order.OrderId);
            if (existOrder == null)
            {
                dbContext.Orders.Add(order);
                await dbContext.SaveChangesAsync();
            }

            UpdateIssueRequest request = new UpdateIssueRequest
            {
                IssueId = Guid.NewGuid(),
                IssueName = "",
                IssueDescription = "",
                IssueAttachments = new List<UpdateIssueAttachmentFile>()
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("missing fields is required"));
        }

        [TestMethod]
        public async Task UpdateIssue_InvalidAttachmentTag()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new UpdateIssueValidation(dbContext);

            Order order = new Order
            {
                OrderId = Guid.Parse("2dd5050e-4528-4a26-a81d-8d773347317e"),
                ClientId = Guid.Parse("80d4d6dd-8f0a-479e-b4a6-1016f34ec78a"),
                SourceLanguageId = "EN",
                HasEditService = true,
                HasTranslateService = true,
                HasEvaluateService = true,
                OrderStatus = "NEW"
            };

            var existOrder = await dbContext.Orders.FirstOrDefaultAsync(c => c.OrderId == order.OrderId);
            if (existOrder == null)
            {
                dbContext.Orders.Add(order);
                await dbContext.SaveChangesAsync();
            }

            UpdateIssueRequest request = new UpdateIssueRequest
            {
                IssueId = Guid.NewGuid(),
                IssueName = "",
                IssueDescription = "",
                IssueAttachments = new List<UpdateIssueAttachmentFile>
                {
                    new UpdateIssueAttachmentFile
                    {
                        AttachmentUrl = "issue.com",
                        Tag = "ERROR2"
                    },
                    new UpdateIssueAttachmentFile
                    {
                        AttachmentUrl = "issue2.com",
                        Tag = "ERROR2"
                    }
                }
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("one of issue attachment's tag is invalid"));
        }
    }
}
