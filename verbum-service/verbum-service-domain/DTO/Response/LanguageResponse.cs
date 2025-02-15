using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verbum_service_domain.DTO.Response
{
    public class LanguageResponse
    {
        public string LanguageName { get; set; } = null!;

        public string LanguageId { get; set; } = null!;

        public bool Support { get; set; }
    }
}
