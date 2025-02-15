namespace verbum_service_domain.DTO.Response
{
    public class ReceiptInfoResponse
    {
        public string OrderName { get; set; }

        public DateTime PayDate { get; set; }

        public bool DepositeOrPayment { get; set; }

        public decimal Amount { get; set; }
        public Guid OrderId { get; set; }
    }
}
