using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.PagedList;

namespace verbum_service_application.Service
{
    public interface UserService
    {
        Task<Tokens> Login(UserLogin loginCredentials);
        Task<Tokens> RefreshAccessToken(Tokens tokens);
        Task SignUp(User user);
        Task<Tokens> ConfirmEmail(string email);
        Task<string> SendConfirmationEmail(string email);
        Task<Tokens> LoginGoogleCallback();
        Task UpdateUser(UserUpdate userUpdate);
        Task<List<UserInfo>> GetAssignList();
    }
}
