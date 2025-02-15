using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_infrastructure.Impl.Workflow;

namespace verbum_service.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        private readonly UpdateUserWorkflow updateUserWorkflow;
        public UserController(UserService userService, UpdateUserWorkflow updateUserWorkflow)
        {
            this.userService = userService;
            this.updateUserWorkflow = updateUserWorkflow;
        }

        [HttpPut("update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(UserUpdate userUpdate)
        {
            await updateUserWorkflow.process(userUpdate);
            return Ok();
        }

        [HttpGet("assign-list")]
        [ProducesResponseType(typeof(List<UserInfo>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAssignList()
        {
            return ResponseFilter.OkOrNoContent(await userService.GetAssignList(), this);
        }
    }
}
