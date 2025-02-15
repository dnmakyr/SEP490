using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Request
{
    public class ReopenIssueRequest
    {
        public Guid IssueId { get; set; }
        public string IssueName { get; set; }
        public string IssueDescription { get; set; }
        public List<UpdateIssueAttachmentFile>? IssueAttachments { get; set; } = new List<UpdateIssueAttachmentFile>();
    }
}
