using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Response
{
    public class DiscountResponse
    {
        public Guid DiscountId { get; set; }
        public decimal DiscountPercent { get; set; }

        public string DiscountName { get; set; }
    }
}
