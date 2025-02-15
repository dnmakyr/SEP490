using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;

namespace verbum_service_domain.DTO.Response
{
    public class IssueResponse
    {
        public Guid IssueId { get; set; }

        public string? IssueName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// CANCEL, OPEN, RESOLVED, ACCEPTED
        /// </summary>
        public string? Status { get; set; }

        public string? ClientName { get; set; }

        public Guid? JobId { get; set; }
        public Guid OrderId { get; set; }
        public string OrderName { get; set; }
        public string? DocumentUrl { get; set; }

        public string? IssueDescription { get; set; }

        public string? AssigneeName { get; set; }
        public string? CancelResponse { get; set; }
        public string? RejectResponse { get; set; }
        public ICollection<UploadIssueAttachmentFiles> IssueAttachments { get; set; } = new List<UploadIssueAttachmentFiles>();
    }
}
