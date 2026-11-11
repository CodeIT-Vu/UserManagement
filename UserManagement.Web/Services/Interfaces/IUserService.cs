using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    // ViewModel input must pass MVC ModelState validation before calling the service.
    public interface IUserService
    {
        UserListViewModel Search(UserSearchViewModel model);

        UserDetailsViewModel GetDetails(int id);

        UserEditViewModel GetEdit(int id);

        void Update(UserEditViewModel model, string currentUsername);
    }
}
