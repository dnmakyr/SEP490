using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_domain.Common;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Service
{
    public interface IssueService
    {
        Task AddIssue(CreateIssueRequest request);
        Task<List<IssueResponse>> ViewAllIssue();
        Task UpdateIssue(UpdateIssueRequest request);
        Task UploadIssueAttachment(List<UploadIssueAttachmentFiles> attachmentFiles);
        Task DeleteIssueAttachmentFile(Guid issueId, string attachmentUrl);
        Task<List<UploadIssueAttachmentFiles>> GetAllIssueAttachments();
        Task UpdateIssueStatus(Guid issueId, string status);
        Task UpdateIssueCancelResponse(ResponseRequest request);
        Task UpdateIssueRejectResponse(ResponseRequest request);
        Task AcceptIssueSolution (Guid issueId);
        Task ReopenIssue(ReopenIssueRequest request);
    }
}
