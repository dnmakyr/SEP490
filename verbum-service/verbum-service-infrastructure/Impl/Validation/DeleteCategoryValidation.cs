using Microsoft.EntityFrameworkCore;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    public class DeleteCategoryValidation: IValidation<CategoryDelete>
    {
        private readonly verbumContext context;
        public DeleteCategoryValidation(verbumContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> Validate(CategoryDelete request)
        {
            List<string> alerts = new List<string>();
            ValidateEmpty(request, alerts);
            await ValidateExist(request, alerts);
            await ValidateWorkExist(request, alerts);
            return alerts;
        }

        private void ValidateEmpty(CategoryDelete request, List<string> alerts)
        {
            if (ObjectUtils.IsEmpty(request.Id) || request.Id == 0)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "CategoryId"));
            }
        }

        private async Task ValidateExist(CategoryDelete request, List<string> alerts)
        {
            if (!await context.Categories.AnyAsync(x => x.CategoryId == request.Id))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Category"));
            }
        }

        private async Task ValidateWorkExist(CategoryDelete request, List<string> alerts)
        {
            if (await context.Categories.Include(c => c.Works).AnyAsync(x => x.CategoryId == request.Id && x.Works.Any()))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "Exist associated works"));
            }
        }
    }
}
