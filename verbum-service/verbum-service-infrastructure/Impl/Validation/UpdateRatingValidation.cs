using Microsoft.EntityFrameworkCore;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    public class UpdateRatingValidation : IValidation<RatingUpdate>
    {
        private readonly verbumContext context;
        public UpdateRatingValidation(verbumContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> Validate(RatingUpdate request)
        {
            List<string> alerts = new List<string>();
            ValidateEmpty(request, alerts);
            await ValidateExist(request, alerts);
            return alerts;
        }

        private void ValidateEmpty(RatingUpdate request, List<string> alerts)
        {
            if (request.InTime < 0 || request.InTime > 5)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "Rating InTime"));
            }
            if (request.Expectation < 0 || request.Expectation > 5)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "Rating Expectation"));
            }
            if (request.IssueResolved < 0 || request.IssueResolved > 5)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "Rating IssueResolved"));
            }
        }

        private async Task ValidateExist(RatingUpdate request, List<string> alerts)
        {
            if (!await context.Ratings.AnyAsync(d => d.RatingId == request.RatingId))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Rating"));
            }
        }
    }
}
