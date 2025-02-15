using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_test.Impl.Validation
{
    [TestClass]
    public class CreateOrderValidationTest
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();

            if (await dbContext.Languages.CountAsync() <= 0)
            {
                dbContext.Languages.Add(new Language
                {
                    LanguageId = "EN",
                    LanguageName = "English",
                    Support = true
                });
                await dbContext.SaveChangesAsync();
            }

            if (await dbContext.Roles.CountAsync() <= 0)
            {
                dbContext.Roles.Add(new Role
                {
                    RoleId = "CLIENT",
                    RoleName = "Client"
                });
                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [TestMethod]
        public async Task CreateOrderValidation_TargetLanguaegeEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" }
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("TargetLanguage is required"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_SourceLanguaegeEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" }
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("SourceLanguageId is required"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_ServiceEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = false,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" }
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("You need to order at least 1 service is required"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_TranslationFileUrlEmpty()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { }
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Translation File is required"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_OrderNote()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" },
                OrderNote = "This is a sample order note that exceeds the maximum allowed character limit of 255 characters.It is important to provide detailed information about the order,including any specific requirements,preferences,or instructions that need to be followed during the processing of this order.Please ensure that all relevant details are included to avoid any misunderstandings or errors in the order fulfillment process."
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("OrderNote can not be over 255 characters is invalid"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_SourceLanguage()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" },
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("SourceLanguage is not found in database"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_TargetLanguage()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" },
                DiscountId = null
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("TargetLanguage is not found in database"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_DiscountCodeNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" },
                DiscountId = Guid.Parse("0c2cbaf4-8550-4053-8382-76ecd5249973")
            };

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Discount Code is not found in database"));
        }

        [TestMethod]
        public async Task CreateOrderValidation_DiscountCodeDupplicate()
        {
            var dbContext = await GetDatabaseContext();
            var validation = new CreateOrderValidation(dbContext);

            OrderCreate request = new OrderCreate
            {
                SourceLanguageId = "abc",
                TargetLanguageIdList = new List<string> { "abc" },
                HasTranslateService = true,
                HasEditService = false,
                HasEvaluateService = false,
                TranslationFileURL = new List<string> { "file.com" },
                DiscountId = Guid.Parse("c0cf6e65-07be-46a1-9bbe-6a3e5e7cc5dd")
            };

            User user = new User
            {
                Id = Guid.Parse("34b15840-243f-42b6-9f7e-6f133b8ded71"),
                Name = "Tuan",
                Email = "checkingabc@gmail.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = "ACTIVE",
                RoleCode = "CLIENT"
            };

            var checkUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == "34b15840-243f-42b6-9f7e-6f133b8ded71");
            if (checkUser == null)
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }

            Order order = new Order
            {
                OrderId = Guid.NewGuid(),
                ClientId = Guid.Parse("34b15840-243f-42b6-9f7e-6f133b8ded71"),
                SourceLanguageId = "EN",
                HasEditService = true,
                HasTranslateService = true,
                HasEvaluateService = true,
                DiscountId = Guid.Parse("c0cf6e65-07be-46a1-9bbe-6a3e5e7cc5dd")
            };

            var checkOrderDisocunt = await dbContext.Orders.FirstOrDefaultAsync(u => u.DiscountId.ToString() == "c0cf6e65-07be-46a1-9bbe-6a3e5e7cc5dd");
            if(checkOrderDisocunt == null)
            {
                dbContext.Orders.Add(order);
                await dbContext.SaveChangesAsync();
            }

            //Act
            List<string> result = await validation.Validate(request);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Discount code was used is invalid"));
        }
    }
}
