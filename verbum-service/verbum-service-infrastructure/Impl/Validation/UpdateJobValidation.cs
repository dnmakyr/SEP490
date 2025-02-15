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
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Validation
{
    [RequiredArgsConstructor]
    public partial class UpdateJobValidation : IValidation<UpdateJobRequest>
    {
        private readonly verbumContext context;
        public async Task<List<string>> Validate(UpdateJobRequest request)
        {
            List<string> errors = new List<string>();
            if (DateTime.Now >= request.DueDate)
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "due date"));
            }
            if (!Enum.IsDefined(typeof(JobStatus), request.Status) || request.Status.Equals(JobStatus.IN_PROGRESS))
            {
                errors.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "job status"));
            }
            return errors;
        }
    }
}
