using Microsoft.AspNetCore.Authorization;

namespace verbum_service.Filter
{
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}
