using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Request
{
    public class DiscountDTO
    {
        public Guid DiscountId { get; set; }

        public decimal? DiscountPercent { get; set; }

        public string? DiscountName { get; set; }

        public bool IsUpdate { get; set; }
    }
}
