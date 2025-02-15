using AutoMapper;
using Lombok.NET;
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
    public partial class UpdateRatingWorkflow : AbstractWorkFlow<RatingUpdate>
    {
        private readonly IMapper mapper;
        private readonly UpdateRatingValidation validation;
        private readonly RatingService ratingService;

        protected async override Task PreStep(RatingUpdate request)
        {
        }

        protected async override Task ValidationStep(RatingUpdate request)
        {
            List<string> alerts = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
        }
        protected async override Task CommonStep(RatingUpdate request)
        {

        }

        protected async override Task PostStep(RatingUpdate request)
        {
            await ratingService.UpdateRating(request);
        }
    }
}
