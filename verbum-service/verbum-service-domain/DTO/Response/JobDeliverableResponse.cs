using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Response
{
    public class JobDeliverableResponse
    {
        public string DeliverableFileUrl { get; set; }
        public int ServiceOrder { get; set; }
    }
}
