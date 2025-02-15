using System.ComponentModel.DataAnnotations;

namespace verbum_service_domain.DTO.Response
{
    public class CategoryInfoResponse
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
