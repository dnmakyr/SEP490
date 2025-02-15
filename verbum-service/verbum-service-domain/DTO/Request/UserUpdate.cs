namespace verbum_service_domain.DTO.Request
{
    public class UserUpdate
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public UserUpdateInfo Data { get; set; }
    }
}
