using System.ComponentModel.DataAnnotations;

namespace verbum_service_domain.DTO.Response
{
    public class OrderResponse
    {
        [Key]
        public Guid OrderId { get; set; }
        public string OrderName { get; set; }
        public string OrderStatus { get; set; }
        public string CreatedDate { get; set; }
    }
}
