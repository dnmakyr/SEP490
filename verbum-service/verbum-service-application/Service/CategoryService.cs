using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Service
{
    public interface CategoryService
    {
        Task<List<CategoryInfoResponse>> GetAllCategory();
        Task CreateCategory(Category info);
        Task AddRange(List<string> info);
        Task<List<int>> GetListIdByCategory(List<string> info);
        Task UpdateCategory(Category info);
        Task DeleteCategory(int id);
        Task<List<CategoryInfoResponse>> GetCategoriesByName(string name);
    }
}
