using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    public interface IUserService
    {
        UserListViewModel Search(UserSearchViewModel model);

        UserDetailsViewModel GetDetails(int id);

        UserEditViewModel GetEdit(int id);

        void Update(UserEditViewModel model, string currentUsername);
    }
}
