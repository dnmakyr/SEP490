using AutoMapper;
using Lombok.NET;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class WorkServiceImpl:WorkService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        private readonly CurrentUser currentUser;

        public async Task AddRange(Guid orderId, DateTime? dueDate, List<string> serviceCodes)
        {
            var works = serviceCodes.Select(serviceCode => new Work
            {
                WorkId = Guid.NewGuid(),
                OrderId = orderId,
                ServiceCode = serviceCode,
                CreatedDate = DateTime.Now,
                DueDate = dueDate
            });

            context.Works.AddRange(works);
            await context.SaveChangesAsync();
        }

        public async Task AddWorkCategory(Guid workId, List<int> categoryIds)
        {
            var categories = context.Categories.Where(c => categoryIds.Contains(c.CategoryId)).ToList();

            var work = context.Works.FirstOrDefault(x => x.WorkId == workId);
            if (work != null)
            {
                foreach (var category in categories)
                {
                    work.Categories.Add(category);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<WorkResponse>> GetAllWork()
        {
            List<Work> orders = await context.Works
                        .Include(w => w.Order).ThenInclude(w => w.TargetLanguages)
                        .Include(w => w.Order).ThenInclude(w => w.OrderReferences).Include(x => x.Jobs).ThenInclude(x => x.Assignees).ToListAsync();
            Guid clientId = currentUser.Id;
            switch (currentUser.Role)
            {
                case UserRole.TRANSLATE_MANAGER:
                    orders = orders
                        .Where(w => w.ServiceCode == "TL")
                        .ToList();
                    break;
                case UserRole.EDIT_MANAGER:
                    orders = orders
                        .Where(w => w.ServiceCode == "ED").ToList();
                    break;
                case UserRole.EVALUATE_MANAGER:
                    orders = orders
                        .Where(w => w.ServiceCode == "EV").ToList();
                    break;
                case UserRole.LINGUIST:
                    orders = orders
                        .Where(w => w.Jobs.Any(j => j.Assignees.Any(a => a.Id == currentUser.Id)))
                        .ToList();
                    break;
                default:
                    throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Role"));
            }
            List<WorkResponse> list = mapper.Map<List<WorkResponse>>(orders);
            return list;
        }

        public async Task<List<Guid>> GenerateWork(GenerateWork request)
        {
            List<string> serviceCodes = new List<string>();
            if (request.HasTranslateService) serviceCodes.Add("TL");
            if (request.HasEditService) serviceCodes.Add("ED");
            if (request.HasEvaluateService) serviceCodes.Add("EV");

            if (ObjectUtils.IsEmpty(serviceCodes))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.INVALID, "ServiceCodes"));
            }
            else
            {
                var general = context.Categories.Where(c => c.CategoryId == 6).ToList();

                var works = serviceCodes.Select(serviceCode => new Work
                {
                    WorkId = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    WorkName = request.OrderName+"_"+serviceCode,
                    ServiceCode = serviceCode,
                    CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                    DueDate = request.DueDate,
                    Categories = general
                }).ToList();

                var workIds = works.Select(w => w.WorkId).ToList();

                context.Works.AddRange(works);
                await context.SaveChangesAsync();

                return workIds;
            }
        }
    }
}
