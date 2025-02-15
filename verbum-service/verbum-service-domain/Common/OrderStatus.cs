namespace verbum_service_domain.Common
{
    public enum OrderStatus
    {
        NEW,
        ACCEPTED,
        REJECTED,
        CANCELLED,
        IN_PROGRESS,
        COMPLETED,
        DELIVERED
    }
    public static class OrderStatusExtensions
    {
        public static bool IsActive(this string status)
        {
            return status == OrderStatus.NEW.ToString() ||
                   status == OrderStatus.ACCEPTED.ToString() ||
                   status == OrderStatus.IN_PROGRESS.ToString() ||
                   status == OrderStatus.DELIVERED.ToString();
        }
    }
}
