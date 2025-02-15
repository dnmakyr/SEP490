using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Service
{
    public interface WorkService
    {
        Task<List<WorkResponse>> GetAllWork();
        Task AddRange(Guid orderId,DateTime? dueDate,List<string> serviceCodes);
        Task AddWorkCategory(Guid workId, List<int> categoryIds);
        Task<List<Guid>> GenerateWork(GenerateWork request);
    }
}
