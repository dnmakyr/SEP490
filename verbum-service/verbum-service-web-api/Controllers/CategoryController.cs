using Lombok.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_infrastructure.Impl.Workflow;

namespace verbum_service.Controllers
{
    [Route("api/category")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class CategoryController : ControllerBase
    {
        private readonly CategoryService categoryService;
        private readonly CreateCategoryWorkflow createCategoryWorkflow;
        private readonly UpdateCategoryWorkflow updateCategoryWorkflow;
        private readonly DeleteCategoryWorkflow deleteCategoryWorkflow;

        [HttpGet("get-all")]
        [EnableQuery]
        [Authorize]
        [ProducesResponseType(typeof(List<CategoryInfoResponse>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            return ResponseFilter.OkOrNoContent(await categoryService.GetAllCategory(), this);
        }

        [HttpGet("search-name")]
        [EnableQuery]
        [Authorize]
        [ProducesResponseType(typeof(CategoryInfoResponse), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCategoriesByName([FromQuery]string name)
        {
            return ResponseFilter.OkOrNoContent(await categoryService.GetCategoriesByName(name), this);
            //return await categoryService.GetCategoriesByName(name);
        }

        [HttpPost("add")]
        [Roles(UserRole.ADMIN)]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddCategory([FromBody] CategoryInfo category)
        {
            await createCategoryWorkflow.process(category);
            return StatusCode(201);
        }

        [HttpPut("update")]
        [Roles(UserRole.ADMIN)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdate category)
        {
            await updateCategoryWorkflow.process(category);
            return NoContent();
        }

        [HttpDelete("delete")]
        [Roles(UserRole.ADMIN)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCategory([FromBody] CategoryDelete category)
        {
            await deleteCategoryWorkflow.process(category);
            return NoContent();
        }
    }
}
