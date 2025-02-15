using AutoMapper;
using verbum_service_application.Service;
using verbum_service_application.Workflow;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Workflow
{
    public class UpdateCategoryWorkflow:AbstractWorkFlow<CategoryUpdate>
    {
        private readonly IMapper mapper;
        private readonly UpdateCategoryValidation validation;
        private readonly CategoryService categoryService;
        private Category category = new Category();
        public UpdateCategoryWorkflow(IMapper mapper, UpdateCategoryValidation validation, CategoryService categoryService)
        {
            this.mapper = mapper;
            this.validation = validation;
            this.categoryService = categoryService;
        }

        protected async override Task PreStep(CategoryUpdate request)
        {
        }

        protected async override Task ValidationStep(CategoryUpdate request)
        {
            List<string> alerts = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
        }

        protected async override Task CommonStep(CategoryUpdate request)
        {
            category = mapper.Map<Category>(request);
        }

        protected async override Task PostStep(CategoryUpdate request)
        {
            await categoryService.UpdateCategory(category);
        }
    }
}
