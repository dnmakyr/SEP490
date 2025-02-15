using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;

namespace verbum_service_application.Service
{
    public interface OrderService
    {
        Task<List<OrderDetailsResponse>> GetAllOrder();
        Task AddRangeMiddle(Guid orderId, List<string> languageIds);
        Task CreateOrder(Order info);
        Task UpdateOrder(OrderUpdate request);
        Task UpdateOrderTargetLanguage(OrderUpdate request);
        Task<OrderDetailsResponse> GetOrderDetails(Guid id);
        Task ChangeOrderStatus(Guid orderId, string orderStatus);
        Task DeleteOrderReferenceFile(Guid orderId, string url);
        Task UploadOrderReferenceFile(List<UploadOrderFileRequest> request);
        Task<List<UploadOrderFileRequest>> GetAllOrderRefrenceFiles();
        Task UpdateOrderPrice(Guid orderId, decimal price);
        Task UpdateOrderRejectResponse(ResponseRequest request);
        Task CreateRevelancy(Guid orderId);
        Task<string> ConfirmPayment(ConfirmPaymentDTO request);
        Task<string> DoPayment(Guid orderId, bool depositeOrPayment);
    }
}
