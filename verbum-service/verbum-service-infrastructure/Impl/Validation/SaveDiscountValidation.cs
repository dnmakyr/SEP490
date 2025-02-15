using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_application.Validation;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    [RequiredArgsConstructor]
    public partial class SaveDiscountValidation : IValidation<DiscountDTO>
    {
        private readonly verbumContext context;
        public async Task<List<string>> Validate(DiscountDTO request)
        {
            List<string> errors = new List<string>();
            ValidateRequired(request, errors);
            ValidateDiscountPercent(request, errors);
            return errors;
        }
        public void ValidateRequired(DiscountDTO request, List<string> errors)
        {
            if(ObjectUtils.IsEmpty(request.DiscountPercent))
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "discount percent"));
            }
            if(ObjectUtils.IsEmpty(request.DiscountName))
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "discount name"));
            }
        }
        public void ValidateDiscountPercent(DiscountDTO request, List<string> errors)
        {
            if (request.DiscountPercent <= 0 || request.DiscountPercent >= 100)
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "discount percentage"));
            }
        }
    }
}
