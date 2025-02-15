using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Service
{
    public interface RatingService
    {
        Task<List<RatingResponse>> GetAllRating();
        Task CreateRating(Rating rating);
        Task UpdateRating(RatingUpdate rating);
        Task DeleteRating(Guid guid);
    }
}
