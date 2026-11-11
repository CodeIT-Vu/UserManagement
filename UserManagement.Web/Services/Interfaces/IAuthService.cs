using UserManagement.Web.Models;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    // ViewModel input must pass MVC ModelState validation before calling the service.
    public interface IAuthService
    {
        AppUser Authenticate(LoginViewModel model);

        AppUser FindActiveUser(string username);

        void Register(RegisterViewModel model);

        void ChangePassword(ChangePasswordViewModel model, string currentUsername);
    }
}
