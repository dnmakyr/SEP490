using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Request
{
    public class UpdateJobRequest
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        /// <summary>
        /// NEW, IN_PROGRESS, COMPLETED
        /// </summary>
        public string? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public string? DeliverableUrl { get; set; }
        public List<Guid>? AssigneesId { get; set; }
    }
}
