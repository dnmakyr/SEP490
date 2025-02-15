using AutoMapper;
using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class RatingServiceImpl:RatingService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        private readonly CurrentUser currentUser;

        public async Task CreateRating(Rating rating)
        {
            context.Ratings.Add(rating);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRating(Guid guid)
        {
            await context.Ratings.Where(c => c.RatingId == guid).ExecuteDeleteAsync();
        }

        public async Task<List<RatingResponse>> GetAllRating()
        {
            List<Rating> listRating = await context.Ratings.ToListAsync();
            if (listRating == null)
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Rating"));
            }
            List<RatingResponse> result = mapper.Map<List<RatingResponse>>(listRating);
            return result;
        }

        public async Task UpdateRating(RatingUpdate rating)
        {
            int records = await context.Ratings
                .Where(r => r.RatingId == rating.RatingId)
                .ExecuteUpdateAsync(x => x.SetProperty(r => r.InTime, rating.InTime)
                                            .SetProperty(r => r.Expectation, rating.Expectation)
                                            .SetProperty(r => r.IssueResolved, rating.IssueResolved)
                                            .SetProperty(r => r.MoreThought, rating.MoreThought));

            if (records < 1) throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
        }
    }
}
