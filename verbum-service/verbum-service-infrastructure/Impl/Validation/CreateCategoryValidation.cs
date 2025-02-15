using Microsoft.EntityFrameworkCore;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    public class CreateCategoryValidation:IValidation<CategoryInfo>
    {
        private readonly verbumContext context;
        public CreateCategoryValidation(verbumContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> Validate(CategoryInfo request)
        {
            List<string> alerts = new List<string>();
            ValidateEmpty(request, alerts);
            ValidateFormat(request, alerts);
            await ValidateDupplicate(request, alerts);
            return alerts;
        }

        private void ValidateEmpty(CategoryInfo request, List<string> alerts)
        {
            if (ObjectUtils.IsEmpty(request.Name))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "CategoryName"));
            }
        }

        private async Task ValidateDupplicate(CategoryInfo request, List<string> alerts)
        {
            if(await context.Categories.AnyAsync(c => c.CategoryName.ToUpper() == request.Name.ToUpper()))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.DUPLICATE, "Category"));
            }
        }

        private void ValidateFormat(CategoryInfo request, List<string> alerts)
        {
            if (!request.Name.All(char.IsLetterOrDigit))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "Category name must only contain letter or digits"));
            }
            if(request.Name.Length < 1 || request.Name.Length > 30)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "Category name must be between 1 to 30 characters"));
            }
        }
    }
}
