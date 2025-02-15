using Microsoft.VisualStudio.TestTools.UnitTesting;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using verbum_service_infrastructure.Impl.Service;
using AutoMapper;
using Moq;
using verbum_service_domain.DTO.Response;

namespace verbum_service_test
{
    [TestClass]
    public class CategoryServiceImplTests
    {
        private async Task<verbumContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<verbumContext>()
                .UseInMemoryDatabase(databaseName: "verbum2").Options;
            var dbContext = new verbumContext(options);
            dbContext.Database.EnsureCreated();
            if(await dbContext.Categories.CountAsync() <= 0)
            {
                for(int i = 1; i <= 3; i++)
                {
                    dbContext.Categories.Add(new Category
                    {
                        CategoryName = "UnitTest"
                    });
                }
                await dbContext.SaveChangesAsync();
            }
            return dbContext;
        }

        [TestMethod]
        public async Task GetAllCategoryTest()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<IEnumerable<CategoryInfoResponse>>(It.IsAny<IEnumerable<Category>>()))
                      .Returns(new List<CategoryInfoResponse>
                      {
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" }
                      });

            var categoryServiceImpl = new CategoryServiceImpl(dbContext,mockMapper.Object);

            //Act
            var result = categoryServiceImpl.GetAllCategory();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllCategoryTest_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<IEnumerable<CategoryInfoResponse>>(It.IsAny<IEnumerable<Category>>()))
                      .Returns(new List<CategoryInfoResponse>
                      {
                          
                      });

            var categoryServiceImpl = new CategoryServiceImpl(dbContext, mockMapper.Object);

            //Act
            var result = categoryServiceImpl.GetAllCategory();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetAllCategoryTest_Exception()
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();

            var categoryServiceImpl = new CategoryServiceImpl(null, mockMapper.Object);

            //Act
            var result = categoryServiceImpl.GetAllCategory();

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task GetCategoriesByNameTest()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<IEnumerable<CategoryInfoResponse>>(It.IsAny<IEnumerable<Category>>()))
                      .Returns(new List<CategoryInfoResponse>
                      {
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" }
                      });

            var categoryServiceImpl = new CategoryServiceImpl(dbContext, mockMapper.Object);

            //Act
            var result = categoryServiceImpl.GetCategoriesByName("UnitTest");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Result.Count());
        }

        [TestMethod]
        public async Task GetCategoriesByNameTest_Empty()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<IEnumerable<CategoryInfoResponse>>(It.IsAny<IEnumerable<Category>>()))
                      .Returns(new List<CategoryInfoResponse>
                      {
                      });

            var categoryServiceImpl = new CategoryServiceImpl(dbContext, mockMapper.Object);

            //Act
            var result = categoryServiceImpl.GetCategoriesByName("OFO");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Result.Count());
        }

        [TestMethod]
        public async Task GetCategoriesByNameTest_Exception()
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<IEnumerable<CategoryInfoResponse>>(It.IsAny<IEnumerable<Category>>()))
                      .Returns(new List<CategoryInfoResponse>
                      {
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" }
                      });

            var categoryServiceImpl = new CategoryServiceImpl(null, mockMapper.Object);

            //Act
            var result = categoryServiceImpl.GetCategoriesByName("UnitTest");

            //Assert
            Assert.ThrowsException<AggregateException>(() => result.Result);
        }

        [TestMethod]
        public async Task CreateCategoryTest()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var categoryService = new CategoryServiceImpl(dbContext, mockMapper.Object);

            var newCategory = new Category
            {
                CategoryName = "New Category"
            };

            //Act
            categoryService.CreateCategory(newCategory);

            //Assert
            var addedCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "New Category");
            Assert.IsNotNull(addedCategory);
            Assert.AreEqual("New Category", addedCategory.CategoryName);
        }

        [TestMethod]
        public async Task AddRangeTest()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            var categoryService = new CategoryServiceImpl(dbContext, mockMapper.Object);

            List<string> newCategories = new List<string> { "New1", "New2" };

            //Act
            categoryService.AddRange(newCategories);

            //Assert
            var addedCategory1 = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "New1");
            Assert.IsNotNull(addedCategory1);
            Assert.AreEqual("New1", addedCategory1.CategoryName);

            var addedCategory2 = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "New2");
            Assert.IsNotNull(addedCategory2);
            Assert.AreEqual("New2", addedCategory2.CategoryName);
        }

        [TestMethod]
        public  async Task GetListIdByCategoryTest()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<IEnumerable<CategoryInfoResponse>>(It.IsAny<IEnumerable<Category>>()))
                      .Returns(new List<CategoryInfoResponse>
                      {
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" },
                          new CategoryInfoResponse { Name = "UnitTest" }
                      });

            var categoryServiceImpl = new CategoryServiceImpl(dbContext, mockMapper.Object);

            List<string> oldCategories = new List<string> { "UnitTest" };

            //Act
            var result = categoryServiceImpl.GetListIdByCategory(oldCategories);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Count());
        }

        public async Task DeleteCategoryTest()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var categoryService = new CategoryServiceImpl(dbContext, mockMapper.Object);

            var existingCategory = new Category
            {
                CategoryId = 10,
                CategoryName = "Old Category"
            };
            dbContext.Categories.Add(existingCategory);
            await dbContext.SaveChangesAsync();

            // Act
            await categoryService.DeleteCategory(10);

            // Assert
            var deletedCategory = await dbContext.Categories.FindAsync(existingCategory.CategoryId);
            Assert.IsNull(deletedCategory);
        }

        public async Task UpdateCategoryTest()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var mockMapper = new Mock<IMapper>();
            var categoryService = new CategoryServiceImpl(dbContext, mockMapper.Object);

            var existingCategory = new Category
            {
                CategoryId = 10,
                CategoryName = "Old Category"
            };
            dbContext.Categories.Add(existingCategory);
            await dbContext.SaveChangesAsync();

            // New category info to update
            var updatedCategoryInfo = new Category
            {
                CategoryId = 10,
                CategoryName = "Updated Category"
            };

            // Act
            await categoryService.UpdateCategory(updatedCategoryInfo);

            // Assert
            var updatedCategory = await dbContext.Categories.FindAsync(existingCategory.CategoryId);
            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual("Updated Category", updatedCategory.CategoryName);
        }
    }
}