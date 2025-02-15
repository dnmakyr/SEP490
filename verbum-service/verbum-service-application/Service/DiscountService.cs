using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Service
{
    public interface DiscountService
    {
        Task<List<DiscountResponse>> GetAllDiscount();
        Task<DiscountResponse> GetDiscountById(Guid guid);
        Task AddDiscount(DiscountDTO request);
        Task UpdateDiscount(DiscountDTO request);
        Task DeleteDiscount(Guid discountId);
    }
}
