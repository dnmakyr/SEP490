using Lombok.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Org.BouncyCastle.Asn1.Ocsp;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_infrastructure.Impl.Workflow;

namespace verbum_service.Controllers
{
    [Route("api/receipt")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class ReceiptController : ControllerBase
    {
        private readonly ReceiptService receiptService;

        [HttpGet("get-all")]
        [EnableQuery]
        [Authorize]
        [ProducesResponseType(typeof(List<ReceiptInfoResponse>), 200)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            return ResponseFilter.OkOrNoContent(await receiptService.GetAllReceipt(), this);
        }
    }
}
