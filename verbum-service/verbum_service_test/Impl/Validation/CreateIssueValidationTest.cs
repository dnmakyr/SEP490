using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class CreateIssueValidationTest
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
        public async Task CreateIssue_MissingFields()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateIssueValidation(dbContext);

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

            CreateIssueRequest request = new CreateIssueRequest
            {
                IssueName = "",
                DeliverableUrl = "something",
                IssueDescription = "",
                IssueAttachments = new List<UploadIssueAttachmentFiles>(),
                OrderId = Guid.Parse("2dd5050e-4528-4a26-a81d-8d773347317e")
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("missing fields is required"));
        }

        [TestMethod]
        public async Task CreateIssue_AttachmentTag()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateIssueValidation(dbContext);

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

            CreateIssueRequest request = new CreateIssueRequest
            {
                IssueName = "issue",
                DeliverableUrl = "something",
                IssueDescription = "issue",
                IssueAttachments = new List<UploadIssueAttachmentFiles>
                {
                    new UploadIssueAttachmentFiles
                    {
                        AttachmentUrl = "issue.com",
                        Tag = "ERROR"
                    }
                },
                OrderId = Guid.Parse("2dd5050e-4528-4a26-a81d-8d773347317e")
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("one of issue attachment's tag is invalid"));
        }

        [TestMethod]
        public async Task CreateIssue_DupplicateUrl()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateIssueValidation(dbContext);

            Issue issue = new Issue
            {
                IssueId = Guid.Parse("fb79f137-77a9-4057-9bd5-4c0434b73da4"),
                JobId = Guid.Parse("5087c2aa-a177-45bf-9b7c-337d5ed171c4"),
                SrcDocumentUrl = "issue.com",
                IssueName = "testName",
            };

            var existOrder = await dbContext.Issues.FirstOrDefaultAsync(c => c.SrcDocumentUrl == "issue.com");
            if (existOrder == null)
            {
                dbContext.Issues.Add(issue);
                await dbContext.SaveChangesAsync();
            }

            CreateIssueRequest request = new CreateIssueRequest
            {
                IssueName = "issue",
                DeliverableUrl = "issue.com",
                IssueDescription = "issue",
                IssueAttachments = new List<UploadIssueAttachmentFiles>
                {
                    new UploadIssueAttachmentFiles
                    {
                        AttachmentUrl = "issue.com",
                        Tag = "ERROR"
                    }
                },
                OrderId = Guid.Parse("2dd5050e-4528-4a26-a81d-8d773347317e")
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("deliverable url is already in database"));
        }
    }
}
