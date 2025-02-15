using AutoMapper;
using CloudinaryDotNet.Actions;
using Lombok.NET;
using Microsoft.EntityFrameworkCore;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Service
{
    [RequiredArgsConstructor]
    public partial class ReceiptServiceImpl: ReceiptService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        private readonly CurrentUser currentUser;

        public async Task<List<ReceiptInfoResponse>> GetAllReceipt()
        {
            List<Receipt> listReceipt = new List<Receipt>();
            switch (currentUser.Role)
            {
                case UserRole.CLIENT:
                    listReceipt = await context.Receipts.Include(r => r.Order).Where(r => r.Order.ClientId == currentUser.Id).ToListAsync();
                    break;
                default:
                    throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Role"));
            }
            if (listReceipt == null)
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "Receipt"));
            }
            List<ReceiptInfoResponse> result = mapper.Map<List<ReceiptInfoResponse>>(listReceipt);
            return result;
        }
    }
}
