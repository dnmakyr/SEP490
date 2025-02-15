using Microsoft.EntityFrameworkCore;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    public class CreateOrderValidation : IValidation<OrderCreate>
    {
        private readonly verbumContext context;
        public CreateOrderValidation(verbumContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> Validate(OrderCreate request)
        {
            List<string> alerts = new List<string>();
            ValidateEmpty(request, alerts);
            await ValidateExist(request, alerts);
            return alerts;
        }

        private void ValidateEmpty(OrderCreate request, List<string> alerts)
        {
            if (ObjectUtils.IsEmpty(request.TargetLanguageIdList))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "TargetLanguage"));
            }
            if (ObjectUtils.IsEmpty(request.SourceLanguageId))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "SourceLanguageId"));
            }
            if (request.HasTranslateService == false && request.HasEvaluateService == false && request.HasEditService == false)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "You need to order at least 1 service"));
            }
            if (ObjectUtils.IsEmpty(request.TranslationFileURL))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "Translation File"));
            }

            if (ObjectUtils.IsNotEmpty(request.OrderNote))
            {
                if (request.OrderNote.Length > 255)
                {
                    alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "OrderNote can not be over 255 characters"));
                }
            }
        }

        private async Task ValidateExist(OrderCreate request, List<string> alerts)
        {
            if (!await context.Languages.AnyAsync(c => c.LanguageId == request.SourceLanguageId))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "SourceLanguage"));
            }
            if (!request.TargetLanguageIdList.All(id => context.Languages.Any(c => c.LanguageId == id)))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "TargetLanguage"));
            }
            if (ObjectUtils.IsNotEmpty(request.DiscountId))
            {
                    if (await context.Orders.AnyAsync(o => o.DiscountId == request.DiscountId))
                    {
                        alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "Discount code was used"));
                    }
                    if (!await context.Discounts.AnyAsync(d => d.DiscountId == request.DiscountId))
                    {
                        alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Discount Code"));
                    }
            }
        }

    }
}
