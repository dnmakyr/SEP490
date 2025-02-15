using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_domain.Models;

namespace verbum_service_domain.DTO.Request
{
    public class CreateIssueRequest
    {
        public string? IssueName { get; set; }
        public string DeliverableUrl { get; set; }

        public string? IssueDescription { get; set; }
        public List<UploadIssueAttachmentFiles>? IssueAttachments { get; set; } = new List<UploadIssueAttachmentFiles>();
        public Guid OrderId { get; set; }
    }
}
