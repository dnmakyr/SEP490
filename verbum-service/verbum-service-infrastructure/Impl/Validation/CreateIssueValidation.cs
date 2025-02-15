using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_application.Validation;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    [RequiredArgsConstructor]
    public partial class CreateIssueValidation : IValidation<CreateIssueRequest>
    {
        private readonly verbumContext context;
        public async Task<List<string>> Validate(CreateIssueRequest request)
        {
            List<string> errors = new List<string>();
            if(ObjectUtils.IsEmpty(request.IssueName) || ObjectUtils.IsEmpty(request.IssueDescription) || ObjectUtils.IsEmpty(request.DeliverableUrl))
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.REQUIRED, "missing fields"));
            }
            foreach (var attachment in request.IssueAttachments)
            {
                if (!Enum.IsDefined(typeof(IssueFileTag), attachment.Tag))
                {
                    errors.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "one of issue attachment's tag"));
                }
            }
            if (!context.Orders.Where(x => x.OrderId.Equals(request.OrderId)).Select(x => x.OrderStatus).FirstOrDefault().Equals(OrderStatus.COMPLETED.ToString()))
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.ISSUE_CREATE_WHEN_ORDER_NOT_COMPLETED));
            }
            if (await context.Issues.AnyAsync(x => x.SrcDocumentUrl.Equals(request.DeliverableUrl)))
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.DUPLICATE, "deliverable url"));
            }
            return errors;
        }
    }
}
