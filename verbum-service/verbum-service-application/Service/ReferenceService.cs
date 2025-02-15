namespace verbum_service_application.Service
{
    public interface ReferenceService
    {
        Task AddRange(Guid orderId, List<string> fileURLs, string tag);
        Task UpdateReference(Guid orderId, List<string> fileURLs);
    }
}
