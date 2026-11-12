using UserManagement.Web.DTOs;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    public interface IAuthService
    {
        CurrentUserDto Authenticate(LoginViewModel model);

        CurrentUserDto FindActiveUser(string username);

        void Logout(int userId);

        void Register(RegisterViewModel model);

        void ChangePassword(ChangePasswordViewModel model, string currentUsername);
    }
}
