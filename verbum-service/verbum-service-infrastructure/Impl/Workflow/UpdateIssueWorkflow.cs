using Lombok.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_application.Service;
using verbum_service_application.Workflow;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Workflow
{
    [RequiredArgsConstructor]
    public partial class UpdateIssueWorkflow : AbstractWorkFlow<UpdateIssueRequest>
    {
        private readonly UpdateIssueValidation validation;
        private readonly IssueService issueService;
        protected override async Task PreStep(UpdateIssueRequest request)
        {
        }

        protected override async Task ValidationStep(UpdateIssueRequest request)
        {
            List<string> errors = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(errors))
            {
                throw new BusinessException(errors);
            }
        }
        protected override async Task CommonStep(UpdateIssueRequest request)
        {
        }

        protected override async Task PostStep(UpdateIssueRequest request)
        {
            await issueService.UpdateIssue(request);
        }

    }
}
