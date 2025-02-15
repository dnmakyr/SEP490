using Microsoft.EntityFrameworkCore;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    public class UpdateOrderValidation : IValidation<OrderUpdate>
    {
        private readonly verbumContext context;
        public UpdateOrderValidation(verbumContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> Validate(OrderUpdate request)
        {
            List<string> alerts = new List<string>();
            ValidateEmpty(request, alerts);
            await ValidateExist(request, alerts);
            return alerts;
        }

        private void ValidateEmpty(OrderUpdate request, List<string> alerts)
        {
            if (ObjectUtils.IsEmpty(request.TargetLanguageIdList))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "TargetLanguage"));
            }
            if (ObjectUtils.IsEmpty(request.SourceLanguageId))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "SourceLanguageId"));
            }
            if (request.TranslateService == false && request.EvaluateService == false && request.EditService == false)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "You need to order at least 1 service"));
            }
            if (ObjectUtils.IsNotEmpty(request.OrderNote))
            {
                if (request.OrderNote.Length > 255)
                {
                    alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "OrderNote can not be over 255 characters"));
                }
            }
        }

        private async Task ValidateExist(OrderUpdate request, List<string> alerts)
        {
            if (!await context.Languages.AnyAsync(c => c.LanguageId == request.SourceLanguageId))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "SourceLanguage"));
            }
            if (!request.TargetLanguageIdList.All(id => context.Languages.Any(c => c.LanguageId == id)))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "TargetLanguage"));
            }
        }
    }
}
