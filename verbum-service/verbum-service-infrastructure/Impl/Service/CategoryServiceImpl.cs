using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using verbum_service_application.Service;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Service
{
    public class CategoryServiceImpl:CategoryService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        public CategoryServiceImpl(verbumContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<CategoryInfoResponse>> GetAllCategory()
        {
            List<Category> listCategory2 = await context.Categories.ToListAsync();
            
            if (listCategory2 == null)
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Category"));
            }
            List<CategoryInfoResponse> result = mapper.Map<List<CategoryInfoResponse>>(listCategory2);
            return result;
        }

        public async Task<List<CategoryInfoResponse>> GetCategoriesByName(string name)
        {
            List<Category> listCategory = await context.Categories
                .Where(c => c.CategoryName.ToUpper().Contains(name.ToUpper()))
                .ToListAsync();
            if (listCategory == null)
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Category"));
            }
            List<CategoryInfoResponse> result = mapper.Map<List<CategoryInfoResponse>>(listCategory);
            return result;
        }

        public async Task CreateCategory(Category info)
        {
            context.Categories.Add(info);
            await context.SaveChangesAsync();
        }

        public async Task AddRange(List<string> info)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var category = info.Select(x => new Category
            {
                CategoryName = textInfo.ToTitleCase(x)
            });
            context.Categories.AddRange(category);
            await context.SaveChangesAsync();
        }

        public async Task<List<int>> GetListIdByCategory(List<string> info)
        {
            List<int> ids = new List<int>();
            foreach (string name in info)
            {
                var categoryId = context.Categories.FirstOrDefault(c => c.CategoryName == name);
                ids.Add(categoryId.CategoryId);
            }
            return ids;
        }

        public async Task DeleteCategory(int id)
        {
            await context.Categories.Where(c => c.CategoryId == id).ExecuteDeleteAsync();
        }

        public async Task UpdateCategory(Category info)
        {
            await context.Categories.Where(c => c.CategoryId == info.CategoryId).ExecuteUpdateAsync(u=>
                u.SetProperty(c => c.CategoryName, info.CategoryName));
        }

    }
}
