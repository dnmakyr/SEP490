using Microsoft.EntityFrameworkCore;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    public class UpdateCategoryValidation:IValidation<CategoryUpdate>
    {
        private readonly verbumContext context;
        public UpdateCategoryValidation(verbumContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> Validate(CategoryUpdate request)
        {
            List<string> alerts = new List<string>();
            ValidateEmpty(request, alerts);
            await ValidateExist(request, alerts);
            await ValidateDupplicate(request, alerts);
            return alerts;
        }

        private void ValidateEmpty(CategoryUpdate request, List<string> alerts)
        {
            if (ObjectUtils.IsEmpty(request.Id) || request.Id == 0)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "CategoryId"));
            }
            if (ObjectUtils.IsEmpty(request.Name))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "CategoryName"));
            }
        }

        public async Task ValidateDupplicate(CategoryUpdate request, List<string> alerts)
        {
            if (await context.Categories.AnyAsync(x => x.CategoryName.ToUpper() == request.Name.ToUpper() && x.CategoryId != request.Id))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.DUPLICATE, "Category"));
            }
        }

        private async Task ValidateExist(CategoryUpdate request, List<string> alerts)
        {
            if (!await context.Categories.AnyAsync(x => x.CategoryId == request.Id))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Category"));
            }
        }
    }
}
