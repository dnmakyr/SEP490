using Lombok.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.ComponentModel.DataAnnotations;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_infrastructure.Impl.Workflow;

namespace verbum_service.Controllers
{
    [Route("api/order")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class OrderController : ControllerBase
    {
        private readonly OrderService orderService;
        private readonly CreateOrderWorkflow createOrderWorkflow;
        private readonly UpdateOrderWorkflow updateOrderWorkflow;
        private readonly ConfirmPaymentWorkflow confirmPaymentWorkflow;

        [HttpGet("get-all")]
        [EnableQuery]
        [Authorize]
        [ProducesResponseType(typeof(List<OrderDetailsResponse>), 200)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            return ResponseFilter.OkOrNoContent(await orderService.GetAllOrder(), this);
        }

        [HttpGet("get-details")]
        //[Authorize]
        [ProducesResponseType(typeof(OrderDetailsResponse), 200)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            return ResponseFilter.OkOrNoContent(await orderService.GetOrderDetails(id), this);
        }

        [HttpPost("add")]
        [Roles(UserRole.CLIENT)]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddOrder([FromBody] OrderCreate order)
        {
            await createOrderWorkflow.process(order);
            return NoContent();
        }

        [HttpPut("update")]
        [Roles(UserRole.CLIENT)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdate order)
        {
            await updateOrderWorkflow.process(order);
            return NoContent();
        }

        [HttpPut("price")]
        [Roles(UserRole.DIRECTOR)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOrderPrice([FromQuery][Required] Guid orderId, [FromQuery][Required] decimal price)
        {
            await orderService.UpdateOrderPrice(orderId, price);
            return NoContent();
        }

        [HttpPut("change-status")]
        [Roles(UserRole.STAFF, UserRole.CLIENT)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)] 
        [ProducesResponseType(500)]
        public async Task<IActionResult> ChangeOrderStatus([FromQuery][Required]Guid orderId, [FromQuery][Required]string orderStatus)
        {
            await orderService.ChangeOrderStatus(orderId, orderStatus);
            return NoContent();
        }

        [HttpGet("confirm-payment")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ConfirmPayment([Required] string PayerID, string? Cancel, [Required] string guid)
        {
            await confirmPaymentWorkflow.process(new ConfirmPaymentDTO()
            {
                PayerID = PayerID,
                Cancel = Cancel,
                guid = guid
            });
            return Redirect(confirmPaymentWorkflow.GetResult());
        }

        [HttpGet("file")]
        [EnableQuery]
        [Roles(UserRole.MANAGER, UserRole.CLIENT)]
        [ProducesResponseType(typeof(List<UploadOrderFileRequest>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllOrderReferenceFiles()
        {
            return ResponseFilter.OkOrNoContent(await orderService.GetAllOrderRefrenceFiles(), this);
        }

        [HttpPost("file")]
        [Roles(UserRole.MANAGER, UserRole.CLIENT, UserRole.LINGUIST)]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadOrderReferenceFile(List<UploadOrderFileRequest> request)
        {
            await orderService.UploadOrderReferenceFile(request);
            return Created();
        }
        [HttpDelete("file")]
        [Roles(UserRole.CLIENT, UserRole.MANAGER)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteOrderReferenceFile(Guid orderId, string fileURl)
        {
            await orderService.DeleteOrderReferenceFile(orderId, fileURl);
            return NoContent();
        }

        [HttpPut("send-reject-response")]
        //[Roles(UserRole.CLIENT, UserRole.MANAGER)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SendRejectResponse([FromBody] ResponseRequest request)
        {
            await orderService.UpdateOrderRejectResponse(request);
            return NoContent();
        }

        [HttpPost("revelancy")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Revelancy(Guid orderId)
        {
            await orderService.CreateRevelancy(orderId);
            return StatusCode(201);
        }

        [HttpGet("payment")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PayForOrder([Required]Guid orderId, [Required]bool isDeposit)
        {
            return Redirect(await orderService.DoPayment(orderId, isDeposit));
        }
    }
}
