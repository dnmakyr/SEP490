using Lombok.NET;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.ComponentModel.DataAnnotations;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.Impl.Workflow;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace verbum_service.Controllers
{
    [Route("api/issue")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class IssueController : ControllerBase
    {
        private readonly IssueService issueService;
        private readonly CreateIssueWorkflow createIssueWorkflow;
        private readonly UpdateIssueWorkflow updateIssueWorkflow;
        [HttpGet]
        [EnableQuery]
        [Roles(UserRole.MANAGER, UserRole.LINGUIST, UserRole.CLIENT)]
        [ProducesResponseType(typeof(List<IssueResponse>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ViewAllIssue()
        {
            return ResponseFilter.OkOrNoContent(await issueService.ViewAllIssue(), this);
        }
        [HttpPost]
        [Roles(UserRole.CLIENT)]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddIssue(CreateIssueRequest request)
        {
            await createIssueWorkflow.process(request);
            return StatusCode(201);
        }

        [HttpPut]
        //note
        [Roles(UserRole.CLIENT, UserRole.MANAGER, UserRole.LINGUIST)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateIssue(UpdateIssueRequest request)
        {
            await updateIssueWorkflow.process(request);
            return NoContent();
        }

        [HttpPut("change-status")]
        //note
        [Roles(UserRole.CLIENT, UserRole.MANAGER, UserRole.LINGUIST)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateIssueStatus([FromQuery][Required] Guid issueId, [FromQuery][Required]string status)
        {
            await issueService.UpdateIssueStatus(issueId, status);
            return NoContent();
        }

        [HttpPut("send-cancel-response")]
        //[Roles(UserRole.CLIENT, UserRole.MANAGER)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SendCancelResponse([FromBody] ResponseRequest request)
        {
            await issueService.UpdateIssueCancelResponse(request);
            return NoContent();
        }

        [HttpPut("send-reject-response")]
        //[Roles(UserRole.CLIENT, UserRole.MANAGER)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SendRejectResponse([FromBody] ResponseRequest request)
        {
            await issueService.UpdateIssueRejectResponse(request);
            return NoContent();
        }

        [HttpPut("accept-issue-solution")]
        [Roles(UserRole.MANAGER)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AcceptIssueSolution([FromQuery][Required] Guid issueId)
        {
            await issueService.AcceptIssueSolution(issueId);
            return NoContent();
        }

        [HttpPut("reopen")]
        [Roles(UserRole.CLIENT)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ReopenIssue([FromBody] ReopenIssueRequest request)
        {
            await issueService.ReopenIssue(request);
            return NoContent();
        }
    }
}
