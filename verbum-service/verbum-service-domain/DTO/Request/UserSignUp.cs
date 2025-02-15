
namespace verbum_service_domain.DTO.Request
{
    public class UserSignUp
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string RoleCode { get; set; } = null!;
    }
}
