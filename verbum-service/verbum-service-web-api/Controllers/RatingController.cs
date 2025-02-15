using Lombok.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_infrastructure.Impl.Workflow;

namespace verbum_service.Controllers
{
    [Route("api/rating")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class RatingController : ControllerBase
    {
        private readonly RatingService ratingService;
        private readonly CreateRatingWorkflow createRatingWorkflow;
        private readonly UpdateRatingWorkflow updateRatingWorkflow;

        [HttpGet("get-all")]
        [EnableQuery]
        //[Roles(UserRole.CLIENT)]
        [ProducesResponseType(typeof(List<RatingResponse>), 200)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRating()
        {
            return ResponseFilter.OkOrNoContent(await ratingService.GetAllRating(), this);
        }

        [HttpPost("add")]
        //[Roles(UserRole.CLIENT)]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Add([FromBody] RatingCreate ratingCreate)
        {
            await createRatingWorkflow.process(ratingCreate);
            return Created();
        }

        [HttpPut("update")]
        //[Roles(UserRole.CLIENT)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update([FromBody] RatingUpdate ratingUpdate)
        {
            await updateRatingWorkflow.process(ratingUpdate);
            return NoContent();
        }

        [HttpDelete("delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(Guid guid)
        {
            await ratingService.DeleteRating(guid);
            return NoContent();
        }
    }
}
