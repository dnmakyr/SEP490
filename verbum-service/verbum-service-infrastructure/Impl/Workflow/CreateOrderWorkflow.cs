using AutoMapper;
using Lombok.NET;
using System.Globalization;
using verbum_service_application.Service;
using verbum_service_application.Workflow;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Workflow
{
    [RequiredArgsConstructor]
    public partial class CreateOrderWorkflow : AbstractWorkFlow<OrderCreate>
    {
        private readonly IMapper mapper;
        private readonly CreateOrderValidation validation;
        private readonly OrderService orderService;
        private readonly ReferenceService referenceService;
        private Order order = new Order();

        protected async override Task PreStep(OrderCreate request)
        {
        }

        protected async override Task ValidationStep(OrderCreate request)
        {
            List<string> alerts = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
        }
        protected async override Task CommonStep(OrderCreate request)
        {
            order = mapper.Map<Order>(request);
            order.OrderId = Guid.NewGuid();
            order.CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            order.OrderStatus = OrderStatus.NEW.ToString();
        }

        protected async override Task PostStep(OrderCreate request)
        {
            await orderService.CreateOrder(order);
            await orderService.AddRangeMiddle(order.OrderId,request.TargetLanguageIdList);
            //await workService.AddRange(order.OrderId, request.DueDate, serviceCodes);
            //workIds = await workService.GetWorkIdsListByOrderId(order.OrderId);
            //await workService.AddRangeMiddle(workIds,request.OldCategoryIds);
            await referenceService.AddRange(order.OrderId, request.TranslationFileURL, "TRANSLATION");
            if(ObjectUtils.IsNotEmpty(request.ReferenceFileURLs)) await referenceService.AddRange(order.OrderId, request.ReferenceFileURLs, "REFERENCES");
        }
    }
}
