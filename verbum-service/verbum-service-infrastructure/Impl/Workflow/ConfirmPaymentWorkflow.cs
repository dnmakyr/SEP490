using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_application.Service;
using verbum_service_application.Workflow;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Workflow
{
    [RequiredArgsConstructor]
    public partial class ConfirmPaymentWorkflow : AbstractWorkFlow<ConfirmPaymentDTO>
    {
        private readonly OrderService orderService;
        private readonly WorkService workService;
        private readonly JobService jobService;
        private readonly verbumContext context;
        private string redirectUrl;
        private Guid? orderId;
        protected override async Task PreStep(ConfirmPaymentDTO request)
        {

        }

        protected override async Task ValidationStep(ConfirmPaymentDTO request)
        {
        }
        protected override async Task CommonStep(ConfirmPaymentDTO request)
        {
            Receipt receipt = await context.Receipts.FirstAsync(r => r.ReceiptId.ToString() == request.guid);
            orderId = receipt.OrderId;
            Order order = await context.Orders.Include(x => x.TargetLanguages).Include(x => x.OrderReferences).Include(x => x.Works).FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (!OrderStatus.ACCEPTED.ToString().Equals(order.OrderStatus) && !OrderStatus.COMPLETED.ToString().Equals(order.OrderStatus))
            {
                throw new BusinessException("Order has been paid");
            }

            if (receipt.DepositeOrPayment)
            {
                //generate work
                GenerateWork generateWork = new GenerateWork()
                {
                    OrderId = order.OrderId,
                    DueDate = order.DueDate,
                    OrderName = order.OrderName,
                    HasEditService = order.HasEditService,
                    HasEvaluateService = order.HasEvaluateService,
                    HasTranslateService = order.HasTranslateService
                };
                await workService.GenerateWork(generateWork);
                //generate job
                CreateJobsRequest createJobsRequest = new CreateJobsRequest()
                {
                    WorkIds = order.Works.Select(x => x.WorkId).ToList(),
                    DocumentUrls = order.OrderReferences.Where(x => x.Tag.Equals(OrderFileTag.TRANSLATION.ToString())).Select(x => x.ReferenceFileUrl).ToList(),
                    TargetLanguageIds = order.TargetLanguages.Select(x => x.LanguageId).ToList()
                };
                await jobService.CreateJobs(createJobsRequest);
            }
        }

        protected override async Task PostStep(ConfirmPaymentDTO request)
        {
            redirectUrl = await orderService.ConfirmPayment(request);
        }

        public string GetResult()
        {
            return redirectUrl;
        }

    }
}
