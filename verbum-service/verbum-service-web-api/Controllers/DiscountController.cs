using Lombok.NET;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace verbum_service.Controllers
{
    [Route("api/discount")]
    [ApiController]
    //[Roles(UserRole.DIRECTOR)]
    [RequiredArgsConstructor]
    public partial class DiscountController : ControllerBase
    {
        private readonly DiscountService discountService;
        // GET: api/discount
        [HttpGet]
        [ProducesResponseType(typeof(List<DiscountResponse>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllDiscount()
        {
            return ResponseFilter.OkOrNoContent(await discountService.GetAllDiscount(), this);
        }

        [HttpGet("get-by-id")]
        [ProducesResponseType(typeof(DiscountResponse), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDiscountById([FromQuery][Required] Guid id)
        {
            return ResponseFilter.OkOrNoContent(await discountService.GetDiscountById(id), this);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddDiscount(DiscountDTO request)
        {
            await discountService.AddDiscount(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateDiscount(DiscountDTO request)
        {
            await discountService.UpdateDiscount(request);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteDiscount(Guid discountId)
        {
            await discountService.DeleteDiscount(discountId);
            return NoContent();
        }
    }
}
