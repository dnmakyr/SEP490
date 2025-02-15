using AutoMapper;
using Lombok.NET;
using verbum_service_application.Service;
using verbum_service_application.Workflow;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Workflow
{
    [RequiredArgsConstructor]
    public partial class UpdateOrderWorkflow : AbstractWorkFlow<OrderUpdate>
    {
        private readonly IMapper mapper;
        private readonly UpdateOrderValidation validation;
        private readonly OrderService orderService;

        protected async override Task PreStep(OrderUpdate request)
        {
        }

        protected async override Task ValidationStep(OrderUpdate request)
        {
            List<string> alerts = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
        }
        protected async override Task CommonStep(OrderUpdate request)
        {
            
        }

        protected async override Task PostStep(OrderUpdate request)
        {
            await orderService.UpdateOrder(request);
            await orderService.UpdateOrderTargetLanguage(request);
        }
    }
}
