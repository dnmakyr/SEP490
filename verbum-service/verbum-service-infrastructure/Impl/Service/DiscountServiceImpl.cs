using AutoMapper;
using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;
using verbum_service_infrastructure.Impl.Validation;

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class DiscountServiceImpl : DiscountService
    {
        private readonly IMapper mapper;
        private readonly verbumContext context;
        private readonly SaveDiscountValidation validation;
        public async Task AddDiscount(DiscountDTO request)
        {
            List<string> errors = await validation.Validate(request);
            request.DiscountId = Guid.NewGuid();
            if (ObjectUtils.IsNotEmpty(errors))
            {
                throw new BusinessException(errors);
            }
            context.Discounts.Add(mapper.Map<Discount>(request));
            if(await context.SaveChangesAsync() < 1)
            {
                throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            }
        }

        public async Task DeleteDiscount(Guid discountId)
        {
            Discount discount = await context.Discounts.Include(x => x.Orders).FirstOrDefaultAsync(x => x.DiscountId == discountId);
            if (discount.Orders.Any(x => x.OrderStatus.IsActive()))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.CANNOT_DELETE, "discount"));
            }
            if(await context.Discounts.Where(x => x.DiscountId == discountId).ExecuteDeleteAsync() < 1)
            {
                throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            }
        }

        public async Task<List<DiscountResponse>> GetAllDiscount()
        {
            return mapper.Map<List<DiscountResponse>>(await context.Discounts.ToListAsync());
        }

        public async Task<DiscountResponse> GetDiscountById(Guid guid)
        {
            Discount discount = await context.Discounts.FirstOrDefaultAsync(d => d.DiscountId == guid);
            if (ObjectUtils.IsEmpty(discount))
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Discount"));
            }
            return mapper.Map<DiscountResponse>(discount);
        }

        public async Task UpdateDiscount(DiscountDTO request)
        {
            List<string> errors = await validation.Validate(request);
            if (ObjectUtils.IsNotEmpty(errors))
            {
                throw new BusinessException(errors);
            }
            context.Discounts.Update(mapper.Map<Discount>(request));
            if(await context.SaveChangesAsync() < 1)
            {
                throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            }
        }
    }
}
