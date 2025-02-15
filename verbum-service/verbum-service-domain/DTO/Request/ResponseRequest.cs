using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Request
{
    public class ResponseRequest
    {
        public Guid Id { get; set; }

        public string ResponseContent { get; set; }
    }
}
