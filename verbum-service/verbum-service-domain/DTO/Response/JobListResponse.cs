using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Response
{
    public class JobListResponse
    {
        public Guid Id { get; set; }
        public Guid WorkId { get; set; }

        public string Name { get; set; } = null!;

        /// <summary>
        /// NEW, IN_PROGRESS, COMPLETED
        /// </summary>
        public string Status { get; set; }
        public ICollection<UserInfo> AssigneeNames { get; set; }
        public string TargetLanguageId { get; set; } = null!;
    }
}
