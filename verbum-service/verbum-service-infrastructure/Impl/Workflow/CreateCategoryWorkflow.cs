
using AutoMapper;
using System.Globalization;
using verbum_service_application.Service;
using verbum_service_application.Workflow;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Workflow
{
    public class CreateCategoryWorkflow:AbstractWorkFlow<CategoryInfo>
    {
        private readonly IMapper mapper;
        private readonly CreateCategoryValidation validation;
        private readonly CategoryService categoryService;
        private Category category = new Category();
        public CreateCategoryWorkflow(IMapper mapper, CreateCategoryValidation validation, CategoryService categoryService)
        {
            this.mapper = mapper;
            this.validation = validation;
            this.categoryService = categoryService;
        }

        protected async override Task PreStep(CategoryInfo request)
        {
        }

        protected async override Task ValidationStep(CategoryInfo request)
        {
            List<string> alerts = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
        }
        protected async override Task CommonStep(CategoryInfo request)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            request.Name = textInfo.ToTitleCase(request.Name);
            category = mapper.Map<Category>(request);
        }

        protected async override Task PostStep(CategoryInfo request)
        {
            await categoryService.CreateCategory(category);
        }
    }
}
