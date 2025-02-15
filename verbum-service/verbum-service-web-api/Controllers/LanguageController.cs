using Lombok.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace verbum_service.Controllers
{
    [Route("api/lang")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class LanguageController : ControllerBase
    {
        private readonly LanguageService languageService;
        [HttpPut("support")]
        //[Roles(UserRole.ADMIN)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSupportedLanguages ([FromBody] List<UpdateLanguageSupportRequest> request)
        {
            await languageService.UpdateSupportLanguages(request);
            return NoContent();
        }
        [HttpGet("support")]
        //[Roles(UserRole.CLIENT)]
        [ProducesResponseType(typeof(List<LanguageResponse>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSupportedLanguages()
        {
            return ResponseFilter.OkOrNoContent(await languageService.GetAllSupportedLanguages(), this);
        }
        [HttpGet]
        //[Roles(UserRole.ADMIN)]
        [ProducesResponseType(typeof(List<LanguageResponse>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllLanguages()
        {
            return ResponseFilter.OkOrNoContent(await languageService.GetAllLanguages(), this);
        }
    }
}
