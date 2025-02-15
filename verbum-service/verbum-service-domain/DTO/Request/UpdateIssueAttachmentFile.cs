using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Request
{
    public class UpdateIssueAttachmentFile
    {
        public Guid IssueId { get; set; }

        public string AttachmentUrl { get; set; }
        public string Tag { get; set; }
        public bool IsDeleted { get; set; }
    }
}
