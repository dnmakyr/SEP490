using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Request
{
    public class ConfirmPaymentDTO
    {
        public string guid { get; set;}
        public string PayerID { get; set; }
        public string? Cancel { get; set; }
    }
}
