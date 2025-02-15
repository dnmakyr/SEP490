using Lombok.NET;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.DTO.Request;
using verbum_service_infrastructure.Impl.Workflow;

namespace verbum_service.Controllers
{
    [Route("api/work")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class WorkController : ControllerBase
    {
        private readonly WorkService workService;

        [HttpGet("get-all")]
        [EnableQuery]
        [Roles(UserRole.MANAGER, UserRole.LINGUIST)]
        [ProducesResponseType(typeof(List<WorkResponse>), 200)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            return ResponseFilter.OkOrNoContent(await workService.GetAllWork(), this);
        }

    }
}
