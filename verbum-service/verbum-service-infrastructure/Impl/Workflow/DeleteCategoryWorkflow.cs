
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
    public class DeleteCategoryWorkflow: AbstractWorkFlow<CategoryDelete>
    {
        private readonly IMapper mapper;
        private readonly DeleteCategoryValidation validation;
        private readonly CategoryService categoryService;
        public DeleteCategoryWorkflow(IMapper mapper, DeleteCategoryValidation validation, CategoryService categoryService)
        {
            this.mapper = mapper;
            this.validation = validation;
            this.categoryService = categoryService;
        }

        protected async override Task PreStep(CategoryDelete request)
        {
        }

        protected async override Task ValidationStep(CategoryDelete request)
        {
            List<string> alerts = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
        }

        protected async override Task CommonStep(CategoryDelete request)
        {
        }

        protected async override Task PostStep(CategoryDelete request)
        {
            await categoryService.DeleteCategory(request.Id);
        }
    }
}
