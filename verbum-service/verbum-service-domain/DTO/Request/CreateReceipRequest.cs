namespace verbum_service_domain.DTO.Request
{
    public class CreateReceipRequest
    {
        public Guid OrderId { get; set; }
        public bool DepositeOrPayment { get; set; }
        public decimal Amount { get; set; }
    }
}
