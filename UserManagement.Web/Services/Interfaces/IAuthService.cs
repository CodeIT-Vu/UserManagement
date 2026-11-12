using UserManagement.Web.DTOs;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    // ViewModel input must pass MVC ModelState validation before calling the service.
    public interface IAuthService
    {
        CurrentUserDto Authenticate(LoginViewModel model);

        CurrentUserDto FindActiveUser(string username);

        void Register(RegisterViewModel model);

        void ChangePassword(ChangePasswordViewModel model, string currentUsername);
    }
}
