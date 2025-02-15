using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;

namespace verbum_service_domain.DTO.Response
{
    public class JobInfoResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        /// <summary>
        /// NEW, IN_PROGRESS, COMPLETED
        /// </summary>
        public string Status { get; set; }
        public DateTime WorkDueDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int WordCount { get; set; }

        public string DocumentUrl { get; set; } = null!;
        public string? DeliverableUrl { get; set; }

        public string TargetLanguageId { get; set; } = null!;

        public Guid? WorkId { get; set; }
        public Guid OrderId { get; set; }
        public int ServiceOrder { get; set; }
        public IssueResponse? Issue { get; set; }
        public ICollection<UserInfo> AssigneeNames { get; set; }
        public Dictionary<string, string> PreviousJobDeliverables { get; set; }
        public List<string> ReferenceUrls { get; set; }
        public string? RejectReason { get; set; }
    }
}
