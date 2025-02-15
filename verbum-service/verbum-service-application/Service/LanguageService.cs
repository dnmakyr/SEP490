using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Service
{
    public interface LanguageService
    {
        Task<List<LanguageResponse>> GetAllSupportedLanguages();
        Task<List<LanguageResponse>> GetAllLanguages();
        Task UpdateSupportLanguages(List<UpdateLanguageSupportRequest> languages);
    }
}
