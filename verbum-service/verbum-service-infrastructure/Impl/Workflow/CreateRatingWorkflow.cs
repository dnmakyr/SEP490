using AutoMapper;
using CloudinaryDotNet.Actions;
using Lombok.NET;
using Microsoft.AspNetCore.Cors.Infrastructure;
using verbum_service_application.Service;
using verbum_service_application.Workflow;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Workflow
{
    [RequiredArgsConstructor]
    public partial class CreateRatingWorkflow : AbstractWorkFlow<RatingCreate>
    {
        private readonly IMapper mapper;
        private readonly CreateRatingValidation validation;
        private readonly RatingService ratingService;
        private Rating rating = new Rating();

        protected async override Task PreStep(RatingCreate request)
        {
        }

        protected async override Task ValidationStep(RatingCreate request)
        {
            List<string> alerts = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
        }
        protected async override Task CommonStep(RatingCreate request)
        {
            rating = mapper.Map<Rating>(request);
            rating.RatingId = Guid.NewGuid();
        }

        protected async override Task PostStep(RatingCreate request)
        {
            await ratingService.CreateRating(rating);
        }
    }
}
