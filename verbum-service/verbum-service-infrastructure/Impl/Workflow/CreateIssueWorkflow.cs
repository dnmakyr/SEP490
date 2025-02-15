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
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Workflow
{
    [RequiredArgsConstructor]
    public partial class CreateIssueWorkflow : AbstractWorkFlow<CreateIssueRequest>
    {
        private readonly CreateIssueValidation validation;
        private readonly IssueService issueService;
        protected override async Task PreStep(CreateIssueRequest request)
        {
        }

        protected override async Task ValidationStep(CreateIssueRequest request)
        {
            List<string> errors = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(errors))
            {
                throw new BusinessException(errors);
            }
        }
        protected override async Task CommonStep(CreateIssueRequest request)
        {
        }

        protected override async Task PostStep(CreateIssueRequest request)
        {
            await issueService.AddIssue(request);
        }

    }
}
